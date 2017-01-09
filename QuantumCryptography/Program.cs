using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuantumCryptography
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("You have to enter config file");
                Environment.Exit(0);
            }

            var configFile = args[0];
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Config file doesn't exist");
                Environment.Exit(0);
            }

            var streamReader = new StreamReader(configFile);
            var code = streamReader.ReadLine().Select(i => i == '0' ? 0 : 1).ToArray();

            var n = code.Length;

            while (true)
            {
                var random1 = new Random(Guid.NewGuid().GetHashCode());
                var field = new Field();
                var twists = new Stack<int>(Enumerable.Range(1, n).Select(x => random1.Next(1, 6)));
                field[0, 0] = 1;
                var current = Tuple.Create(0, 0);
                var reserve = 3;

                for (var i = 1; i <= n; ++i)
                {
                    var next = twists.Pop();
                    var codeValue = code[i - 1];
                    Tuple<int, int>[] points = null;
                    var minx = field.MinX - reserve;
                    var miny = field.MinY - reserve;
                    var maxx = field.MaxX + reserve;
                    var maxy = field.MaxY + reserve;

                    var point1 = Left(minx, miny, maxx, maxy, field, current, random1);
                    var point2 = LeftDiag(minx, miny, maxx, maxy, field, current, random1);
                    var point3 = Up(minx, miny, maxx, maxy, field, current, random1);
                    var point4 = RightDiag(minx, miny, maxx, maxy, field, current, random1);
                    var point5 = Right(minx, miny, maxx, maxy, field, current, random1);

                    switch (next)
                    {
                        case 1:
                        {
                            if (codeValue == 0)
                            {
                                points = new[] {point2, point3, point4, point5};
                            }
                            else
                            {
                                points = new[] {point1};
                            }
                            break;
                        }
                        case 2:
                        {
                            if (codeValue == 0)
                            {
                                points = new[] {point1, point3, point4, point5};
                            }
                            else
                            {
                                points = new[] {point2};
                            }
                            break;
                        }
                        case 3:
                        {
                            if (codeValue == 0)
                            {
                                points = new[] {point1, point2, point4, point5};
                            }
                            else
                            {
                                points = new[] {point3};
                            }
                            break;
                        }
                        case 4:
                        {
                            if (codeValue == 0)
                            {
                                points = new[] {point1, point2, point3, point5};
                            }
                            else
                            {
                                points = new[] {point4};
                            }
                            break;
                        }
                        case 5:
                        {
                            if (codeValue == 0)
                            {
                                points = new[] {point1, point2, point3, point4};
                            }
                            else
                            {
                                points = new[] {point5};
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }

                    var nextPoint = points[random1.Next(0, points.Length)];
                    field[nextPoint.Item1, nextPoint.Item2] = i + 1;
                    current = nextPoint;
                }

                var random2 = new Random(Guid.NewGuid().GetHashCode());
                var twists2 = new Stack<int>(Enumerable.Range(1, n).Select(x => random2.Next(1, 6)));
                var t2 = twists2.ToArray();

                var bits = new int[n];
                for (var i = 1; i <= n; ++i)
                {
                    var p1 = field.GetPosition(i);
                    var p2 = field.GetPosition(i + 1);
                    var direction = twists2.Pop();
                    bits[i - 1] = IsVisible(p1, p2, direction) ? 1 : 0;
                }

                var rightBits = new List<int>();
                var newCode = "";
                for (var i = 0; i < n; ++i)
                {
                    if (code[i] == bits[i])
                    {
                        rightBits.Add(i);
                        newCode += bits[i] == 0 ? "0" : "1";
                    }
                }

                if (newCode.Length > 0)
                {
                    field.Print(n);
                    foreach (var t in t2)
                    {
                        switch (t)
                        {
                            case 1:
                            {
                                Console.Write("Left ");
                                break;
                            }
                            case 2:
                            {
                                Console.Write("LeftUpDiagonal ");
                                break;
                            }
                            case 3:
                            {
                                Console.Write("Up ");
                                    break;
                            }
                            case 4:
                            {
                                Console.Write("RightUpDiagonal ");
                                break;
                            }
                            case 5:
                            {
                                Console.Write("Right ");
                                    break;
                            }
                            default:
                            {
                                throw new ArgumentException();
                            }
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Key = {newCode}");
                    break;
                }
                Console.WriteLine("Next iteration");
            }
        }

        private static bool IsVisible(Tuple<int, int> first, Tuple<int, int> second, int direction)
        {
            switch (direction)
            {
                case 1:
                {
                    if (first.Item2 != second.Item2)
                    {
                        return false;
                    }

                    if (second.Item1 < first.Item1)
                    {
                        return true;
                    }
                    return false;
                }
                case 2:
                {
                    if (second.Item1 < first.Item1 && second.Item2 > first.Item2)
                    {
                        return OnDiagonale(first, second);
                    }
                    return false;
                }
                case 3:
                {
                    if (first.Item1 != second.Item1)
                    {
                        return false;
                    }

                    if (second.Item2 > first.Item2)
                    {
                        return true;
                    }
                    return false;
                }
                case 4:
                {
                    if (second.Item1 > first.Item1 && second.Item2 > first.Item2)
                    {
                        return OnDiagonale(first, second);
                    }
                    return false;
                }
                case 5:
                {
                    if (first.Item2 != second.Item2)
                    {
                        return false;
                    }

                    if (second.Item1 > first.Item1)
                    {
                        return true;
                    }
                    return false;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        private static bool OnDiagonale(Tuple<int, int> first, Tuple<int, int> second)
        {
            var x = Math.Abs(first.Item1 - second.Item1);
            var y = Math.Abs(first.Item2 - second.Item2);
            return x == y;
        }

        private static Tuple<int, int> Left(int minx, int miny, int maxx, int maxy, Field field, Tuple<int, int> current,
            Random random)
        {
            while (true)
            {
                var add = random.Next(1, Math.Abs(minx - current.Item1) + 1);
                var res = Tuple.Create(current.Item1 - add, current.Item2);
                if (field[res.Item1, res.Item2] == 0)
                {
                    return res;
                }
            }
        }

        private static Tuple<int, int> Right(int minx, int miny, int maxx, int maxy, Field field,
            Tuple<int, int> current, Random random)
        {
            while (true)
            {
                var add = random.Next(1, Math.Abs(maxx - current.Item1) + 1);
                var res = Tuple.Create(current.Item1 + add, current.Item2);
                if (field[res.Item1, res.Item2] == 0)
                {
                    return res;
                }
            }
        }

        private static Tuple<int, int> Up(int minx, int miny, int maxx, int maxy, Field field, Tuple<int, int> current,
            Random random)
        {
            while (true)
            {
                var add = random.Next(1, Math.Abs(maxy - current.Item2) + 1);
                var res = Tuple.Create(current.Item1, current.Item2 + add);
                if (field[res.Item1, res.Item2] == 0)
                {
                    return res;
                }
            }
        }

        private static Tuple<int, int> LeftDiag(int minx, int miny, int maxx, int maxy, Field field,
            Tuple<int, int> current, Random random)
        {
            while (true)
            {
                var add1 = random.Next(1, Math.Abs(minx - current.Item1) + 1);
                var add2 = random.Next(1, Math.Abs(maxy - current.Item2) + 1);
                var add = Math.Min(add1, add2);

                var res = Tuple.Create(current.Item1 - add, current.Item2 + add);
                if (field[res.Item1, res.Item2] == 0)
                {
                    return res;
                }

                Console.WriteLine("yes");
            }
        }

        private static Tuple<int, int> RightDiag(int minx, int miny, int maxx, int maxy, Field field,
            Tuple<int, int> current, Random random)
        {
            while (true)
            {
                var add1 = random.Next(1, Math.Abs(maxx - current.Item1) + 1);
                var add2 = random.Next(1, Math.Abs(maxy - current.Item2) + 1);
                var add = Math.Min(add1, add2);

                var res = Tuple.Create(current.Item1 + add, current.Item2 + add);
                if (field[res.Item1, res.Item2] == 0)
                {
                    return res;
                }
            }
        }
    }
}