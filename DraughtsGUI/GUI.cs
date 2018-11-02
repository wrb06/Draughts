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
        public GUI()
        {
            InitializeComponent();
            Board board = new Board();
            int i = 0;
            foreach (Piece p in board.GetBoard())
            {
                TextBox txt = new TextBox();

                txt.BackColor = (i % 2 + i / 8) % 2 == 0 ? Color.LightGray : Color.Gray;

                
                txt.Location = new Point((i%8)*scale, (i/8) * scale);
                txt.AutoSize = false;
                txt.Size = new Size(scale, scale);
                txt.Enabled = false;
                this.Controls.Add(txt);
                i++;
            }
        }

    }
}
