﻿using System;
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
        // Set Image Constants
        // ---------------------------------------------------------------------------------------------------------
        const string WhitePieceLocation = "../../../Images/WhitePiece.png";
        const string BlackPieceLocation = "../../../Images/BlackPiece.png";
        const string WhiteKingPieceLocation = "../../../Images/WhiteKingPiece.png";
        const string BlackKingPieceLocation = "../../../Images/BlackKingPiece.png";

        const string EmptyImage1 = "../../../Images/EmptyPiece.png";
        const string EmptyImage2 = "../../../Images/EmptyPiece1.png";
        const string EmptyHighlightImage = "../../../Images/EmptyPieceHighlight.png";

        const string WhitePieceHighlightLocation = "../../../Images/WhitePieceHighlight.png";
        const string WhiteKingPieceHighlightLocation = "../../../Images/WhiteKingPieceHighlight.png";
        const string BlackPieceHighlightLocation = "../../../Images/BlackPieceHighlight.png";
        const string BlackKingPieceHighlightLocation = "../../BlackKingPieceHighlight.png";
        // -----------------------------------------------------------------------------------------------------------

        // Set Startup team;
        string YourImage = WhitePieceLocation;
        string YourKingImage = WhiteKingPieceLocation;
        string YourHighlightImage = WhitePieceHighlightLocation;
        string YourKingHighlightImage = WhiteKingPieceHighlightLocation;
        string EnemyImage = BlackPieceLocation;
        string EnemyKingImage = BlackKingPieceLocation;

        // Other Variables
        int scale;
        int MoveNum = 0;
        bool MovedThisTurn = false;
        bool TakeMoveMade = false;
        Position TakingPiecePosition;
        Position SelectedPosition;
        bool FoundPieceToMove;

        bool GameEnded = false;
        int CountSinceLastTake = 0;

        Board board;
        List<PictureBox> boxes;
        AIPlayer AIBlack = new AIPlayer(false, 3, false);
        bool PlayingAsWhite = true;

        BackgroundWorker worker;

        List<string> pastboards;

        public GUI()
        {
            InitializeComponent();

            pastboards = new List<string>();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += BlackAIMove;
            worker.ProgressChanged += BlackAIMoveProgress;
            worker.RunWorkerCompleted += BlackAIFinishedMove;


            scale = FindScale();
            SetupBoard();
            label9.Text = (40 - CountSinceLastTake).ToString();

            UpdateBoard();
            worker.RunWorkerAsync();

        }

        private void BlackAIMoveProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //Console.WriteLine(e.ProgressPercentage);
        }

        private void BlackAIFinishedMove(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveButton.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            trackBar1.Enabled = true;

            label7.Text = (++MoveNum).ToString();
            label9.Text = (40 - CountSinceLastTake).ToString();
            numericUpDown1.Value = MoveNum;
            Console.WriteLine("B " + board.ConvertForSave());
            pastboards.Add(board.ConvertForSave() + MoveNum.ToString("00"));

            MovedThisTurn = false;
            TakeMoveMade = false;

            if (board.BlackHasWon()) { DisplayEnd("Black");  }
            else if (board.WhiteHasWon()) { DisplayEnd("White"); }

            progressBar1.Value = 0;
        }

        private void BlackAIMove(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            List<Board> Boardstates = AIBlack.MakeMove(board, bgWorker, ref CountSinceLastTake);

            board = Boardstates.First();
            UpdateBoard();

            if (Boardstates.Count > 1)
            {
                foreach (Board state in Boardstates.Skip(1))
                {
                    // Sleeping the Backgroundworker doesnt affect UI
                    System.Threading.Thread.Sleep(500);

                    board = state;
                    UpdateBoard();
                }
            }
        }

        private int FindScale()
        {
            return (int)Math.Floor(Math.Min(ClientSize.Width, ClientSize.Height - 160) / 8.0);
        }
        private void SetupBoard(bool empty = false)
        {
            board = new Board(empty);

            Console.WriteLine("  " + board.ConvertForSave());
            pastboards.Add(board.ConvertForSave() + MoveNum.ToString("00"));

            boxes = new List<PictureBox>();
            int i = 0;
            foreach (Piece p in board.GetBoard())
            {
                PictureBox picture = new PictureBox();

                if (p == null){ picture.ImageLocation = (i % 2 + i / 8) % 2 == 0 ? EmptyImage1 : EmptyImage2; }
                else
                {
                    if (p.Value == 1) { picture.ImageLocation = YourImage; }
                    if (p.Value == 500) { picture.ImageLocation = YourKingImage; }
                    if (p.Value == -1) { picture.ImageLocation = EnemyImage; }
                    if (p.Value == -500) { picture.ImageLocation = EnemyKingImage; }
                }
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Location = new Point((i % 8) * scale, 160 + (i / 8) * scale);
                Console.WriteLine(((i % 8) * scale).ToString() + " " + (160 + (i / 8) * scale).ToString());
                picture.Size = new Size(scale, scale);

                picture.Click += CellClicked;

                this.Controls.Add(picture);
                boxes.Add(picture);
                i++;
            }
        }

        private void CellClicked(object sender, EventArgs e)
        {
            if (!GameEnded)
            {
                Position ClickedPoint = PointToPosition(PointToClient(Cursor.Position));

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
                        CountSinceLastTake++;

                        // Update to show we have made a take move
                        if (SelectedPosition.IsTakeMove(ClickedPoint))
                        {
                            CountSinceLastTake = 0;
                            TakeMoveMade = true;
                            SelectedPosition = ClickedPoint;
                        }

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

                            // Highlight this piece
                            if (board.GetPiece(ClickedPoint).Value == 1) { UpdatePiece(ClickedPoint, YourHighlightImage); }
                            else { UpdatePiece(ClickedPoint, YourKingHighlightImage); }

                            // Highlight take moves
                            if (MovedThisTurn && TakeMoveMade)
                            {
                                // Highlight only take moves if we have moved allready
                                foreach (List<Position> move in board.GetPiece(ClickedPoint).GetTakeMovesOnly(board))
                                {
                                    UpdatePiece(move.Last(), EmptyHighlightImage);
                                }
                            }
                            else if (!MovedThisTurn)
                            {
                                // Highlight all moves if this is the first move this turn
                                foreach (List<Position> move in board.GetPiece(ClickedPoint).GetMoves(board))
                                {
                                    UpdatePiece(move.Last(), EmptyHighlightImage);
                                }
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
                    boxes[i].ImageLocation = (i % 2 + i / 8) % 2 == 0 ? EmptyImage1 : EmptyImage2;
                }
                else
                {
                    // Show the correct piece 
                    if (p.Value == 1) { boxes[i].ImageLocation = YourImage; }
                    if (p.Value == 500) { boxes[i].ImageLocation = YourKingImage; }
                    if (p.Value == -1) { boxes[i].ImageLocation = EnemyImage; }
                    if (p.Value == -500) { boxes[i].ImageLocation = EnemyKingImage; }
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
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                return new Position((point.X - (ClientSize.Width - 8 * scale) / 2) / scale, (point.Y - 160) / scale);
            }
            else
            {

                return new Position(point.X / scale, (point.Y - 160) / scale);
            }
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
                if (board.BlackHasWon()) { DisplayEnd("Black"); }
                else if (board.WhiteHasWon()) { DisplayEnd("White"); }
                else
                {

                    // Add one to the move count and display
                    label9.Text = (40 - CountSinceLastTake).ToString();
                    label7.Text = (++MoveNum).ToString();
                    Console.WriteLine("W " + board.ConvertForSave());
                    pastboards.Add(board.ConvertForSave() + MoveNum.ToString("00"));
                    Application.DoEvents();

                    // Check for stalemate
                    if (CountSinceLastTake >= 40)
                    {
                        Console.WriteLine("STALEMATE");
                        DisplayEnd("No one");
                    }
                    else
                    {
                        // Let Black make a move and display           
                        SaveButton.Enabled = false;
                        button1.Enabled = false;
                        button2.Enabled = false;
                        trackBar1.Enabled = false;
                        worker.RunWorkerAsync();
                    }
                }

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
                string FileData = board.ConvertForSave() + MoveNum.ToString("000") + Environment.NewLine;

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
                FileData += "Moves Made so far: " + MoveNum.ToString();


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

                // Only take the first 35 characters as that is the board and Move number
                fileStream.Read(FileBytes, 0, 35);

                // Empty board, place new pieces on board
                board = new Board(true);

                // Read the first 32 characters and turn into pieces
                int ByteCount = 0;
                for (int i = 0; i < 64; i++)
                {
                    if ((i % 2 + i / 8) % 2 == 1)
                    {
                        byte b = FileBytes[ByteCount];

                        if (b == (byte)'w') { board.PlacePiece(new Piece(true, i % 8, i / 8)); }
                        else if (b == (byte)'b') { board.PlacePiece(new Piece(false, i % 8, i / 8)); }
                        else if (b == (byte)'W') { board.PlacePiece(new KingPiece(true, i % 8, i / 8)); }
                        else if (b == (byte)'B') { board.PlacePiece(new KingPiece(false, i % 8, i / 8)); }

                        ByteCount++;
                    }
                }

                // Read MoveNum from final two bytes
                int.TryParse(((char)FileBytes[32]).ToString() + ((char)FileBytes[33]).ToString() + ((char)FileBytes[34]).ToString(), out MoveNum);
                UpdateBoard();

                restartgame.Visible = false;
                restartgame.Location = new Point(0, 0);
                restartgame.Size = new Size(0, 0);
                
                MovedThisTurn = false;
                TakeMoveMade = false;
                GameEnded = false;

                label7.Text = MoveNum.ToString();

            }
        }

        private void Resized(object sender, EventArgs e)
        {
            scale = FindScale();
            for (int i = 0; i<64; i++)
            {
                if (ClientSize.Width > ClientSize.Height - 160)
                {
                    boxes[i].Location = new Point((ClientSize.Width - 8 * scale) / 2 + (i % 8) * scale, 160 + (i / 8) * scale);
                }
                else
                {
                    boxes[i].Location = new Point((i % 8) * scale, 160 + (i / 8) * scale);
                }
                boxes[i].Size = new Size(scale, scale);
            }
            UpdateBoard();
            // If the size of the board is height limited display the button in the centre of the screen
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                restartgame.Location = new Point((ClientSize.Width - 8 * scale) / 2 + 3 * scale, 160 + (int)(3.5 * scale));
            }
            else
            {
                restartgame.Location = new Point(3 * scale, 160 + (int)(3.5 * scale));
            }
            restartgame.Size = new Size(2 * scale, scale);
        }

        private void ChangeDifficulty(object sender, EventArgs e)
        {
            AIBlack = new AIPlayer(false, trackBar1.Value + 1, checkBox1.Checked);
            label4.Text = trackBar1.Value.ToString();
            //Console.WriteLine((trackBar1.Value / 10 + 1).ToString());
        }

        private void DisplayEnd(string Winner)
        {
            if (worker.IsBusy)
            {
                // Shouldnt ever run
                worker.CancelAsync();
                Console.WriteLine("Canceled worker"); 
            }

            GameEnded = true;
            restartgame.Text = Winner + " has won. \n Click to play again";
            // If the size of the board is height limited display the button in the centre of the screen
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                restartgame.Location = new Point((ClientSize.Width - 8 * scale) / 2 + 3 * scale, 160 + (int)(3.5 * scale));
            }
            else
            {
                restartgame.Location = new Point(3 * scale, 160 + (int)(3.5 * scale));
            }
            restartgame.Size = new Size(2*scale, scale);
            restartgame.Visible = true;

        }

        private void RestartGameClick(object sender, EventArgs e)
        {
            
            board = new Board();
            //Console.WriteLine("\n");
            //Console.WriteLine("  " + board.ConvertForSave());


            UpdateBoard();
            
            SaveButton.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            worker.RunWorkerAsync();

            MoveNum = 1;
            //Console.WriteLine("B " + board.ConvertForSave());
            UpdateBoard();

            
            restartgame.Visible = false;
            restartgame.Location = new Point(0, 0);
            restartgame.Size = new Size(0, 0);

            MovedThisTurn = false;
            TakeMoveMade = false;
            GameEnded = false;

            CountSinceLastTake = 0;
            label9.Text = CountSinceLastTake.ToString();
            
        }

        private void NewGame(object sender, EventArgs e)
        {
            board = new Board();
            //Console.WriteLine("\n");
            //Console.WriteLine("  " + board.ConvertForSave());

            UpdateBoard();
            
            SaveButton.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            worker.RunWorkerAsync();
            MoveNum = 0;

            label7.Text = MoveNum.ToString();
            numericUpDown1.Value = MoveNum;
            //Console.WriteLine("B " + board.ConvertForSave());
            UpdateBoard();


            restartgame.Visible = false;
            restartgame.Location = new Point(0, 0);
            restartgame.Size = new Size(0, 0);

            MovedThisTurn = false;
            TakeMoveMade = false;
            GameEnded = false;
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            // Step through debug mode
            
            bool changed = false;
            if (e.KeyCode == Keys.Left) { numericUpDown1.Value--; changed = true; }
            else if (e.KeyCode == Keys.Right) { numericUpDown1.Value++; changed = true; }

            if (changed)
            {
                board = new Board(true);
                // Load the board state with index stored
                int Charcount = 0;
                String Boardstate = pastboards[(int)numericUpDown1.Value];
                for (int i = 0; i < 64; i++)
                {
                    if ((i % 2 + i / 8) % 2 == 1)
                    {
                        char c = Boardstate[Charcount];

                        if (c == 'w') { board.PlacePiece(new Piece(true, i % 8, i / 8)); }
                        else if (c == 'b') { board.PlacePiece(new Piece(false, i % 8, i / 8)); }
                        else if (c == 'W') { board.PlacePiece(new KingPiece(true, i % 8, i / 8)); }
                        else if (c == 'B') { board.PlacePiece(new KingPiece(false, i % 8, i / 8)); }

                        Charcount++;
                    }
                }
                UpdateBoard();
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            AIBlack = new AIPlayer(false, trackBar1.Value / 10 + 1, checkBox1.Checked);
            Console.WriteLine("prune" + checkBox1.Checked.ToString());
        }

        private void SwitchTeamColor(object sender, EventArgs e)
        {
            // switch teams
            PlayingAsWhite = !PlayingAsWhite;

            if (PlayingAsWhite)
            {
                // Change images;
                YourImage = WhitePieceLocation;
                YourKingImage = WhiteKingPieceLocation;

                YourHighlightImage = WhitePieceHighlightLocation;
                YourKingHighlightImage = WhiteKingPieceHighlightLocation;

                EnemyImage = BlackPieceLocation;
                EnemyKingImage = BlackKingPieceLocation;

                label11.Text = "White";
            }
            else
            {
                // Change images;
                YourImage = BlackPieceLocation;
                YourKingImage = BlackKingPieceLocation;

                YourHighlightImage = BlackPieceHighlightLocation;
                YourKingHighlightImage = BlackKingPieceHighlightLocation;

                EnemyImage = WhitePieceLocation;
                EnemyKingImage = WhiteKingPieceLocation;
                label11.Text = "Black";
            }
            UpdateBoard();
        }
    }
}
