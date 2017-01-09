using System;
using System.Collections.Generic;

namespace QuantumCryptography
{
    public class Field
    {
        private readonly Dictionary<int, Dictionary<int, int>> field = new Dictionary<int, Dictionary<int, int>>();
        private readonly Dictionary<int, Tuple<int, int>> positions = new Dictionary<int, Tuple<int, int>>();
        public int MinX { get; private set; }
        public int MinY { get; private set; }
        public int MaxX {get; private set; }
        public int MaxY { get; private set; }

        public int this[int x, int y]
        {
            get
            {
                if (!field.ContainsKey(x))
                {
                    field[x] = new Dictionary<int, int>();
                }

                return field[x].ContainsKey(y) ? field[x][y] : 0; 
            }
            set
            {
                if (!field.ContainsKey(x))
                {
                    field[x] = new Dictionary<int, int>();
                }

                field[x][y] = value;
                MinX = Math.Min(MinX, x);
                MinY = Math.Min(MinY, y);
                MaxX = Math.Max(MaxX, x);
                MaxY = Math.Max(MaxY, y);

                positions[value] = Tuple.Create(x, y);
            }
        }

        public Tuple<int, int> GetPosition(int agent)
        {
            return positions[agent];
        }

        public void Print(int n)
        {
            var length = $"{n+1}".Length;

            for (var y = MaxY; y >= MinY; --y)
            {
                for (var x = MinX; x <= MaxX; ++x)
                {
                    if (this[x, y] == 0)
                    {
                        Console.Write("".PadLeft(length, ' '));
                    }
                    else
                    {
                        Console.Write($"{this[x, y]}".PadLeft(length, ' '));
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
