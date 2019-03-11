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

        public int X => _x;
        public int Y => _y;

        public Position(int x_value, int y_value)
        {
            _x = x_value;
            _y = y_value;
        }

        // Returns true if the position is in the board
        public bool InBoard()
        {
            return ((X < 8 && X >= 0) && (Y < 8 && Y >= 0));
        }

        // Returns the position as a single number rather than XY cords
        public int GetAsSingleNum()
        {
            return Y * 8 + X;
        }

        // Returns the middle point between two positions
        public Position GetMiddlePosition(Position p)
        {
            return new Position(this.X + (p.X - this.X) / 2, this.Y + (p.Y - this.Y) / 2);
        }

        // Checks if the position could be a take move
        public bool CouldBeTakeMove(Position to)
        {
            return (Math.Abs(this.X - to.X) == 2 && Math.Abs(this.Y - to.Y) == 2);
        }

        /* Each peice sees themselves as moving forward up the board
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

        // Convert the position to a string
        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }

        // Tests if two positions are the same
        public override bool Equals(object obj)
        {
            // Tests if the object doesnt exist or is a different type
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (obj.ToString() == this.ToString())
            {
                // Tests if the two objects are the same by checking if the string versions are the same
                return true;
            }
            else
            {
                return false;
            }
        }

        // Overrides object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
