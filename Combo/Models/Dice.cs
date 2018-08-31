using System;

namespace Combo.Models
{
    public class Dice
    {
        public int Sides { get; } = 6;
        public readonly Random _rand;

        public Dice(Random rand)
        {
            _rand = rand;
        }

        public Dice(Random rand, int sides) : this(rand)
        {
            Sides = sides;
        }

        public int Roll()
        {
            return _rand.Next(1, Sides + 1);
        }
    }
}
