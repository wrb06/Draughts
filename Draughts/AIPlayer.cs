using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Draughts
{
    class AIPlayer
    {
        bool UsePruning;
        bool Debug;

        private readonly bool _isWhite;
        private readonly int _depthOfSearch;
        public bool IsWhite => _isWhite;
        public int DepthOfSearch => _depthOfSearch;

        // Constructor
        public AIPlayer(bool white, int depth, bool usePruning, bool debug = false)
        {
            _isWhite = white;
            _depthOfSearch = depth;
            UsePruning = usePruning;
            Debug = debug;
            
        }

        // Gets the move from minimax, then makes the move
        public List<Board> MakeMove(Board board, BackgroundWorker worker, ref int CountSinceLastTake)
        {
            //Console.WriteLine(DepthOfSearch);
            //Console.WriteLine((40 - CountSinceLastTake).ToString() + "Moves Untill a stalemate");
            CalculatedMove Move = NegaMax(DepthOfSearch, IsWhite ? 1 : -1, float.MinValue, float.MaxValue, board, worker);

            /*
            try
            {
                Console.Write("\nSCORE: " + Move.Value.ToString());
                Console.Write(" MOVED: " + Move.MoveFrom.ToString());
                Console.WriteLine(" TO: " + Move.Moveset.Last().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }
            */

            if (Move.MoveFrom.CouldBeTakeMove(Move.Moveset.First()))
            {
                CountSinceLastTake = 0;
            }
            else
            {
                CountSinceLastTake++;
            }

            
            if (!Move.MoveFrom.InBoard()) { return new List<Board>(); }
            else
            {
                List<Board> Boardstates = new List<Board>();
                Piece MovingPiece = board.GetPiece(Move.MoveFrom);
                foreach (Position move in Move.Moveset.Moves)
                {
                    board.MovePeice(MovingPiece.CurrentPosition, move);
                    Boardstates.Add(board.MakeNewCopyOf());
                    
                }
                // Console.WriteLine(board.ConvertForSave());
                return Boardstates;
            }       
        }

        // Returns the best move
        private CalculatedMove NegaMax(int Depth, int PlayerColour, float alpha, float beta, Board board, BackgroundWorker worker)
        {
            // Setup 
            float BestValue = float.MinValue;
            bool FoundTakeMove = false;
            bool AppliedTakeMove = false;
            List<MoveSet> possibleMovesets;

            List<Position> usablepieces = new List<Position>();
            if (PlayerColour == 1)
            {
                usablepieces = board.GetWhitePositions();            }
            else
            {
                usablepieces = board.GetBlackPositions();
            }

            //Show the tree (to the console)
            if (Debug && Depth > 0)
            {
                Console.WriteLine();
                for (int p = 0; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }

                if ((DepthOfSearch % 2 == Depth % 2 && IsWhite) || (DepthOfSearch % 2 != Depth % 2 && !IsWhite)) { Console.Write("> MAX | "); }
                else { Console.Write("> MIN | "); }

                Console.Write("Depth: " + Depth.ToString());
                Console.WriteLine();
            }
            if (Debug)
            {
                ShowBoard(board, Depth+1);
            }
   
            // detect if we should stop
            if (Depth == 0 || usablepieces.Count == 0 || board.WhiteHasWon() || board.BlackHasWon())
            {
                return new CalculatedMove(PlayerColour * board.EvaluateBoard(), new Position(-1, -1), new MoveSet());
            }

            // If we can carry on searching, setup BestPiece and BestMoveset with first valid move we come accross
            Position BestPiecePosition = new Position(-1, -1);
            MoveSet BestMoveset = new MoveSet();
            foreach (Position p in usablepieces)
            {
                BestPiecePosition = p;
                if (board.GetPiece(p).GetMoves(board).Count > 0)
                {
                    BestMoveset = board.GetPiece(p).GetMoves(board).First();
                    break;
                }
            }

            // Counts the number of pieces we have searched
            int piececount = 0;
            foreach (Position pieceposition in usablepieces)
            {
                if (Debug)
                {
                    /*
                    Console.WriteLine();
                    for (int p = -1; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                    Console.WriteLine("NEW PIECE at " + pieceposition.ToString());
                    */
                }

                // generate all possible movesets
                possibleMovesets = board.GetPiece(pieceposition).GetTakeMovesOnly(board);
                if (possibleMovesets.Count > 0)
                {
                    if (!FoundTakeMove && !AppliedTakeMove)
                    {
                        // mark it so that every move is better that this
                        FoundTakeMove = true;
                    }
                }
                else if (!FoundTakeMove)
                {
                    possibleMovesets = board.GetPiece(pieceposition).GetMoves(board);
                }

                if (Debug)
                {
                    /*
                    for (int p = 0; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                    Console.WriteLine("Take Moves found: " + FoundTakeMove);
                    */
                }

                bool BreakOuterLoop = false;
                foreach (MoveSet moveset in possibleMovesets)
                {
                    // Make test board
                    Board testboard = board.MakeNewCopyOf();

                    // Make move
                    Position oldpos = pieceposition;
                    foreach (Position move in moveset.Moves)
                    {
                        testboard.MovePeice(oldpos, move);
                        oldpos = move;
                    }

                    
                    CalculatedMove Move = NegaMax(Depth - 1, -PlayerColour, -beta, -alpha, testboard, worker);

                    // Change the best result if we need to
                    if (BestValue < Move.Value || (FoundTakeMove && !AppliedTakeMove))
                    {
                        if (FoundTakeMove && !AppliedTakeMove) { AppliedTakeMove = true; }

                        BestMoveset = moveset;
                        BestPiecePosition = pieceposition;

                        if (Debug && Move.Moveset.Count() > 0)
                        {
                            for (int p = 0; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                            Console.WriteLine("New best move from " + Move.MoveFrom.ToString() + " to " + Move.Moveset.Last().ToString() + " with score of: " + Move.Value.ToString() + " breating previous of: " + BestValue.ToString());
                        }
                        BestValue = Move.Value;
                    }
                    alpha = Math.Max(alpha, BestValue);

                    if (Debug)
                    {
                        for (int p = -1; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                        Console.WriteLine(" | Current Board Score: " + (PlayerColour*testboard.EvaluateBoard()).ToString() + " | Best Value: " + BestValue.ToString());
                    }

                    if (alpha >= beta && UsePruning)
                    {
                        if (Debug)
                        {
                            for (int p = 0; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                            Console.WriteLine(alpha.ToString() + " " + beta.ToString() + "Broke");
                        }
                        BreakOuterLoop = true;
                        break;
                    }
                }

                // If its the first call (highest depth) and we have finished a piece, report back to the worker our progress 
                if (Depth == DepthOfSearch)
                {
                    worker.ReportProgress((int)(100f * piececount) / usablepieces.Count);
                }
                piececount++;

                if (BreakOuterLoop) { break; }
                
            }
            return new CalculatedMove(BestValue, BestPiecePosition, BestMoveset); 
        }
        
        void ShowBoard(Board b, int depth)
        {
            for (int po = -1; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
            Console.WriteLine("#|0_1_2_3_4_5_6_7");
            for (int po = -1; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
            Console.Write("0|");
            int i = 0;
            foreach (Piece p in b.GetBoard())
            {

                i++;
                if (p != null)
                {
                    if (p.IsWhite)
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("W "); }
                        else { Console.Write("w "); }
                    }
                    else
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("B "); }
                        else { Console.Write("b "); }
                    }
                }
                else
                {
                    Console.Write(". ");
                }



                if (i % 8 == 0 && i != 64)
                {
                    Console.WriteLine();
                    for (int po = -1; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
                    Console.Write((i / 8) + "|");
                }
            }
            Console.WriteLine();
            Console.WriteLine();

        }
    }

}