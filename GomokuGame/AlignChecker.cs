namespace Game
{
    /// <summary>
    /// 並びの判定クラス
    /// </summary>
    class AlignChecker
    {
        private readonly Board board;

        public AlignChecker(Board board)
        {
            this.board = board;
        }

        public bool IsAlign(int y, int x, int checkCount)
        {
            return IsAlignedLine(d => board.Get(y, x + d), checkCount) || //横
                   IsAlignedLine(d => board.Get(y + d, x), checkCount) || //縦
                   IsAlignedLine(d => board.Get(y + d, x + d), checkCount) || //右斜め
                   IsAlignedLine(d => board.Get(y - d, x + d), checkCount); //左斜め
        }

        bool IsAlignedLine(Func<int, Stone> stoneGetter, int checkCount)
        {
            Stone head = stoneGetter(0);
            int counter = 1;

            //基準点に何も設置されてなかったら終了
            if (head == Stone.None || head == Stone.Invalid)
            {
                return false;
            }

            //基準点から前後に並んでる数をカウント
            for (int i = 1; i < checkCount; i++)
            {
                Stone front = stoneGetter(i);

                if (front != head)
                    break;

                counter++;
            }

            for (int i = 1; i < checkCount; i++)
            {
                Stone back = stoneGetter(-i);

                if (back != head)
                    break;

                counter++;
            }

            //指定の数以上並んでいるか
            return counter >= checkCount;
        }
    }
}