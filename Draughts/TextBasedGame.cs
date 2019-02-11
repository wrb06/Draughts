using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

//  This is to test the AI's without having to make a GUI.
namespace Draughts
{ 
    class TextBasedGame
    {
        Random rng = new Random();
        static Board b = new Board(true); // REMOVE TEST

        static private Board TestSetup()
        {
            Board b = new Board(true);
            b.PlacePiece(new Piece(false, 1, 0));
            b.PlacePiece(new Piece(false, 3, 0));
            b.PlacePiece(new Piece(false, 5, 0));
            b.PlacePiece(new Piece(false, 7, 0));
            b.PlacePiece(new Piece(false, 0, 1));
            b.PlacePiece(new Piece(false, 2, 1));
            b.PlacePiece(new Piece(false, 4, 1));
            b.PlacePiece(new Piece(false, 1, 2));
            b.PlacePiece(new Piece(false, 3, 2));
            b.PlacePiece(new Piece(false, 7, 2));
            b.PlacePiece(new Piece(false, 4, 3));
            b.PlacePiece(new Piece(true, 0, 5));
            b.PlacePiece(new Piece(true, 2, 5));
            b.PlacePiece(new Piece(true, 4, 5));
            b.PlacePiece(new Piece(true, 1, 6));
            b.PlacePiece(new Piece(true, 3, 6));
            b.PlacePiece(new Piece(true, 5, 6));
            b.PlacePiece(new Piece(true, 7, 6));
            b.PlacePiece(new Piece(true, 0, 7));
            b.PlacePiece(new Piece(true, 2, 7));
            b.PlacePiece(new Piece(true, 4, 7));
            b.PlacePiece(new Piece(true, 6, 7)); ;
            
            
            return b;
        }
 
        static void Main(string[] args)
        {
            // Setup
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            // Load
            b = TestSetup();
            ShowBoard();

            AIPlayer white = new AIPlayer(true, 2, !true, false);
            AIPlayer black = new AIPlayer(false, 4, !true, false);
            /*
            b = white.MakeMove(b, worker);
            ShowBoard();
            */
            for (int i = 0; i < 100; i++)
            {
                b = white.MakeMove(b, worker);
                ShowBoard();
                Console.WriteLine(b.EvaluateBoard());

                if (b.BlackHasWon()) { Console.WriteLine("black"); break; }
                if (b.WhiteHasWon()) { Console.WriteLine("white"); break; }

                b = black.MakeMove(b, worker);
                ShowBoard();
                Console.WriteLine(b.EvaluateBoard());

                if (b.BlackHasWon()) { Console.WriteLine("black"); break; }
                if (b.WhiteHasWon()) { Console.WriteLine("white"); break; }
            }
            
            





            Console.ReadLine();
        }

        static void ShowBoardNums()
        {
            int i = 0;
            foreach (Piece p in b.GetBoard())
            {

                if (p != null)
                {
                    if (p.IsWhite)
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("W "); }
                        else { Console.Write("w "); }
                    }
                    else
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("B "); }
                        else { Console.Write("b "); }
                    }
                }
                else
                {
                    Console.Write(Convert.ToString(i));
                    if (i < 10)
                    {
                        Console.Write("  ");
                    }
                    else { Console.Write(" "); }
                }



                if ((i+1) % 8 == 0)
                {
                    Console.WriteLine("");
                }
                i++;
            }
        }

        static void ShowBoard()
        {
            Console.WriteLine("#|0_1_2_3_4_5_6_7");
            Console.Write("0|");
            int i = 0;
            foreach (Piece p in b.GetBoard())
            {
                i++;
                if (p != null)
                {
                    if (p.IsWhite)
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("W "); }
                        else { Console.Write("w "); }
                    }
                    else
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("B "); }
                        else { Console.Write("b "); }
                    }
                }
                else
                {
                    Console.Write(". ");
                }



                if (i % 8 == 0 && i!=64)
                {
                    Console.WriteLine();
                    Console.Write((i/8) +"|");
                }
            }
            Console.WriteLine();

        }

        static void ShowBoardHighlight(List<Position> positions, string highlight = "# ")
        {
            int count = 0;
            foreach (Piece p in b.GetBoard())
            {
                if (p == null)
                {
                    bool highlighted = false;
                    foreach (Position pos in positions)
                    {
                        if (pos.GetAsSingleNum() == count) { Console.Write(highlight); highlighted = true; }
                    }
                    if (!highlighted)
                    {
                        Console.Write(". ");
                    }
                }
                else
                {
                    if (p.IsWhite)
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("W "); }
                        else { Console.Write("w "); }
                    }
                    else
                    {
                        if (p.GetType() == typeof(KingPiece)) { Console.Write("B "); }
                        else { Console.Write("b "); }
                    }
                }

                if ((count+1) % 8 == 0)
                {
                    Console.WriteLine("");
                }
                count++;

            }
            
        }

        static void ExplainMove(Position from, Position to)
        {
            Console.WriteLine("moved peice from: " + from.ToString() + " to position: " + to.ToString());
        }
    }
}
