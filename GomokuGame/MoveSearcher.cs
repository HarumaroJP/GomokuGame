namespace Game
{
    public class MoveSearcher
    {
        delegate (int y, int x) SearchDirectionEvent(int y, int x, int delta);

        private readonly Board board;
        private readonly AlignChecker alignChecker;
        private readonly int maxDepth = 1;

        private bool isPlayerTurn;
        private SearchDirectionEvent[] directions;
        private (int y, int x)[] searchPositions;
        private int[] searchBuffer;

        public MoveSearcher(Board board)
        {
            this.board = board;
            alignChecker = new AlignChecker(board);
            searchBuffer = new int[board.Size];
            searchPositions = Evaluation.ProducePositions(board.Size).ToArray();

            directions = new SearchDirectionEvent[]
            {
                (y, x, delta) => (y, x + delta),
                (y, x, delta) => (y + delta, x),
                (y, x, delta) => (y + delta, x + delta),
            };
        }

        public (int y, int x) Search()
        {
            int maxScore = 0;
            (int y, int x) bestPos = (0, 0);

            foreach ((int y, int x) pos in searchPositions)
            {
                int score = MiniMax(1, 0);

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPos = pos;
                }
            }

            Console.WriteLine($"bestPos = {bestPos}");

            return bestPos;
        }

        int MiniMax(int depth, int maxScore)
        {
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    Stone stone = board.GetUnsafe(i, j);

                    if (stone != Stone.None)
                        continue;

                    board.Set(i, j, Stone.Black);

                    int score = EvaluateLinks(i, j);

                    if (score > maxScore)
                    {
                        maxScore = score;
                    }

                    if (alignChecker.IsAlign(i, j, 5))
                    {
                        score += Evaluation.AlignScore;
                        maxScore = score;
                        continue;
                    }

                    board.Set(i, j, Stone.None);

                    if (depth > maxDepth)
                        continue;

                    int nextScore = MiniMax(depth + 1, maxScore);
                    if (nextScore > maxScore)
                    {
                        maxScore = score;
                    }
                }
            }

            return maxScore;
        }

        int EvaluateLinks(int y, int x)
        {
            int additionalScore = 0;
            Span<int> buffer = searchBuffer.AsSpan();

            foreach (Link link in Evaluation.Links)
            {
                foreach (SearchDirectionEvent dirEvent in directions)
                {
                    if (!IsValidRange(y, x, link.Pattern.Length, dirEvent))
                        continue;

                    Span<int> range = buffer.Slice(0, link.Pattern.Length);

                    GetRange(range, y, x, dirEvent);

                    if (!EqualLink(range, link.Pattern.AsSpan()))
                        continue;

                    additionalScore += link.Score;
                }
            }

            return additionalScore;
        }

        bool EqualLink(Span<int> a, Span<int> b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        bool IsValidRange(int y, int x, int length, SearchDirectionEvent searchEvent)
        {
            (int y, int x) last = searchEvent(y, x, length - 1);

            return board.IsValidRange(last.y, last.x);
        }

        void GetRange(Span<int> range, int y, int x, SearchDirectionEvent searchEvent)
        {
            for (int i = 0; i < range.Length; i++)
            {
                (int y, int x) pos = searchEvent(y, x, i);
                range[i] = (int)board.GetUnsafe(pos.y, pos.x);
            }
        }
    }
}