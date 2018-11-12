using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class AIPlayer
    {
        private readonly bool _isWhite;
        private readonly int _depthOfSearch;

        public bool IsWhite => _isWhite;
        public int DepthOfSearch => _depthOfSearch;

        // Constructor
        public AIPlayer(bool white, int depth)
        {
            _isWhite = white;
            _depthOfSearch = depth;
        }

        // Gets the move from minimax, then makes the move
        public Board MakeMove(Board board)
        {
            Tuple<float, Position, List<Position>> mm = Minimax(DepthOfSearch, IsWhite, board);


            if (!mm.Item2.InBoard()) { return board; }
            else
            {
                Piece MovingPiece = board.GetPiece(mm.Item2);
                foreach (Position move in mm.Item3)
                {
                    board.MovePeice(MovingPiece.CurrentPosition, move);
                }
                return board;
            }

        }

        // Returns the best move
        private Tuple<float, Position, List<Position>> Minimax(int Depth, bool MaximisingPlayer, Board board)
        {
            // Setup 
            float BestValue;

            Position BestPiecePosition = new Position(-1, -1);
            List<Position> usablepieces = new List<Position>();
            if (MaximisingPlayer)
            {
                BestValue = int.MinValue;
                usablepieces = board.GetWhitePositions();
            }
            else
            {
                BestValue = int.MaxValue;
                usablepieces = board.GetBlackPositions();
            }

            // setup best moveset
            List<Position> BestMoveset = new List<Position>();

            // detect wins
            if (board.WhiteHasWon()) { return Tuple.Create(float.MaxValue, BestPiecePosition, BestMoveset); }
            if (board.BlackHasWon()) { return Tuple.Create(float.MinValue, BestPiecePosition, BestMoveset); }

            // detect if we should stop
            if (Depth == 0 || usablepieces.Count == 0)
            {
                return Tuple.Create(board.EvaluateBoard(), BestPiecePosition, BestMoveset);
            }

            // If we shouldnt stop
            if (MaximisingPlayer)
            {
                foreach (Position pieceposition in usablepieces)
                {
                    // generate all possible movesets
                    List<List<Position>> possibleMovesets = board.GetPiece(pieceposition).GetMoves(board);

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

                        Tuple<float, Position, List<Position>> mm = Minimax(Depth - 1, false, testboard);

                        // Change the best result if we need to
                        if (mm.Item1 >= BestValue)
                        {
                            BestValue = mm.Item1;
                            BestMoveset = moveset;
                            BestPiecePosition = pieceposition;
                        }

                    }
                }
                return Tuple.Create(BestValue, BestPiecePosition, BestMoveset);
            }
            else
            {
                foreach (Position pieceposition in usablepieces)
                {
                    // generate all possible movesets
                    List<List<Position>> possibleMovesets = board.GetPiece(pieceposition).GetMoves(board);

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

                        Tuple<float, Position, List<Position>> mm = Minimax(Depth - 1, true, testboard);

                        // Change the best result if we need to
                        if (mm.Item1 <= BestValue)
                        {
                            BestValue = mm.Item1;
                            BestMoveset = moveset;
                            BestPiecePosition = pieceposition;
                        }

                    }
                }
                return Tuple.Create(BestValue, BestPiecePosition, BestMoveset);
            }
        }

    }

}