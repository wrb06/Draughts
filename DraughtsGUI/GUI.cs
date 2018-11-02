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
        const int scale = 50;

        Board board;
        List<TextBox> boxes;

        public GUI()
        {
            InitializeComponent();

            SetupBoard();
            
        }

        private void SetupBoard(bool empty = false)
        {
            board = new Board(empty);
            boxes = new List<TextBox>();
            int i = 0;
            foreach (Piece p in board.GetBoard())
            {
                TextBox txt = new TextBox();

                txt.BackColor = (i % 2 + i / 8) % 2 == 0 ? Color.LightGray : Color.Gray;

                if (p != null)
                {
                    txt.ForeColor = (p.IsWhite) ? Color.White : Color.Black;
                    txt.Text = "O";
                }
                txt.Font = new Font(FontFamily.GenericSansSerif, scale / 1.5f);
                txt.Location = new Point((i % 8) * scale, (i / 8) * scale);
                txt.AutoSize = false;
                txt.Size = new Size(scale, scale);

                txt.TextAlign = HorizontalAlignment.Center;
                txt.ReadOnly = true;

                this.Controls.Add(txt);
                boxes.Add(txt);
                i++;
            }
        }


        private void GUI_Load(object sender, EventArgs e)
        {

        }
    }
}
