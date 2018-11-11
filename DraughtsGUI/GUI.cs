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
        const int scale = 60;

        Board board;
        List<PictureBox> boxes;
        AIPlayer AIBlack = new AIPlayer(false, 5);
        AIPlayer AIWhite = new AIPlayer(true, 1);
        bool turn = false;

        public GUI()
        {
            InitializeComponent();
            SetupBoard(); 
        }

        private void SetupBoard(bool empty = false)
        {
            board = new Board(empty);
            boxes = new List<PictureBox>();
            int i = 0;
            foreach (Piece p in board.GetBoard())
            {
                PictureBox picture = new PictureBox();
                
                if (p == null)
                {
                    picture.ImageLocation = (i % 2 + i / 8) % 2 == 0 ? "../../EmptyPiece.png" : "../../EmptyPiece1.png";
                }
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
 
               


                this.Controls.Add(picture);
                boxes.Add(picture);
                i++;
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

        private void GUI_Load(object sender, EventArgs e)
        {

        }

        private void EndTurn_Click(object sender, EventArgs e)
        {
            if (turn) { board = AIBlack.MakeMove(board); } else { board = AIWhite.MakeMove(board); }
            turn = !turn;
            UpdateBoard();
            textBox1.Text = board.EvaluateBoard().ToString();
        }
    }
}
