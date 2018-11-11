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
        private const int scale = 60;

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

                if (p == null){ picture.ImageLocation = (i % 2 + i / 8) % 2 == 0 ? "../../EmptyPiece.png" : "../../EmptyPiece1.png"; }
                else
                {
                    if (p.Value == 1) { picture.ImageLocation = "../../WhitePiece.png"; }
                    if (p.Value == 100) { picture.ImageLocation = "../../WhiteKingPiece.png"; }
                    if (p.Value == -1) { picture.ImageLocation = "../../BlackPiece.png"; }
                    if (p.Value == -100) { picture.ImageLocation = "../../WhiteKingPiece.png"; }
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

                if (MoveFrom != point1 && board.IsLegalMove(MoveFrom, point1))
                {
                    // Move
                    board.MovePeice(MoveFrom, point1);
                    FoundFirstMove = false;
                    UpdateBoard();
                    textBox1.Text = board.EvaluateBoard().ToString();
 
                }
                else
                {
                    // Deselect the piece
                    textBox2.Text = "";
                    FoundFirstMove = false;
                }
            }
            else
            {
                if (board.GetPiece(point1) != null)
                {
                    // if its our piece
                    if (board.GetPiece(point1).Value > 0)
                    {
                        FoundFirstMove = true;
                        MoveFrom = point1;
                        textBox2.Text = MoveFrom.ToString();
                    }
                }

            }
                
            
        }

        private void UpdateBoard()
        {
            
            for (int i = 0; i < 64; i++)
            {
                Piece p = board.GetBoard()[i / 8, i % 8];
                if (p == null)
                {
                    boxes[i].ImageLocation = (i % 2 + i / 8) % 2 == 0 ? "../../EmptyPiece.png" : "../../EmptyPiece1.png";
                }
                else
                { 
                    if (p.Value == 1) { boxes[i].ImageLocation = "../../WhitePiece.png"; }
                    if (p.Value == 100) { boxes[i].ImageLocation = "../../WhiteKingPiece.png"; }
                    if (p.Value == -1) { boxes[i].ImageLocation = "../../BlackPiece.png"; }
                    if (p.Value == -100) { boxes[i].ImageLocation = "../../BlackKingPiece.png"; }
                }

            }
            
        }

        private Position PointToPosition(Point point)
        {
            return new Position(point.X / scale, point.Y / scale);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            board = AIBlack.MakeMove(board);
            UpdateBoard();
            textBox1.Text = board.EvaluateBoard().ToString();
        }
    }
}
