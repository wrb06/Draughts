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
            if (moveset.Count() > 0)
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

        public void Add(Position p)
        {
            _moves.Add(p);
        }
        public void Remove(Position p)
        {
            _moves.Remove(p);
        }
        public Position First()
        {
            return Moves.First();
        }
        public Position Last()
        {
            return Moves.Last();
        }
        public int Count()
        {
            return Moves.Count();
        }
        

        
    }
}
