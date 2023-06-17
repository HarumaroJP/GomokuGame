using System.Text;

namespace Game
{
    /// <summary>
    /// 盤面の表示クラス
    /// </summary>
    public class BoardWriter
    {
        private readonly Board board;
        private readonly StringBuilder builder;

        public BoardWriter(Board board)
        {
            this.board = board;
            builder = new StringBuilder();
        }

        public void Write()
        {
            int size = board.Size;

            builder.Append("    ");

            for (int i = 0; i < size; i++)
            {
                builder.Append($"{i + 1}   ");
            }

            builder.Append("\n  ");

            BuildParts(size, '┏', '┳', '┓');

            for (int i = 0; i < size; i++)
            {
                builder.Append($"{i + 1} ┃");
                for (int j = 0; j < size; j++)
                {
                    Stone v = board.Get(i, j);
                    builder.Append($" {GetStoneChar(v)} ");
                    builder.Append('┃');
                }

                builder.Append("\n  ");

                if (i < size - 1)
                {
                    BuildParts(size, '┣', '╋', '┫');
                }
            }

            BuildParts(size, '┗', '┻', '┛');

            Console.WriteLine(builder.ToString());
            builder.Clear();
        }

        void BuildParts(int w, char lSide, char mSide, char rSide)
        {
            builder.Append(lSide);
            for (int j = 0; j < w - 1; j++)
            {
                builder.Append($"━━━{mSide}");
            }

            builder.Append($"━━━{rSide}\n");
        }

        string GetStoneChar(Stone stone)
        {
            switch (stone)
            {
                case Stone.White:
                    return "●";
                case Stone.Black:
                    return "○";
                case Stone.None:
                    return " ";

                default:
                    return " ";
            }
        }
    }
}