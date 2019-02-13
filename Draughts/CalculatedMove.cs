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
        public List<Position> Moveset;

        public CalculatedMove(float value, Position moveFrom, List<Position> moveset)
        {
            // Automatically does the negating
            Value = -value;
            MoveFrom = moveFrom;
            Moveset = moveset;
        }
    }
}
