using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Draughts;

namespace DraughtsGUI
{
    public partial class GUI : Form
    {
        const string WhitePieceLocation = "../../WhitePiece.png";
        const string BlackPieceLocation = "../../BlackPiece.png";
        const string WhiteKingPieceLocation = "../../WhiteKingPiece.png";
        const string BlackKingPieceLocation = "../../BlackKingPiece.png";
        const string EmptyPieceLocation1 = "../../EmptyPiece.png";
        const string EmptyPieceLocation2 = "../../EmptyPiece1.png";
        const string EmptyPieceHighlightLocation = "../../EmptyPieceHighlight.png";
        const string WhiteHighlightPieceLocation = "../../WhitePieceHighlight.png";



        private const int scale = 60;
        private bool MovedThisTurn = false;
        private bool TakeMoveMade = false;
        private Position TakingPiecePosition;
        private Position SelectedPosition;
        public bool FoundPieceToMove;

        Board board;
        List<PictureBox> boxes;

        AIPlayer AIBlack = new AIPlayer(false, 3);

        public GUI()
        {
            InitializeComponent();
            SetupBoard();

            /*
            board.PlacePeice(new Piece(true, 4, 7));
            board.PlacePeice(new Piece(true, 7, 6));

            board.PlacePeice(new Piece(false, 3, 6));
            board.PlacePeice(new Piece(false, 3, 4));
            board.PlacePeice(new Piece(false, 3, 2));
            board.PlacePeice(new Piece(false, 0, 6));
            */

            UpdateBoard();

            board = AIBlack.MakeMove(board);
            UpdateBoard();

        }

        private void SetupBoard(bool empty = false)
        {
            board = new Board(empty);
            boxes = new List<PictureBox>();
            int i = 0;
            foreach (Piece p in board.GetBoard())
            {
                PictureBox picture = new PictureBox();

                if (p == null){ picture.ImageLocation = (i % 2 + i / 8) % 2 == 0 ? EmptyPieceLocation1 : EmptyPieceLocation2; }
                else
                {
                    if (p.Value == 1) { picture.ImageLocation = WhitePieceLocation; }
                    if (p.Value == 500) { picture.ImageLocation = WhiteKingPieceLocation; }
                    if (p.Value == -1) { picture.ImageLocation = BlackPieceLocation; }
                    if (p.Value == -500) { picture.ImageLocation = BlackKingPieceLocation; }
                }
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Location = new Point((i % 8) * scale, (i / 8) * scale);
                picture.Size = new Size(scale, scale);

                picture.Click += Picture_Click;

                this.Controls.Add(picture);
                boxes.Add(picture);
                i++;
            }
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            Position ClickedPoint = PointToPosition(PointToClient(Cursor.Position));
            textBox3.Text = ClickedPoint.ToString();

            if (FoundPieceToMove)
            {
                if (!MovedThisTurn && board.IsLegalMove(SelectedPosition, ClickedPoint))
                {
                    // Select the empty spot and move
                    board.MovePeice(SelectedPosition, ClickedPoint);

                    // Redraw board, clearing highlights
                    UpdateBoard();

                    // Update Variables
                    FoundPieceToMove = false;
                    MovedThisTurn = true;
                    TakingPiecePosition = ClickedPoint;

                    // Update Display
                    textBox1.Text = board.EvaluateBoard().ToString();

                    // Update to show we have made a take move
                    if (SelectedPosition.IsTakeMove(ClickedPoint)) { TakeMoveMade = true; SelectedPosition = ClickedPoint; }

                    // auto end turn stuff
                    CheckForTurnEnd();
                    
                }
                else if (TakeMoveMade && SelectedPosition.Equals(TakingPiecePosition) && SelectedPosition.IsTakeMove(ClickedPoint))
                {
                    // Select the empty spot and move
                    board.MovePeice(SelectedPosition, ClickedPoint);

                    // Redraw board, clearing highlights
                    UpdateBoard();

                    // Update Variables
                    FoundPieceToMove = false;
                    MovedThisTurn = true;
                    SelectedPosition = ClickedPoint;
                    TakingPiecePosition = ClickedPoint;

                    CheckForTurnEnd();
                }
                else
                {
                    // Deselect the piece
                    textBox2.Text = "";
                    FoundPieceToMove = false;

                    // Redraw board, clearing highlights
                    UpdateBoard();
                }
            }
            else
            {
                // if the square isnt empty
                if (board.GetPiece(ClickedPoint) != null)
                {
                    // if its our piece
                    if (board.GetPiece(ClickedPoint).Value > 0)
                    {
                        FoundPieceToMove = true;
                        SelectedPosition = ClickedPoint;
                        textBox2.Text = SelectedPosition.ToString();

                        // Highlight this piece
                        UpdatePiece(ClickedPoint, WhiteHighlightPieceLocation);

                        // Highlight take moves
                        if (MovedThisTurn && TakeMoveMade)
                        {
                            // Highlight only take moves if we have moved allready
                            foreach (List<Position> move in board.GetPiece(ClickedPoint).GetTakeMovesOnly(board))
                            {
                                UpdatePiece(move.Last(), EmptyPieceHighlightLocation);
                            }
                        }
                        else if (!MovedThisTurn)
                        {
                            // Highlight all moves if this is the first move this turn
                            foreach (List<Position> move in board.GetPiece(ClickedPoint).GetMoves(board))
                            {
                                UpdatePiece(move.Last(), EmptyPieceHighlightLocation);
                            }
                        }
                    }
                }
            }

        }

