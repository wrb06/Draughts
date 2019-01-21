using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class Piece
    {
        // private variables
        protected int _value;
        protected readonly bool _isWhite;
        protected Position _currentPosition;

        // Properties to access private variables
        public int Value { get => _value; set => _value = value;  }
        public bool IsWhite => _isWhite;
        public Position CurrentPosition { get => _currentPosition; set => _currentPosition = value; }

        // Constructor
        public Piece(bool White, Position position)
        {
            _currentPosition = position;
            _isWhite = White;
            if (IsWhite) { _value = 1; }
            else { _value = -1; }
        }
        public Piece(bool White, int x, int y)
        {
            CurrentPosition = new Position(x, y);
            _isWhite = White;
            if (IsWhite) { _value = 1; }
            else { _value = -1; }
        }

        // Returns all possible movesets a piece can make, including ones with multiple steps
        public virtual List<List<Position>> GetMoves(Board board)
        {
            List<List<Position>> Moves = new List<List<Position>>();
         
            Moves.AddRange(GetNonTakeMoves(board));

            // Calls recursive function to find multi-step take moves
            Moves.AddRange(GetTakeMoves(board, CurrentPosition, IsWhite, new List<Position>()));

            return Moves;
        }        
           
        public virtual List<List<Position>> GetNonTakeMoves(Board board)
        {
            List<List<Position>> Moves = new List<List<Position>>();

            // Adds non take moves
            Position Left = this.CurrentPosition.GetLeftForward(IsWhite);
            Position Right = this.CurrentPosition.GetRightForward(IsWhite);
            if (Left.InBoard())
            {
                if (board.GetPiece(Left) == null)
                {
                    List<Position> moveset = new List<Position>
                    {
                        Left
                    };
                    Moves.Add(moveset);
                }
            }
            if (Right.InBoard())
            {
                if (board.GetPiece(Right) == null)
                {
                    List<Position> moveset = new List<Position>
                    {
                        Right
                    };
                    Moves.Add(moveset);
                }
            }

            return Moves;
        }

        // recursive structure to find multi-step moves
        private List<List<Position>> GetTakeMoves(Board board, Position position, bool iswhite, List<Position> moveset)
        {
            List<List<Position>> Moves = new List<List<Position>>();

            Position LF = position.GetLeftForward(iswhite);
            Position LFT = position.GetLeftForwardTake(IsWhite);

            Position RF = position.GetRightForward(IsWhite);
            Position RFT = position.GetRightForwardTake(IsWhite);

            List<Position> CurrentMove = new List<Position>(moveset);

            if (LF.InBoard() && LFT.InBoard())
            {
                if (board.GetPiece(LF) != null && board.GetPiece(LFT) == null)
                {
                    if (board.GetPiece(LF).IsWhite != this.IsWhite)
                    {
                        CurrentMove.Add(LFT);
                        Moves.Add(CurrentMove);

                        moveset.Add(LFT);
                        Moves.AddRange(GetTakeMoves(board, LFT, IsWhite, moveset));
                        moveset.Remove(LFT);
                    }


                }
            }
            CurrentMove = new List<Position>(moveset);
            if (RF.InBoard() && RFT.InBoard())
            {
                if (board.GetPiece(RF) != null && board.GetPiece(RFT) == null)
                {
                    if (board.GetPiece(RF).IsWhite != this.IsWhite)
                    {
                        CurrentMove.Add(RFT);
                        Moves.Add(CurrentMove);

                        moveset.Add(RFT);
                        Moves.AddRange(GetTakeMoves(board, RFT, IsWhite, moveset));
                        moveset.Remove(RFT);
                    }
                }
            }
            return Moves;
        }

        // public version of GetTakeMoves which only needs the board as an input
        public virtual List<List<Position>> GetTakeMovesOnly(Board board)
        {
            return GetTakeMoves(board, CurrentPosition, IsWhite, new List<Position>());
        }


    }
}
