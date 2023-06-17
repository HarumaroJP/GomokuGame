namespace Game;

public interface IReadOnlyBoard
{
    int Size { get; }
    
    (int black, int white) Sum();
    Stone Get(int y, int x);
}