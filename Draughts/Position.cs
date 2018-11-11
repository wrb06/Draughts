using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class Position
    {
        private readonly int _x;
        private readonly int _y;

        // properties
        public int X => _x;
        public int Y => _y;
        const int size = 8;

        public Position(int x_value, int y_value)
        {
            _x = x_value;
            _y = y_value;
        }

        // Returns true if the position is in the board
        public bool InBoard()
        {
            return ((X < size && X >= 0) && (Y < size && Y >= 0));
        }

        // Returns the position as a single number rather than XY cords
        public int GetAsSingleNum()
        {
            return Y * size + X;
        }

        // Returns the middle point between two positions
        public Position GetMiddlePosition(Position p)
        {
            return new Position(this.X + (p.X - this.X) / 2, this.Y + (p.Y - this.Y) / 2);
        }

        // Checks if the position could be a take move
        public bool IsTakeMove(Position to)
        {
            return (Math.Abs(this.X - to.X) == 2 && Math.Abs(this.Y - to.Y) == 2);
        }

        /* Each peice sees themselves as moving forward
         * RFT|__|__|__|LFT
         *  __|RF|__|LF|__
         *  __|__|W_|__|__
         *  __|RB|__|LB|__
         * RBT|__|__|__|LBT 
         */
        public Position GetRightForward(bool iswhite)
        {
            if (iswhite) { return new Position(X + 1, Y - 1); }
            else { return new Position(X + 1, Y + 1); }
        }
        public Position GetRightForwardTake(bool iswhite)
        {
            if (iswhite) { return new Position(X + 2, Y - 2); }
            else { return new Position(X + 2, Y + 2); }
        }
        public Position GetLeftForward(bool iswhite)
        {
            if (iswhite) { return new Position(X - 1, Y - 1); }
            else { return new Position(X - 1, Y + 1); }
        }
        public Position GetLeftForwardTake(bool iswhite)
        {
            if (iswhite) { return new Position(X - 2, Y - 2); }
            else { return new Position(X - 2, Y + 2); }
        }
        public Position GetRightBack(bool iswhite)
        {
            return GetRightForward(!iswhite);
        }
        public Position GetRightBackTake(bool iswhite)
        {
            return GetRightForwardTake(!iswhite);
        }
        public Position GetLeftBack(bool iswhite)
        {
            return GetLeftForward(!iswhite);
        }
        public Position GetLeftBackTake(bool iswhite)
        {
            return GetLeftForwardTake(!iswhite);
        }

        // Override functions
        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj.ToString() == this.ToString();
        }
    }

}
