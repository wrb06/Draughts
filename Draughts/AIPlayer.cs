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
        public Board MakeMove(Board board, BackgroundWorker worker)
        {

            Tuple<float, Position, List<Position>> mm = NegaMax(DepthOfSearch, IsWhite ? 1 : -1, float.MinValue, float.MaxValue, board, worker);

            //Console.WriteLine("\nSCORE: " + (-mm.Item1).ToString() + " MOVED: " + mm.Item2.ToString() + " TO: " + mm.Item3.Last().ToString());

            if (!mm.Item2.InBoard()) { return board; }
            else
            {
                Piece MovingPiece = board.GetPiece(mm.Item2);
                foreach (Position move in mm.Item3)
                {
                    board.MovePeice(MovingPiece.CurrentPosition, move);
                }
                //Console.WriteLine(board.ConvertForSave());
                return board;
            }
            
        }

        // Returns the best move
        private Tuple<float, Position, List<Position>> NegaMax(int Depth, int PlayerColour, float alpha, float beta, Board board, BackgroundWorker worker)
        {
            // Setup 
            float BestValue = float.MinValue;
            bool FoundTakeMove = false;
            List<List<Position>> possibleMovesets;
            Position BestPiecePosition = new Position(-1, -1);
            List<Position> usablepieces = new List<Position>();


            if (PlayerColour == 1)
            {
                usablepieces = board.GetWhitePositions();
            }
            else
            {
                usablepieces = board.GetBlackPositions();
            }

            // setup best moveset
            List<Position> BestMoveset = new List<Position>();

            //Show the tree (console only)
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
               // ShowBoard(board, Depth+1);
            }
   
            // detect if we should stop
            if (Depth == 0 || usablepieces.Count == 0 || board.WhiteHasWon() || board.BlackHasWon())
            {
                return Tuple.Create(PlayerColour*board.EvaluateBoard(), BestPiecePosition, BestMoveset); 
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
                    if (!FoundTakeMove)
                    {
                        // mark it so that every move is better that this
                        BestValue = float.MinValue;
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
                foreach (List<Position> moveset in possibleMovesets)
                {
                    // Make test board
                    Board testboard = board.MakeNewCopyOf();

                    // Make move
                    Position oldpos = pieceposition;
                    foreach (Position move in moveset)
                    {
                        testboard.MovePeice(oldpos, move);
                        oldpos = move;
                    }

                    
                    Tuple<float, Position, List<Position>> mm = NegaMax(Depth - 1, -PlayerColour, -beta, -alpha, testboard, worker);
                    float MoveScore = -mm.Item1;

                    // Change the best result if we need to
                    if (BestValue < MoveScore)
                    {
                        BestValue = MoveScore;
                        BestMoveset = moveset;
                        BestPiecePosition = pieceposition;

                        if (Debug)
                        {
                            for (int p = 0; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
                            Console.Write("New best move from " + BestPiecePosition.ToString() + " to " + BestMoveset.Last().ToString() + " with score of: " + BestValue.ToString() + " breating previous of: " +  (-mm.Item1).ToString());
                        }
                    }
                    alpha = Math.Max(alpha, BestValue);

                    if (Debug)
                    {
                        //for (int p = -1; p < (DepthOfSearch - Depth); p++) { Console.Write("\t"); }
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

            return Tuple.Create(BestValue, BestPiecePosition, BestMoveset); 
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