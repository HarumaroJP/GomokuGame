using System.Text.Json.Serialization;

namespace Game
{
    public class Evaluation
    {
        static readonly (int y, int x)[] directions = { (-1, 0), (0, -1), (1, 0), (0, 1) };

        public static readonly int AlignScore = 1000000;

        public static readonly Link[] Links = new[]
        {
            Link.Define(new[] { 1, 2, 2, 2, 2, 1 }, 100),
            Link.Define(new[] { 3, 2, 2, 2, 2, 1 }, 60),
            Link.Define(new[] { 1, 2, 2, 2, 2, 3 }, 60),
            Link.Define(new[] { 1, 2, 2, 2, 1 }, 80),
            Link.Define(new[] { 3, 2, 2, 2, 1 }, 50),
            Link.Define(new[] { 1, 2, 2, 2, 3 }, 50),
            Link.Define(new[] { 1, 2, 2, 1 }, 50),
            Link.Define(new[] { 3, 2, 2, 1 }, 30),
            Link.Define(new[] { 1, 2, 2, 3 }, 30),
        };

        public static IEnumerable<(int y, int x)> ProducePositions(int size)
        {
            int y = (int)Math.Ceiling(size / 2f);
            int x = (int)Math.Ceiling(size / 2f);

            (int y, int x) dir = directions[0];
            int current = 0;
            int corner = 1;
            int dirIdx = 0;

            for (int i = 0; i < size * size; i++)
            {
                Console.WriteLine((y, x));
                yield return (y, x);

                if (current >= corner)
                {
                    if (dirIdx % 2 == 1)
                    {
                        corner++;
                    }

                    dir = directions[++dirIdx % directions.Length];
                    current = 0;
                }

                y += dir.y;
                x += dir.x;
                current++;
            }
        }
    }

    public readonly struct Link
    {
        public readonly int[] Pattern;
        public readonly int Score;

        public static Link Define(int[] pattern, int score)
        {
            return new Link(pattern, score);
        }

        Link(int[] pattern, int score)
        {
            Pattern = pattern;
            Score = score;
        }
    }
}