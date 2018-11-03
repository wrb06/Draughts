using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    // Need to improve EvaluateBoard and GamEnded functs
    class Board
    {
        const int size = 8;
        private Piece[,] _boardArray = new Piece[size, size];
        private int _numberOfPeices;

        // property to access the number of pieces
        public int NumberOfPeices { get => _numberOfPeices; private set => _numberOfPeices = value; }
        
        // Constructor, if not empty will place peices in their starting positions
        public Board(bool empty = false)
        {
            NumberOfPeices = 0;
            if (!empty)
            {
                for (int i = 0; i < size; i += 2)
                {
                    // Set White's peices up at the bottom

                    PlacePeice(new Piece(true, new Position(i, 7)));
                    PlacePeice(new Piece(true, new Position(i + 1, 6)));
                    PlacePeice(new Piece(true, new Position(i, 5)));

                    // Set Black's Peices at the top
                    PlacePeice(new Piece(false, new Position(i + 1, 0)));
                    PlacePeice(new Piece(false, new Position(i, 1)));
                    PlacePeice(new Piece(false, new Position(i + 1, 2)));
                }
            }
        }

        // Returns the board so it can be displayed
        public Piece[,] GetBoard()
        {
            return _boardArray;
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
            if (Math.Abs(from.Y - to.Y) == 2) { _boardArray[from.GetMiddlePosition(to).Y, from.GetMiddlePosition(to).X] = null; NumberOfPeices--; }

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
        public void PlacePeice(Piece piece)
        {
            _boardArray[piece.CurrentPosition.Y, piece.CurrentPosition.X] = piece;
            NumberOfPeices++;
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

        // Gets a list of all white/black peices positions
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
            if (WhiteHasWon()) { return 9999f; }
            if (BlackHasWon()) { return -9999f; }

            float BoardScore = 0.0f;
            int PieceValue;

            foreach (Position pos in GetWhitePositions())
            {
                PieceValue = GetPiece(pos).Value;
                BoardScore += PieceValue;
                if (PieceValue == 1) { BoardScore += (7 - pos.Y) * (7 - pos.Y); }
            }
            foreach (Position pos in GetBlackPositions())
            {
                PieceValue = GetPiece(pos).Value;
                BoardScore += PieceValue;
                if (PieceValue == -1) { BoardScore -= pos.Y * pos.Y; }
            }

            //BoardScore /= NumberOfPeices;
            return BoardScore;
        }


        // Evaluates whether someone, returns 1 if white wins, -1 if black wins, 0 if neither side has won.
        public bool WhiteHasWon()
        {
            foreach (Position pieceposition in GetBlackPositions())
            {
                if (GetPiece(pieceposition).GetMoves(this).Count != 0) { return false; }
            }
            return GetBlackPositions().Count == 0;
        }
        public bool BlackHasWon()
        {
            bool OppoHasMoves = false;
            foreach (Position pieceposition in GetWhitePositions())
            {
                if (GetPiece(pieceposition).GetMoves(this).Count != 0) { OppoHasMoves = true; break; }
            }
            return GetWhitePositions().Count == 0 || !OppoHasMoves;
        }

        // creates a new version of the board
        public Board MakeNewCopyOf()
        {
            Board copy = new Board(true);
            foreach (Piece p in _boardArray)
            {
                if (p != null) { copy.PlacePeice(new Piece(p.IsWhite, p.CurrentPosition)); }
            }
            return copy;
        }
    }
}
