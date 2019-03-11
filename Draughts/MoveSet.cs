using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draughts;

namespace Draughts
{
    class MoveSet
    {
        private readonly List<Position> _moves;
        internal List<Position> Moves => _moves;

        // Constructors that generate a new moveset containing a list of positions for every stage in a move
        public MoveSet()
        {
            _moves = new List<Position>();
        }
        public MoveSet(List<Position> moveset)
        {
            _moves = moveset;
        }
        public MoveSet(MoveSet moveset)
        {
            _moves = new List<Position>();
            if (moveset.NumberOfStages() > 0)
            {
                foreach (Position p in moveset.Moves)
                {
                    _moves.Add(p);
                }
            }
        }
        public MoveSet(Position position)
        {
            _moves = new List<Position>
            {
                position
            };
        }

        // Adds something to the moveset
        public void Add(Position p)
        {
            _moves.Add(p);
        }

        // Removes from the moveset
        public void Remove(Position p)
        {
            _moves.Remove(p);
        }

        // Gets the first position in the moveset
        public Position First()
        {
            return Moves.First();
        }

        // Gets the last position in the moveset
        public Position Last()
        {
            return Moves.Last();
        }

        //Gets the number of stages in the moveset
        public int NumberOfStages()
        {
            return Moves.Count();
        }
        

        
    }
}
