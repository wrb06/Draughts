using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class TextBasedGame
    {
        Random rng = new Random();
        static Board b = new Board(); // REMOVE TEST
       
        static void Main(string[] args)
        {

            //test
            ShowBoard();
            Console.WriteLine();

            AIPlayer white = new AIPlayer(true, 2);            
            AIPlayer black = new AIPlayer(false, 2);



            
            for (int i = 0; i < 50; i++)
            {
                b = black.MakeMove(b);
                ShowBoard();
                Console.WriteLine(b.EvaluateBoard());
                Console.WriteLine("black moved");
                Console.WriteLine();
                System.Threading.Thread.Sleep(3000);

                int a, e, c, d;
                a = int.Parse(Console.ReadLine());
                e = int.Parse(Console.ReadLine());
                c = int.Parse(Console.ReadLine());
                d = int.Parse(Console.ReadLine());

                b.MovePeice(new Position(a, e), new Position(c, d));
                Console.WriteLine(b.EvaluateBoard());
                Console.WriteLine("white moved");
                Console.WriteLine();


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
