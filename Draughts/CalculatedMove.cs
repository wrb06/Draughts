using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    struct CalculatedMove
    {
        public float Value;
        public Position MoveFrom;
        public MoveSet Moveset;

        public CalculatedMove(float value, Position moveFrom, MoveSet moveset)
        {
            // Automatically does the negating
            Value = -value;
            MoveFrom = moveFrom;
            Moveset = moveset;
        }
    }
}
