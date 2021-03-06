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
        // ---------------------------------------------------------------------------------------------------------


        // Set Startup team;
        string YourImage = WhitePieceLocation;
        string YourKingImage = WhiteKingPieceLocation;
        string YourHighlightImage = WhitePieceHighlightLocation;
        string YourKingHighlightImage = WhiteKingPieceHighlightLocation;
        string EnemyImage = BlackPieceLocation;
        string EnemyKingImage = BlackKingPieceLocation;
        bool PlayingAsWhite = true;


        // Other Variables
        int BoardScale;
        int MoveNum = 0;
        bool MovedThisTurn = false;
        bool TakeMoveMade = false;
        Position TakingPiecePosition;
        Position SelectedPosition;
        bool FoundPieceToMove;

        bool GameEnded = false;
        int CountSinceLastTake = 0;

        // Setup the board
        Board GameBoard;
        List<PictureBox> Boxes;

        // Setup the AI
        AIPlayer AIBlack = new AIPlayer(false, 3, false);
       
        // Setup the background thread
        BackgroundWorker Worker;

        // Initialise the board and tell black to make the first move 
        public GUI()
        {
            InitializeComponent();


            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += BlackAIMove;
            Worker.ProgressChanged += BlackAIMoveProgress;
            Worker.RunWorkerCompleted += BlackAIFinishedMove;


            BoardScale = FindScale();
            SetupBoard();
            label9.Text = (40 - CountSinceLastTake).ToString();

            UpdateBoard();
            Worker.RunWorkerAsync();

        }

        // Runs whenever the worker is told we have made progress, updates the progressbar
        private void BlackAIMoveProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //Console.WriteLine(e.ProgressPercentage);
        }

        // AFter the move has been made, reenable the buttons and check if the game has ended
        private void BlackAIFinishedMove(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveButton.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            trackBar1.Enabled = true;

            label7.Text = (++MoveNum).ToString();
            label9.Text = (40 - CountSinceLastTake).ToString();
            //Console.WriteLine("B " + board.ConvertForSave());

            MovedThisTurn = false;
            TakeMoveMade = false;

            if (GameBoard.BlackHasWon()) { DisplayEnd("Black");  }
            else if (GameBoard.WhiteHasWon()) { DisplayEnd("White"); }

            progressBar1.Value = 0;
        }

        // Runs the search, then updates the board with what is returned. Does multistage moves in steps.
        private void BlackAIMove(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            List<Board> Boardstates = AIBlack.MakeMove(GameBoard, bgWorker, ref CountSinceLastTake);

            GameBoard = Boardstates.First();
            UpdateBoard();

            if (Boardstates.Count > 1)
            {
                foreach (Board state in Boardstates.Skip(1))
                {
                    // Sleeping the Backgroundworker doesnt affect UI
                    System.Threading.Thread.Sleep(500);

                    GameBoard = state;
                    UpdateBoard();
                }
            }
        }

        // Finds the largest one square in the board can be while still remaining inside the window
        private int FindScale()
        {
            return (int)Math.Floor(Math.Min(ClientSize.Width, ClientSize.Height - 160) / 8.0);
        }

        // Generates both backend and frontend board. Shows GUI board to the user
        private void SetupBoard(bool empty = false)
        {
            GameBoard = new Board(empty);

            // Console.WriteLine("  " + board.ConvertForSave());

            Boxes = new List<PictureBox>();
            int i = 0;
            foreach (Piece p in GameBoard.GetBoard())
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
                picture.Location = new Point((i % 8) * BoardScale, 160 + (i / 8) * BoardScale);
                picture.Size = new Size(BoardScale, BoardScale);

                picture.Click += CellClicked;

                this.Controls.Add(picture);
                Boxes.Add(picture);
                i++;
            }
        }

        // User interface when the user clicks on a square
        private void CellClicked(object sender, EventArgs e)
        {
            if (!GameEnded)
            {
                Position ClickedPosition = PointToPosition(PointToClient(Cursor.Position));

                if (FoundPieceToMove)
                {
                    // If the user has allready selected a move, and has just clicked another.

                    // Collect all takemoves
                    List<Position> TakeMoves = new List<Position>();
                    foreach (Position p in GameBoard.GetWhitePositions())
                    {
                        if (GameBoard.GetPiece(p).GetTakeMovesOnly(GameBoard).Count > 0)
                        { 
                            foreach (MoveSet moveset in GameBoard.GetPiece(p).GetTakeMovesOnly(GameBoard))
                            {
                                TakeMoves.Add(moveset.First());
                            }
                        }
                    }

                    if (!MovedThisTurn && GameBoard.IsLegalMove(SelectedPosition, ClickedPosition) && (TakeMoves.Count == 0 || TakeMoves.Contains(ClickedPosition)))
                    {
                        // Select the empty spot and move
                        GameBoard.MovePeice(SelectedPosition, ClickedPosition);

                        // Redraw board, clearing highlights
                        UpdateBoard();

                        // Update Variables
                        FoundPieceToMove = false;
                        MovedThisTurn = true;
                        TakingPiecePosition = ClickedPosition;
                        CountSinceLastTake++;

                        // Update to show we have made a take move
                        if (SelectedPosition.CouldBeTakeMove(ClickedPosition))
                        {
                            CountSinceLastTake = 0;
                            TakeMoveMade = true;
                            SelectedPosition = ClickedPosition;
                        }

                        // auto end turn stuff
                        CheckForTurnEnd();

                    }
                    else if (TakeMoveMade && SelectedPosition.Equals(TakingPiecePosition) && SelectedPosition.CouldBeTakeMove(ClickedPosition))
                    {
                        // Select the empty spot and move
                        GameBoard.MovePeice(SelectedPosition, ClickedPosition);

                        // Redraw board, clearing highlights
                        UpdateBoard();

                        // Update Variables
                        FoundPieceToMove = false;
                        MovedThisTurn = true;
                        SelectedPosition = ClickedPosition;
                        TakingPiecePosition = ClickedPosition;

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
                    // If this is the first click

                    // if the square isnt empty
                    if (GameBoard.GetPiece(ClickedPosition) != null)
                    {
                        // if its our piece
                        if (GameBoard.GetPiece(ClickedPosition).Value > 0)
                        {
                            FoundPieceToMove = true;
                            SelectedPosition = ClickedPosition;

                            // Highlight this piece
                            if (GameBoard.GetPiece(ClickedPosition).Value == 1) { UpdatePiece(ClickedPosition, YourHighlightImage); }
                            else { UpdatePiece(ClickedPosition, YourKingHighlightImage); }

                            // Highlight take moves
                            if (MovedThisTurn && TakeMoveMade)
                            {
                                // Highlight only take moves if we have moved allready
                                foreach (MoveSet move in GameBoard.GetPiece(ClickedPosition).GetTakeMovesOnly(GameBoard))
                                {
                                    UpdatePiece(move.Last(), EmptyHighlightImage);
                                }
                            }
                            else if (!MovedThisTurn)
                            {
                                // Collect all takemoves
                                List<Position> TakeMoves = new List<Position>();
                                foreach (Position p in GameBoard.GetWhitePositions())
                                {
                                    if (GameBoard.GetPiece(p).GetTakeMovesOnly(GameBoard).Count > 0)
                                    {
                                        TakeMoves.AddRange(GameBoard.GetPiece(p).GetTakeMovesOnly(GameBoard).First().Moves);
                                    }
                                }

                                
                                if (TakeMoves.Count > 0)
                                {
                                    // If there are take moves only highlight the take moves this piece can make
                                    foreach (MoveSet move in GameBoard.GetPiece(ClickedPosition).GetTakeMovesOnly(GameBoard))
                                    {
                                        UpdatePiece(move.Last(), EmptyHighlightImage);
                                    }
                                }
                                else
                                {

                                    // Highlight all moves if this is the first move this turn
                                    foreach (MoveSet move in GameBoard.GetPiece(ClickedPosition).GetMoves(GameBoard))
                                    {
                                        UpdatePiece(move.Moves.Last(), EmptyHighlightImage);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Updates every square in the board
        private void UpdateBoard()
        { 
            // For every square in the board
            for (int i = 0; i < 64; i++)
            {
                Piece p = GameBoard.GetBoard()[i / 8, i % 8];
                if (p == null)
                {   
                    // If the square is empty show the correct empty square picture
                    Boxes[i].ImageLocation = (i % 2 + i / 8) % 2 == 0 ? EmptyImage1 : EmptyImage2;
                }
                else
                {
                    // Show the correct piece 
                    if (p.Value == 1) { Boxes[i].ImageLocation = YourImage; }
                    if (p.Value == 500) { Boxes[i].ImageLocation = YourKingImage; }
                    if (p.Value == -1) { Boxes[i].ImageLocation = EnemyImage; }
                    if (p.Value == -500) { Boxes[i].ImageLocation = EnemyKingImage; }
                }
            }
        }

        // Updates a certain square with a new picture
        private void UpdatePiece(Position position, string filelocation)
        {
            Boxes[position.Y * 8 + position.X].ImageLocation = filelocation;
        }  
        private void UpdatePiece(int index, string filelocation)
        {
            Boxes[index].ImageLocation = filelocation;
        }
        private void UpdatePiece(int x, int y, string filelocation)
        {
            Boxes[y * 8 + x].ImageLocation = filelocation;
        }

        // Converts a pixel position into a board position
        private Position PointToPosition(Point point)
        {
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                return new Position((point.X - (ClientSize.Width - 8 * BoardScale) / 2) / BoardScale, (point.Y - 160) / BoardScale);
            }
            else
            {

                return new Position(point.X / BoardScale, (point.Y - 160) / BoardScale);
            }
        }

        // Runs when the GUI loads
        private void GUI_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        // Checks if the users turn has ended, if it has, updates the UI and starts searching for a new move
        private void CheckForTurnEnd()
        {
            // Auto ends turn if no more moves are found
            if ((MovedThisTurn && !TakeMoveMade) || (TakeMoveMade && GameBoard.GetPiece(SelectedPosition).GetTakeMovesOnly(GameBoard).Count == 0))
            {
                if (GameBoard.BlackHasWon()) { DisplayEnd("Black"); }
                else if (GameBoard.WhiteHasWon()) { DisplayEnd("White"); }
                else
                {

                    // Add one to the move count and display
                    label9.Text = (40 - CountSinceLastTake).ToString();
                    label7.Text = (++MoveNum).ToString();
                    //Console.WriteLine("W " + board.ConvertForSave());
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
                        Worker.RunWorkerAsync();
                    }
                }

            }
        }

        // Saves the current board
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
                string FileData = GameBoard.ConvertForSave() + MoveNum.ToString("000") + Environment.NewLine;

                // Add a human readable form so the user can see what state the board is in easily
                FileData += "#|0_1_2_3_4_5_6_7" + Environment.NewLine +
                            "0|";
                int i = 0;
                foreach (Piece p in GameBoard.GetBoard())
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

        // Loads a board
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

                if (FileBytes.Length < 35)
                {
                    MessageBox.Show("The file you have chosen is too small to be a board file");
                    return;
                }

                // Only take the first 35 characters as that is the board and Move number
                fileStream.Read(FileBytes, 0, 35);

                // Empty board, place new pieces on board
                
                Board newboard = new Board(true);

                // Read the first 32 characters and turn into pieces
                int ByteCount = 0;
                for (int i = 0; i < 64; i++)
                {
                    if ((i % 2 + i / 8) % 2 == 1)
                    {
                        byte b = FileBytes[ByteCount];

                        if (b == (byte)'w') { newboard.PlacePiece(new Piece(true, i % 8, i / 8)); }
                        else if (b == (byte)'b') { newboard.PlacePiece(new Piece(false, i % 8, i / 8)); }
                        else if (b == (byte)'W') { newboard.PlacePiece(new KingPiece(true, i % 8, i / 8)); }
                        else if (b == (byte)'B') { newboard.PlacePiece(new KingPiece(false, i % 8, i / 8)); }
                        else if (b == (byte)'E') { }
                        else
                        {
                            MessageBox.Show("The file you have chosen contains invalid pieces");
                            return;
                        }

                        ByteCount++;
                    }
                }

                // Read MoveNum from final three bytes
                try
                {
                    MoveNum = int.Parse(((char)FileBytes[32]).ToString() + ((char)FileBytes[33]).ToString() + ((char)FileBytes[34]).ToString());
                }
                catch
                {
                    MessageBox.Show("Error in loading the number of moves");
                    return;
                }
                
                

                this.GameBoard = newboard;
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

        // Whenever the board is resized, this moves and resizes the board to fit the new screen size
        private void Resized(object sender, EventArgs e)
        {
            BoardScale = FindScale();
            for (int i = 0; i<64; i++)
            {
                if (ClientSize.Width > ClientSize.Height - 160)
                {
                    Boxes[i].Location = new Point((ClientSize.Width - 8 * BoardScale) / 2 + (i % 8) * BoardScale, 160 + (i / 8) * BoardScale);
                }
                else
                {
                    Boxes[i].Location = new Point((i % 8) * BoardScale, 160 + (i / 8) * BoardScale);
                }
                Boxes[i].Size = new Size(BoardScale, BoardScale);
            }
            UpdateBoard();

            // If the size of the board is height limited display the button in the centre of the screen
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                restartgame.Location = new Point((ClientSize.Width - 8 * BoardScale) / 2 + 3 * BoardScale, 160 + (int)(3.5 * BoardScale));
            }
            else
            {
                restartgame.Location = new Point(3 * BoardScale, 160 + (int)(3.5 * BoardScale));
            }
            restartgame.Size = new Size(2 * BoardScale, BoardScale);
        }

        // When the difficulty slider is changed, this updates the AI
        private void ChangeDifficulty(object sender, EventArgs e)
        {
            AIBlack = new AIPlayer(false, trackBar1.Value + 1, checkBox1.Checked);
            label4.Text = trackBar1.Value.ToString();
            //Console.WriteLine((trackBar1.Value / 10 + 1).ToString());
        }

        // Displays the button to start a new game after the old game has finished
        private void DisplayEnd(string Winner)
        {
            if (Worker.IsBusy)
            {
                // Shouldnt ever run
                Worker.CancelAsync();
                Console.WriteLine("Canceled worker"); 
            }

            GameEnded = true;
            restartgame.Text = Winner + " has won. \n Click to play again";
            // If the size of the board is height limited display the button in the centre of the screen
            if (ClientSize.Width > ClientSize.Height - 160)
            {
                restartgame.Location = new Point((ClientSize.Width - 8 * BoardScale) / 2 + 3 * BoardScale, 160 + (int)(3.5 * BoardScale));
            }
            else
            {
                restartgame.Location = new Point(3 * BoardScale, 160 + (int)(3.5 * BoardScale));
            }
            restartgame.Size = new Size(2*BoardScale, BoardScale);
            restartgame.Visible = true;

        }

        // If the user wants to play again after finishing a game, this resets the board
        private void RestartGameClick(object sender, EventArgs e)
        {
            
            GameBoard = new Board();
            //Console.WriteLine("\n");
            //Console.WriteLine("  " + board.ConvertForSave());


            UpdateBoard();
            
            SaveButton.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            Worker.RunWorkerAsync();

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

        // Whenever the new game button is clicked, this resets the board and starts a new game.
        private void NewGame(object sender, EventArgs e)
        {
            GameBoard = new Board();
            //Console.WriteLine("\n");
            //Console.WriteLine("  " + board.ConvertForSave());

            UpdateBoard();
            
            SaveButton.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            Worker.RunWorkerAsync();
            MoveNum = 0;

            label7.Text = MoveNum.ToString();
            //Console.WriteLine("B " + board.ConvertForSave());
            UpdateBoard();


            restartgame.Visible = false;
            restartgame.Location = new Point(0, 0);
            restartgame.Size = new Size(0, 0);

            MovedThisTurn = false;
            TakeMoveMade = false;
            GameEnded = false;
        }

        // Button that toggles pruning
        private void TogglePruning(object sender, EventArgs e)
        {
            AIBlack = new AIPlayer(false, trackBar1.Value / 10 + 1, checkBox1.Checked);
            //Console.WriteLine("prune" + checkBox1.Checked.ToString());
        }

        // Switches which colour team you appear to play as
        private void SwitchTeamColor(object sender, EventArgs e)
        {
            // Disable the button
            button3.Enabled = false;

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

            // Apply the update
            UpdateBoard();

            // Enable the button again
            button3.Enabled = true;
        }
    }
}
