using System.Diagnostics;

namespace Game
{
    public class MoveSearcher
    {
        delegate (int y, int x) SearchDirectionEvent(int y, int x, int delta);

        private readonly Board board;
        private readonly AlignChecker alignChecker;
        private readonly SearchDirectionEvent[] directions;
        private readonly (int y, int x)[] searchPositions;

        public MoveSearcher(Board board)
        {
            this.board = board;
            alignChecker = new AlignChecker(board);
            searchPositions = Evaluation.ProducePositions(board.Size).ToArray();

            //探索方向を登録
            directions = new SearchDirectionEvent[]
            {
                (y, x, delta) => (y, x + delta),
                (y, x, delta) => (y, x - delta),
                (y, x, delta) => (y + delta, x),
                (y, x, delta) => (y - delta, x),
                (y, x, delta) => (y + delta, x + delta),
                (y, x, delta) => (y - delta, x + delta),
                (y, x, delta) => (y - delta, x - delta),
                (y, x, delta) => (y + delta, x - delta),
            };
        }

        private List<long> secs = new List<long>();

        /// <summary>
        /// 最適な手を返します。
        /// </summary>
        /// <returns>手の座標</returns>
        public (int y, int x) Search()
        {
            Stopwatch sw = Stopwatch.StartNew();
            SearchResult result = AlphaBetaSearch((0, 0), 3, int.MinValue, int.MaxValue, true);
            sw.Stop();

            secs.Add(sw.ElapsedMilliseconds);
            Console.WriteLine($"ave = {secs.Average()}");

            return result.Position;
        }

        //alpha-beta法で探索を行う関数
        SearchResult AlphaBetaSearch((int y, int x) setPos, int depth, int alpha, int beta, bool isMaxTurn)
        {
            if (depth == 0)
            {
                int score = Evaluate();
                return new SearchResult(score, setPos);
            }

            SearchResult best = new SearchResult(isMaxTurn ? int.MinValue : int.MaxValue, (0, 0));

            foreach ((int y, int x) p in searchPositions.AsSpan())
            {
                //すでに設置されていたら探索しない
                if (board.GetUnsafe(p.y, p.x) != Stone.None)
                    continue;

                board.Set(p.y, p.x, isMaxTurn ? Stone.Black : Stone.White);

                //揃っていたら探索終了
                if (alignChecker.IsAlign(p.y, p.x, 5))
                {
                    board.Set(p.y, p.x, Stone.None);
                    return new SearchResult(isMaxTurn ? int.MinValue : int.MaxValue, p);
                }

                SearchResult result = AlphaBetaSearch(p, depth - 1, alpha, beta, !isMaxTurn);
                board.Set(p.y, p.x, Stone.None);

                if (isMaxTurn)
                {
                    //βカット
                    if (beta <= result.Score)
                        return new SearchResult(result.Score, p);

                    if (result.Score > best.Score)
                    {
                        best = new SearchResult(result.Score, p);
                        alpha = result.Score;
                    }
                } else
                {
                    //αカット
                    if (alpha >= result.Score)
                        return new SearchResult(result.Score, p);

                    if (result.Score < best.Score)
                    {
                        best = new SearchResult(result.Score, p);
                        beta = result.Score;
                    }
                }
            }

            return best;
        }

        //評価関数
        int Evaluate()
        {
            int bestScore = 0;

            Parallel.ForEach(searchPositions, p => {
                //指定位置から全方向への設置パターンを探索する
                int score = EvaluateLinks(p.y, p.x);

                bestScore += score;
            });

            return bestScore;
        }

        int EvaluateLinks(int y, int x)
        {
            int totalScore = 0;

            foreach (SearchDirectionEvent dir in directions.AsSpan())
            {
                foreach (Link link in Evaluation.Links.AsSpan())
                {
                    //設置パターンが盤面外に出るようだったら飛ばす
                    if (!IsValidRange(y, x, link.Pattern.Length, dir))
                        continue;

                    //パターンを満たしていなかったら飛ばす
                    if (!HasPattern(y, x, link.Pattern, dir))
                        continue;

                    totalScore += link.Score;
                }
            }

            return totalScore;
        }

        bool HasPattern(int y, int x, byte[] pattern, SearchDirectionEvent searchEvent)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                (int y, int x) pos = searchEvent(y, x, i);
                bool isDifferent = pattern[i] != (byte)board.GetUnsafe(pos.y, pos.x);

                if (isDifferent)
                    return false;
            }

            return true;
        }

        bool IsValidRange(int y, int x, int length, SearchDirectionEvent searchEvent)
        {
            (int y, int x) last = searchEvent(y, x, length - 1);
            return board.IsValidRange(last.y, last.x);
        }

        private readonly struct SearchResult
        {
            public readonly int Score;
            public readonly (int y, int x) Position;

            public SearchResult(int score, (int y, int x) position)
            {
                Score = score;
                Position = position;
            }
        }
    }
}