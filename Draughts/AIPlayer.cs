﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

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
        public Board MakeMove(Board board, BackgroundWorker worker)
        {
            Tuple<float, Position, List<Position>> mm = Minimax(DepthOfSearch, IsWhite, board, worker);

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
        private Tuple<float, Position, List<Position>> Minimax(int Depth, bool MaximisingPlayer, Board board, BackgroundWorker worker)
        {
            // Setup 
            float BestValue;
            bool FoundTakeMove = false;
            List<List<Position>> possibleMovesets;

            Position BestPiecePosition = new Position(-1, -1);
            List<Position> usablepieces = new List<Position>();
            if (MaximisingPlayer)
            {
                // Start low, aim high
                BestValue = float.MinValue;
                usablepieces = board.GetWhitePositions();
            }
            else
            {
                // Start High, aim low
                BestValue = float.MaxValue;
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
                // Counts the number of pieces we have searched
                int piececount = 0;

                foreach (Position pieceposition in usablepieces)
                {
                    // generate all possible movesets
                    // testing prioritisation of take moves
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

                        Tuple<float, Position, List<Position>> mm = Minimax(Depth - 1, false, testboard, worker);

                        // Change the best result if we need to
                        if (mm.Item1 >= BestValue)
                        {
                            BestValue = mm.Item1;
                            BestMoveset = moveset;
                            BestPiecePosition = pieceposition;
                        }

                    }
                    // If its the first call (highest depth) and we have finished a piece, report back to the worker our progress 
                    if (Depth == DepthOfSearch)
                    {
                        worker.ReportProgress((int)(100f * piececount) / usablepieces.Count);
                    }
                    piececount++;
                }
                return Tuple.Create(BestValue, BestPiecePosition, BestMoveset);
            }
            else
            {
                // Counts the number of pieces we have searched
                int piececount = 0;

                foreach (Position pieceposition in usablepieces)
                {
                    // generate all possible movesets
                    // testing prioritisation of take moves                   
                    possibleMovesets = board.GetPiece(pieceposition).GetTakeMovesOnly(board);
                    if (possibleMovesets.Count > 0)
                    {
                        if (!FoundTakeMove)
                        {
                            // mark it so that every move is better that this
                            BestValue = float.MaxValue;
                            FoundTakeMove = true;
                        }
                    }
                    else if (!FoundTakeMove)
                    {
                        possibleMovesets = board.GetPiece(pieceposition).GetMoves(board);
                    }

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

                        Tuple<float, Position, List<Position>> mm = Minimax(Depth - 1, true, testboard, worker);

                        // Change the best result if we need to
                        if (mm.Item1 <= BestValue)
                        {
                            BestValue = mm.Item1;
                            BestMoveset = moveset;
                            BestPiecePosition = pieceposition;
                        }
                    }

                    // If its the first call (highest depth) and we have finished a piece, report back to the worker our progress 
                    if (Depth == DepthOfSearch)
                    {
                        worker.ReportProgress((int)(100f * ++piececount) / usablepieces.Count);
                    }
                }
                return Tuple.Create(BestValue, BestPiecePosition, BestMoveset);
            }
        }
        
        void ShowBoard(Board b, int depth)
        {
            for (int po = 0; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
            Console.WriteLine("#|0_1_2_3_4_5_6_7");
            for (int po = 0; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
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
                    for (int po = 0; po < (DepthOfSearch - depth); po++) { Console.Write("\t"); }
                    Console.Write((i / 8) + "|");
                }
            }
            Console.WriteLine();
            Console.WriteLine();

        }
    }

}