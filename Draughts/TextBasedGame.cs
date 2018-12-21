using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  This is to test the AI's without having to make a GUI.
namespace Draughts
{
    class TextBasedGame
    {
        Random rng = new Random();
        static Board b = new Board(true); // REMOVE TEST
       
        static void Main(string[] args)
        {
            b.PlacePeice(new Piece(true, 6, 1));
            b.PlacePeice(new KingPiece(false, 7, 0));
            b.PlacePeice(new KingPiece(false, 1, 7));
            b.PlacePeice(new Piece(false, 4, 6));

            AIPlayer white = new AIPlayer(true, 2);            
            AIPlayer black = new AIPlayer(false, 2);

            ShowBoard();
            b = white.MakeMove(b);
            Console.WriteLine();

            ShowBoard();
            b = black.MakeMove(b);
            Console.WriteLine();

            ShowBoard();
            b = white.MakeMove(b);
            Console.WriteLine();

            ShowBoard();
            b = black.MakeMove(b);
            Console.WriteLine();
            /*
            for (int i = 0; i < 50; i++)
            {

                b = white.MakeMove(b);
                //ShowBoard();
                Console.WriteLine(b.EvaluateBoard());
                //Console.WriteLine("white moved");
                //Console.WriteLine();
                //Console.ReadLine();

                b = black.MakeMove(b);
                //ShowBoard();
                Console.WriteLine(b.EvaluateBoard());
                //Console.WriteLine("black moved");
                //Console.WriteLine();
                //Console.ReadLine();
            }
            */
            ShowBoard();

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