        private void UpdateBoard()
        { 
            // For every square in the board
            for (int i = 0; i < 64; i++)
            {
                Piece p = board.GetBoard()[i / 8, i % 8];
                if (p == null)
                {   
                    // If the square is empty show the correct empty square picture
                    boxes[i].ImageLocation = (i % 2 + i / 8) % 2 == 0 ? EmptyPieceLocation1 : EmptyPieceLocation2;
                }
                else
                {
                    // Show the correct piece 
                    if (p.Value == 1) { boxes[i].ImageLocation = WhitePieceLocation; }
                    if (p.Value == 500) { boxes[i].ImageLocation = WhiteKingPieceLocation; }
                    if (p.Value == -1) { boxes[i].ImageLocation = BlackPieceLocation; }
                    if (p.Value == -500) { boxes[i].ImageLocation = BlackKingPieceLocation; }
                }
            }
        }

        private void UpdatePiece(Position position, string filelocation)
        {
            boxes[position.Y * 8 + position.X].ImageLocation = filelocation;
        }  
        private void UpdatePiece(int index, string filelocation)
        {
            boxes[index].ImageLocation = filelocation;
        }
        private void UpdatePiece(int x, int y, string filelocation)
        {
            boxes[y * 8 + x].ImageLocation = filelocation;
        }

        private Position PointToPosition(Point point)
        {
            return new Position(point.X / scale, point.Y / scale);
        }

        private void EndTurn_Click(object sender, EventArgs e)
        {
            board = AIBlack.MakeMove(board);
            UpdateBoard();
            textBox1.Text = board.EvaluateBoard().ToString();
            textBox2.Text = "";
            textBox3.Text = "";
            MovedThisTurn = false;
            TakeMoveMade = false;
        }

        private void GUI_Load(object sender, EventArgs e)
        {

        }

        private void CheckForTurnEnd()
        {
            // Auto ends turn if no more moves are found
            if ((MovedThisTurn && !TakeMoveMade) || (TakeMoveMade && board.GetPiece(SelectedPosition).GetTakeMovesOnly(board).Count == 0))
            {
                textBox1.Text = board.EvaluateBoard().ToString();
                textBox2.Text = "Calculating";
                textBox3.Text = "Calculating";
                Application.DoEvents();
                Endturn.PerformClick();
            }
        }
    }
}
