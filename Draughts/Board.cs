using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    // Need to improve islegalmove funct
    class Board
    {
        private Piece[,] _boardArray = new Piece[8, 8];
        private int _numberOfPeices;

        // property to access the number of pieces
        public int NumberOfPeices  => _numberOfPeices; 
        // Constructor, if not empty will place peices in their starting positions
        public Board(bool empty = false)
        {
            _numberOfPeices = 0;
            if (!empty)
            {
                for (int i = 0; i < 8; i += 2)
                {
                    // Set White's peices up at the bottom
                    PlacePiece(new Piece(true, new Position(i, 7)));
                    PlacePiece(new Piece(true, new Position(i + 1, 6)));
                    PlacePiece(new Piece(true, new Position(i, 5)));

                    // Set Black's Peices at the top
                    PlacePiece(new Piece(false, new Position(i + 1, 0)));
                    PlacePiece(new Piece(false, new Position(i, 1)));
                    PlacePiece(new Piece(false, new Position(i + 1, 2)));
                }
            }
        }

        // Moves a piece in position from to position to
        public void MovePeice(Position from, Position to)
        {
            // If trying to move an empty scquare throw an error
            if (_boardArray[from.Y, from.X] == null) { throw new NullReferenceException(); }

            if (to.Y == 0 || to.Y == 7)
            {
                bool WasWhite = _boardArray[from.Y, from.X].IsWhite;

                _boardArray[to.Y, to.X] = new KingPiece(WasWhite, to.X, to.Y);
                _boardArray[to.Y, to.X].CurrentPosition = to;
            }
            else
            {
                // replace the piece in to with the piece in from
                _boardArray[to.Y, to.X] = _boardArray[from.Y, from.X];
                _boardArray[to.Y, to.X].CurrentPosition = to;
            }

            // If the move was a take move, remove the piece
            if (Math.Abs(from.Y - to.Y) == 2) { _boardArray[from.GetMiddlePosition(to).Y, from.GetMiddlePosition(to).X] = null; _numberOfPeices--; }

            // Clear the origional position
            _boardArray[from.Y, from.X] = null;

        }

        // Returns the piece in the position
        public Piece GetPiece(Position position)
        {
            return _boardArray[position.Y, position.X];
        }
        public Piece GetPiece(int x, int y)
        {
            return _boardArray[y, x];
        }

        // Adds the piece to the board
        public void PlacePiece(Piece piece)
        {
            _boardArray[piece.CurrentPosition.Y, piece.CurrentPosition.X] = piece;
            _numberOfPeices++;
        }

        // Removes a piece from the board
        public void RemovePeice(Piece piece)
        {
            _boardArray[piece.CurrentPosition.Y, piece.CurrentPosition.X] = null;
        }
        public void RemovePeice(Position position)
        {
            _boardArray[position.Y, position.X] = null;
        }

        // Gets a list of all positions that have white pieces
        public List<Position> GetWhitePositions()
        {
            List<Position> WhitePositions = new List<Position>();
            int i = 0;

            foreach (Piece p in _boardArray)
            {
                if (p != null)
                {
                    if (p.IsWhite) { WhitePositions.Add(new Position(i % 8, i / 8)); }
                }
                i++;
            }
            return WhitePositions;
        }

        // Gets a list of all positions that have black pieces
        public List<Position> GetBlackPositions()
        {
            List<Position> BlackPositions = new List<Position>();
            int i = 0;

            foreach (Piece p in _boardArray)
            {
                if (p != null)
                {
                    if (!p.IsWhite) { BlackPositions.Add(new Position(i % 8, i / 8)); }
                }
                i++;
            }
            return BlackPositions;
        }

        // Evaluates the board, more white is positive, more black is negative
        public float EvaluateBoard()
        {
            // Check if either player has won
            if (WhiteHasWon()) { return float.MaxValue; }
            if (BlackHasWon()) { return float.MinValue; }
            float BoardScore = 0.0f;
            int PieceValue;

            foreach (Position pos in GetWhitePositions())
            { 
                // Add up the values of the white pieces
                PieceValue = GetPiece(pos).Value;
                BoardScore += PieceValue;

                // If it's not a king, it also gains score by being further up the board, the maximum score for a normal piece is 37 (1 + 36 bonus)
                if (PieceValue == 1) { BoardScore += (7 - pos.Y) * (7 - pos.Y); }
            }
            foreach (Position pos in GetBlackPositions())
            {
                // Add up the values of the black pieces
                PieceValue = GetPiece(pos).Value;
                BoardScore += PieceValue;

                // If it's not a king, it also gains score by being down the board, the maximum score for a normal piece is 37
                if (PieceValue == -1) { BoardScore -= pos.Y * pos.Y; }
            }

            return BoardScore;
        }

        // Tests whether white has won
        public bool WhiteHasWon()
        {
            List<Position> BlackPositions = GetBlackPositions();
            if (BlackPositions.Count == 0) { return true; }
            else
            {
                foreach (Position PiecePosition in BlackPositions)
                {
                    if (GetPiece(PiecePosition).GetMoves(this).Count != 0) { return false; }
                }
                return true;
            }
        }

        // Tests whether black has won
        public bool BlackHasWon()
        {
            List<Position> WhitePositions = GetWhitePositions();
            if (WhitePositions.Count == 0) { return true; }
            else
            {
                foreach (Position PiecePosition in WhitePositions)
                {
                    if (GetPiece(PiecePosition).GetMoves(this).Count != 0) { return false; }
                }
                return true;
            }
        }

        // Creates a copy of the board
        public Board MakeNewCopyOf()
        {
            Board copy = new Board(true);

            foreach (Piece p in _boardArray)
            {
                if (p != null)
                {
                    if (p.GetType() == typeof(Piece))
                    {
                        copy.PlacePiece(new Piece(p.IsWhite, p.CurrentPosition));
                    }
                    else
                    {
                        copy.PlacePiece(new KingPiece(p.IsWhite, p.CurrentPosition));
                    }
                    
                }
            }

            return copy;
        }

        // Tests if it would be a legal move if the piece in from moved to the square to.
        public bool IsLegalMove(Position from, Position to)
        {
            if (!from.InBoard() || !to.InBoard()) { return false; }

            // test if the positionin is valid
            int i = to.GetAsSingleNum();
            if ((i % 2 + i / 8) % 2 == 0) { return false; }

            if (GetPiece(to) != null) { return false; }
   
            if (from.CouldBeTakeMove(to))
            {
                Piece middlepiece = this.GetPiece(from.GetMiddlePosition(to));

                // middlepiece has to be black
                if (middlepiece == null) { return false; }
                else if (middlepiece.IsWhite) { return false; }

                // EndPiece has to be null
                if (this.GetPiece(to) != null) { return false; }
            }

            if (GetPiece(from).Value == 1)
            {
                List<Position> PosMoves;

                if (from.CouldBeTakeMove(to))
                {                   
                    // if its a normal white piece and is taking
                    PosMoves = new List<Position>()
                    {
                    from.GetRightForwardTake(true), from.GetLeftForwardTake(true)
                    };
                }
                else
                {
                    // if its a normal white piece and is not taking
                    PosMoves = new List<Position>()
                    {
                    from.GetLeftForward(true), from.GetRightForward(true)
                    };
                };

                if (!PosMoves.Contains(to)) { return false; }
            }
            else
            {
                List<Position> PosMoves;

                if (from.CouldBeTakeMove(to))
                {
                    // if its a normal white piece and is taking
                    PosMoves = new List<Position>()
                    {
                    from.GetRightBackTake(true), from.GetLeftBackTake(true),
                    from.GetRightForwardTake(true), from.GetLeftForwardTake(true)
                    };
                }
                else
                {
                    // if its a normal white piece and is not taking
                    PosMoves = new List<Position>()
                    {
                    from.GetLeftForward(true), from.GetRightForward(true),
                    from.GetLeftBack(true), from.GetRightBack(true)
                    };
                };

                if (!PosMoves.Contains(to)) { return false; }
            }
            return true;
        }

        // Converts the board into a string of 32 characters in order to save the board
        public string ConvertForSave()
        {
            Piece p;
            string BoardString = "";
            for (int i = 0; i < 64; i++)
            {
                if ((i % 2 + i / 8) % 2 == 1) {
                    p = this.GetPiece(i % 8, i / 8);
                    if (p == null) { BoardString += "E"; }
                    else
                    {
                        if (p.Value == 1) { BoardString += "w"; }
                        else if (p.Value == -1) { BoardString += "b"; }
                        else if (p.Value == 500) { BoardString += "W"; }
                        else if (p.Value == -500) { BoardString += "B"; }
                        else { BoardString += "A"; }
                    } }
            }

            return BoardString;
        }

        // Returns the board array, used for debugging
        public Piece[,] GetBoard()
        {
            return _boardArray;
        }
    }
}
