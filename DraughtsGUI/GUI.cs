using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
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
        const string WhitePieceHighlightLocation = "../../WhitePieceHighlight.png";
        const string WhiteKingPieceHighlightLocation = "../../WhiteKingPieceHighlight.png";

        private int scale;
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
            scale = FindScale();
            SetupBoard();

            UpdateBoard();
            board = AIBlack.MakeMove(board);
            UpdateBoard();

        }

        private int FindScale()
        {
            if (Height-140<Width) { return (Height-140) / 8; }
            else { return (Width-16) / 8; }
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
                picture.Location = new Point((i % 8) * scale, 100 + (i / 8) * scale);
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
                        if (board.GetPiece(ClickedPoint).Value == 1) { UpdatePiece(ClickedPoint, WhitePieceHighlightLocation); }
                        else { UpdatePiece(ClickedPoint, WhiteKingPieceHighlightLocation); }

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
            return new Position(point.X / scale, (point.Y-100) / scale);
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
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

                board = AIBlack.MakeMove(board);
                UpdateBoard();

                textBox1.Text = board.EvaluateBoard().ToString();
                textBox2.Text = "";
                textBox3.Text = "";

                MovedThisTurn = false;
                TakeMoveMade = false;
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            // Opens a new dialogue box
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save the board";
            saveFileDialog.Filter = "Board File|*.txt";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                // Convert the board into computer readable form
                string FileData = board.ConvertForSave();
                FileData += Environment.NewLine;


                // Add a human readable form so the user can see what state the board is in easily
                FileData += "#|0_1_2_3_4_5_6_7" + Environment.NewLine +
                            "0|";

                int i = 0;
                foreach (Piece p in board.GetBoard())
                {
                    i++;
                    if (p != null)
                    {
                        if (p.IsWhite)
                        {
                            if (p.GetType() == typeof(KingPiece)) { FileData += "W "; }
                            else { FileData += "w "; }
                        }
                        else
                        {
                            if (p.GetType() == typeof(KingPiece)) { FileData += "B "; }
                            else { FileData += "b "; }
                        }
                    }
                    else
                    {
                        FileData += ". ";
                    }

                    if (i % 8 == 0 && i != 64)
                    {
                        FileData += Environment.NewLine + (i / 8) + "|";
                    }
                }


                // Open the file
                FileStream file = (FileStream)saveFileDialog.OpenFile();

                // Convert to bytes
                byte[] FileBytes = Encoding.Default.GetBytes(FileData);


                file.Write(FileBytes, 0, FileBytes.Length);
                file.Close();
            }
        }

        private void LoadFile(object sender, EventArgs e)
        {
            // Open a new Dialogue box
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open a previous Board";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                // Read the file to bytes
                FileStream fileStream = (FileStream)openFileDialog.OpenFile();
                byte[] FileBytes = new byte[fileStream.Length];

                // Only take the first 64 characters as that is the board
                fileStream.Read(FileBytes, 0, 64);

                // Empty board, place new pieces on board
                board = new Board(true);
                int i = 0;
                foreach (byte b in FileBytes)
                {
                    if (b == (byte)'w') { board.PlacePeice(new Piece(true, i % 8, i / 8)); }
                    else if (b == (byte)'b') { board.PlacePeice(new Piece(false, i % 8, i / 8)); }
                    else if (b == (byte)'W') { board.PlacePeice(new KingPiece(true, i % 8, i / 8)); }
                    else if (b == (byte)'B') { board.PlacePeice(new KingPiece(false, i % 8, i / 8)); }

                    i++;
                }

                UpdateBoard();
            }
        }

        private void Resized(object sender, EventArgs e)
        {
            scale = FindScale();
            for (int i = 0; i<64; i++)
            {
                boxes[i].Location = new Point((i % 8) * scale, 100 + (i / 8) * scale);
                boxes[i].Size = new Size(scale, scale);
            }
            UpdateBoard();
            this.Invalidate();
        }

        private void ChangeDifficulty(object sender, EventArgs e)
        {
            AIBlack = new AIPlayer(false, trackBar1.Value/10 + 1);
            label4.Text = trackBar1.Value.ToString();
            //Console.WriteLine((trackBar1.Value / 10 + 1).ToString());
        }
    }
}
