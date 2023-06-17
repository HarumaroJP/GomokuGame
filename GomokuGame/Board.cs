using System.Text;

namespace Game
{
    /// <summary>
    /// 盤面情報の管理クラス
    /// </summary>
    public class Board : IReadOnlyBoard
    {
        public int Size { get; }

        private readonly Stone[][] fieldData;
        private readonly List<int> noneData;
        private readonly Random rand;

        public Board(int size)
        {
            this.Size = size;
            fieldData = Enumerable.Range(0, size).Select(_ => new Stone[size].Select(_ => Stone.None).ToArray()).ToArray();
            noneData = new List<int>(size);
            rand = new Random();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = i * size + j;
                    noneData.Add(index);
                }
            }
        }

        public bool IsValidRange(int y, int x)
        {
            return 0 <= x && x < Size && 0 <= y && y < Size;
        }

        public Stone Get(int y, int x)
        {
            if (!IsValidRange(y, x))
            {
                return Stone.Invalid;
            }

            return fieldData[y][x];
        }

        public Stone GetUnsafe(int y, int x)
        {
            return fieldData[y][x];
        }

        public void Set(int x, int y, Stone stone)
        {
            fieldData[y][x] = stone;

            int pos = y * Size + x;

            if (!noneData.Contains(pos))
                return;

            if (stone == Stone.None)
            {
                noneData.Add(pos);
            }
            else
            {
                noneData.Remove(pos);
            }
        }

        public (int y, int x) GetNonePosition()
        {
            int pos = noneData[rand.Next(0, noneData.Count)];

            return (pos / Size, pos % Size);
        }

        public (int black, int white) Sum()
        {
            int black = 0;
            int white = 0;

            foreach (Stone[] val in fieldData)
            {
                foreach (Stone stone in val)
                {
                    if (stone == Stone.White)
                    {
                        white++;
                    }
                    else if (stone == Stone.Black)
                    {
                        black++;
                    }
                }
            }

            return (black, white);
        }
    }
}