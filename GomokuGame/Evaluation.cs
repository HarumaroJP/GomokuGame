namespace Game
{
    public class Evaluation
    {
        //評価パターン
        // 1: None
        // 2: White
        // 3: Black
        public static readonly Link[] Links = new[]
        {
            //守
            Link.Define(new byte[] { 1, 2, 2, 2, 2 }, -2000),
            Link.Define(new byte[] { 1, 2, 2, 1, 2 }, -800),
            Link.Define(new byte[] { 1, 2, 1, 2, 2 }, -800),
            Link.Define(new byte[] { 1, 2, 2, 2 }, -400),

            //攻
          //  Link.Define(new byte[] { 3, 3, 3, 3 }, 900),
          //  Link.Define(new byte[] { 3, 3, 3 }, 300),
          //  Link.Define(new byte[] { 3, 3 }, 120)
        };

        private static readonly (int y, int x)[] directions = { (-1, 0), (0, -1), (1, 0), (0, 1) };

        /// <summary>
        /// 螺旋状の座標列を返します
        /// </summary>
        /// <param name="size">1辺のサイズ</param>
        /// <returns>イテレータ</returns>
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
        public readonly byte[] Pattern;
        public readonly int Score;

        public static Link Define(byte[] pattern, int score)
        {
            return new Link(pattern, score);
        }

        Link(byte[] pattern, int score)
        {
            Pattern = pattern;
            Score = score;
        }
    }
}