using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class KingPiece : Piece
    {
        // Constructor
        public KingPiece(bool white, Position position) : base(white, position)
        {
            if (IsWhite) { _value = 500; }
            else { _value = -500; }
        }
        public KingPiece(bool white, int x, int y) : base(white, x, y)
        {
            if (IsWhite) { _value = 500; }
            else { _value = -500; }
        }

        // Returns all possible moves this peice can make
        public override List<MoveSet> GetMoves(Board board)
        {
            List<MoveSet> Moves = new List<MoveSet>();

            // Adds non take moves
            Moves.AddRange(GetNonTakeMoves(board));

            // Adds take moves
            Moves.AddRange(GetTakeMoves(board, CurrentPosition, IsWhite, new MoveSet()));

            return Moves;
        }

        // Non recursive checking the 4 moves around the piece
        public override List<MoveSet> GetNonTakeMoves(Board board)
        {
            List<MoveSet> Moves = new List<MoveSet>();

            // Adds backwards non take moves
            Position LB = this.CurrentPosition.GetLeftBack(IsWhite);
            Position RB = this.CurrentPosition.GetRightBack(IsWhite);
            Position LF = this.CurrentPosition.GetLeftForward(IsWhite);
            Position RF = this.CurrentPosition.GetRightForward(IsWhite);

            if (LF.InBoard())
            {
                if (board.GetPiece(LF) == null)
                {
                    MoveSet moveset = new MoveSet(LF);
                    Moves.Add(moveset);
                }
            }
            if (RF.InBoard())
            {
                if (board.GetPiece(RF) == null)
                {
                    MoveSet moveset = new MoveSet(RF);
                    Moves.Add(moveset);
                }
            }
            if (LB.InBoard())
            {
                if (board.GetPiece(LB) == null)
                {
                    MoveSet moveset = new MoveSet(LB);
                    Moves.Add(moveset);
                }
            }
            if (RB.InBoard())
            {
                if (board.GetPiece(RB) == null)
                {
                    MoveSet moveset = new MoveSet(RB);
                    Moves.Add(moveset);
                }
            }

            return Moves;
        }

        // recursive structure to find multi-step moves around the piece
        private List<MoveSet> GetTakeMoves(Board board, Position position, bool iswhite, MoveSet moveset)
        {
            List<MoveSet> Moves = new List<MoveSet>();

            Position LB = position.GetLeftBack(iswhite);
            Position LBT = position.GetLeftBackTake(IsWhite);

            Position RB = position.GetRightBack(IsWhite);
            Position RBT = position.GetRightBackTake(IsWhite);

            Position LF = position.GetLeftForward(iswhite);
            Position LFT = position.GetLeftForwardTake(IsWhite);

            Position RF = position.GetRightForward(IsWhite);
            Position RFT = position.GetRightForwardTake(IsWhite);

            MoveSet CurrentMove = new MoveSet(moveset);
            if (LB.InBoard() && LBT.InBoard())
            {
                if (board.GetPiece(LB) != null && board.GetPiece(LBT) == null)
                {
                    if (board.GetPiece(LB).IsWhite != this.IsWhite)
                    {
                        Board testboard = board.MakeNewCopyOf();
                        testboard.RemovePeice(testboard.GetPiece(LB));
                        CurrentMove.Add(LBT);
                        Moves.Add(CurrentMove);

                        moveset.Add(LBT);
                        Moves.AddRange(GetTakeMoves(testboard, LBT, IsWhite, moveset));
                        moveset.Remove(LBT);
                    }
                }
            }
            CurrentMove = new MoveSet(moveset);
            if (RB.InBoard() && RBT.InBoard())
            {
                if (board.GetPiece(RB) != null && board.GetPiece(RBT) == null)
                {
                    if (board.GetPiece(RB).IsWhite != this.IsWhite)
                    {
                        Board testboard = board.MakeNewCopyOf();
                        testboard.RemovePeice(testboard.GetPiece(RB));
                        CurrentMove.Add(RBT);
                        Moves.Add(CurrentMove);

                        moveset.Add(RBT);
                        Moves.AddRange(GetTakeMoves(testboard, RBT, IsWhite, moveset));

                        moveset.Remove(RBT);
                    }
                }
            }
            CurrentMove = new MoveSet(moveset);
            if (LF.InBoard() && LFT.InBoard())
            {
                if (board.GetPiece(LF) != null && board.GetPiece(LFT) == null)
                {
                    if (board.GetPiece(LF).IsWhite != this.IsWhite)
                    {
                        Board testboard = board.MakeNewCopyOf();
                        testboard.RemovePeice(testboard.GetPiece(LF));

                        CurrentMove.Add(LFT);
                        Moves.Add(CurrentMove);

                        moveset.Add(LFT);
                        Moves.AddRange(GetTakeMoves(testboard, LFT, IsWhite, moveset));
                        moveset.Remove(LFT);
                    }


                }
            }
            CurrentMove = new MoveSet(moveset);
            if (RF.InBoard() && RFT.InBoard())
            {
                if (board.GetPiece(RF) != null && board.GetPiece(RFT) == null)
                {
                    if (board.GetPiece(RF).IsWhite != this.IsWhite)
                    {
                        Board testboard = board.MakeNewCopyOf();
                        testboard.RemovePeice(testboard.GetPiece(RF));
                        CurrentMove.Add(RFT);
                        Moves.Add(CurrentMove);

                        moveset.Add(RFT);
                        Moves.AddRange(GetTakeMoves(testboard, RFT, IsWhite, moveset));
                        moveset.Remove(RFT);
                    }
                }
            }

            return Moves;
        }

        // public version of GetTakeMoves which only needs the board as an input
        public override List<MoveSet> GetTakeMovesOnly(Board board)
        {
            return GetTakeMoves(board, CurrentPosition, IsWhite, new MoveSet());
        }
    }
}
