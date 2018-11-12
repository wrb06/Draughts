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
        private Position MovedPosition;

        private Position MoveFrom;
        public bool FoundFirstMove;

        Board board;
        List<PictureBox> boxes;

        AIPlayer AIBlack = new AIPlayer(false, 5);

        public GUI()
        {
            InitializeComponent();
            SetupBoard();
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
            Position point1 = PointToPosition(PointToClient(Cursor.Position));
            textBox3.Text = point1.ToString();
            if (FoundFirstMove)
            {

                if (((MoveFrom == point1 && TakeMoveMade) || (!MovedThisTurn)) && board.IsLegalMove(MoveFrom, point1))
                {
                    // Move
                    board.MovePeice(MoveFrom, point1);
                    UpdateBoard();
                    
                    if (MoveFrom.IsTakeMove(point1)) { TakeMoveMade = true; }
                    FoundFirstMove = false;
                    MovedThisTurn = true;
                    MovedPosition = point1;
                    textBox1.Text = board.EvaluateBoard().ToString();

                    Application.DoEvents();
                    if ((MovedThisTurn && !TakeMoveMade) || (TakeMoveMade && board.GetPiece(MovedPosition).GetTakeMovesOnly(board).Count == 0))
                    {
                        textBox4.Text = "Calculating";
                        Application.DoEvents();
                        Endturn.PerformClick();
                        textBox4.Text = "";
                    }
                    
 
                }
                else
                {
                    // Deselect the piece
                    textBox2.Text = "";
                    FoundFirstMove = false;
                    UpdateBoard();

                }
            }
            else
            {
                if (board.GetPiece(point1) != null)
                {
                    // if its our piece
                    if (board.GetPiece(point1).Value > 0) 
                    {
                        if ((MovedPosition == point1 && TakeMoveMade) || !MovedThisTurn) 
                        FoundFirstMove = true;
                        MoveFrom = point1;
                        textBox2.Text = MoveFrom.ToString();
                        UpdatePiece(point1, WhiteHighlightPieceLocation);
                        if (MovedThisTurn && TakeMoveMade)
                        {
                            foreach (List<Position> move in board.GetPiece(point1).GetTakeMovesOnly(board))
                            {
                                UpdatePiece(move.Last(), EmptyPieceHighlightLocation);
                            }
                        }
                        else if (!MovedThisTurn)
                        {
                            foreach (List<Position> move in board.GetPiece(point1).GetMoves(board))
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
            MovedThisTurn = false;
            TakeMoveMade = false;
        }
    }
}
