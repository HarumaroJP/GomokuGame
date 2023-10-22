//MSゴシック以外のフォントで動かすと表示がおかしくなるかもしれないです。

namespace Game
{
    public enum Stone : byte
    {
        Invalid,
        None,
        White,
        Black
    }

    class Gomoku
    {
        private readonly Board board;
        private readonly AlignChecker alignChecker;
        private readonly MoveSearcher moveSearcher;
        private readonly BoardWriter boardWriter;
        private readonly List<string> inputHistory;

        private bool showHistory = false; //入力履歴を表示する

        public Gomoku()
        {
            board = new Board(8); //8x8の盤面を設定
            alignChecker = new AlignChecker(board);
            moveSearcher = new MoveSearcher(board);
            boardWriter = new BoardWriter(board);
            inputHistory = new List<string>();
        }

        public void Start()
        {
            Console.WriteLine("遊び方: ");

            CommandLoop();
        }

        private void Refresh()
        {
            //Console.Clear();
            boardWriter.Write();

            if (showHistory)
            {
                RenderHistory();
            }
        }

        void CommandLoop()
        {
            bool isPlayerTurn = true;
            (int y, int x) setPoint = (0, 0);

            while (true)
            {
                Refresh();

                //縦横斜めのいずれかが揃ったら終了
                if (alignChecker.IsAlign(setPoint.y, setPoint.x, 5))
                {
                    Console.WriteLine("コマが揃いました!");
                    Console.WriteLine($"{(isPlayerTurn ? "COM" : "プレイヤー")}の勝利です!");
                    return;
                }

                //全部埋まったら終了
                if (IsDraw())
                {
                    Console.WriteLine("引き分けです!");
                    return;
                }

                setPoint = isPlayerTurn ? TurnPlayer() : TurnCOM();

                isPlayerTurn = !isPlayerTurn;
            }
        }

        bool IsDraw()
        {
            (int black, int white) sum = board.Sum();
            return sum.black + sum.white == board.Size * board.Size;
        }

        (int y, int x) TurnPlayer()
        {
            Console.WriteLine("コマンドリスト");
            Console.WriteLine(" 縦のマス数 横のマス数 : 指定したマスにコマを配置します");
            Console.WriteLine(" end : 黒の石数と白の石数を表示して終了します。");
            Console.WriteLine(" exit : プログラムを終了します。");

            while (true)
            {
                Console.WriteLine("\nコマンドを入力してください。");

                string? input = Console.ReadLine();

                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("不正なコマンドです。");
                    continue;
                }

                string[] inputArgs = input.Split(' ');

                //exitコマンド
                if (inputArgs.Length == 1 && inputArgs[0] == "exit")
                {
                    Console.WriteLine("プログラムを終了します。");
                    Environment.Exit(0);
                }

                //endコマンド
                if (inputArgs.Length == 1 && inputArgs[0] == "end")
                {
                    (int black, int white) sum = board.Sum();
                    Console.WriteLine($"合計は、黒: {sum.black}, 白: {sum.white}です");
                    Environment.Exit(0);
                }

                //設置コマンド
                if (inputArgs.Length == 2)
                {
                    int inputX = 0;
                    int inputY = 0;

                    bool canParse = int.TryParse(inputArgs[0], out inputY) && int.TryParse(inputArgs[1], out inputX);

                    if (!canParse)
                    {
                        Console.WriteLine($"不正な値です。数値を入力してください。");
                        continue;
                    }

                    int fieldX = inputX - 1;
                    int fieldY = inputY - 1;

                    if (board.IsValidRange(fieldY, fieldX) && board.Get(fieldY, fieldX) == Stone.None)
                    {
                        board.Set(fieldY, fieldX, Stone.White); //盤面に石をセット
                        RecordInput("Player", fieldX, fieldY); //入力履歴を記録する
                        return (fieldY, fieldX);
                    } else
                    {
                        Console.WriteLine("置くことができないマスです");
                    }
                }
            }
        }

        (int y, int x) TurnCOM()
        {
            (int y, int x) nonePos = moveSearcher.Search(); //ランダムな空所を取得
            board.Set(nonePos.y, nonePos.x, Stone.Black); //盤面に石をセット
            RecordInput("COM", nonePos.x, nonePos.y); //入力履歴を記録する

            return nonePos;
        }

        void RecordInput(string author, int x, int y)
        {
            inputHistory.Add($"{inputHistory.Count + 1} {author}: 縦{y + 1}, 横{x + 1}");
        }

        void RenderHistory()
        {
            if (inputHistory.Count == 0)
                return;

            Console.WriteLine();
            foreach (string history in inputHistory)
            {
                Console.WriteLine(history);
            }

            Console.WriteLine();
        }
    }
}