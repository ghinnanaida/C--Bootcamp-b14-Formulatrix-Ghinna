namespace ChessGame.RecordStructs;

public record struct Point
{
    public int X { get; init; }
    public int Y { get; init; }
    public bool IsValid => X >= 0 && X < 8 && Y >= 0 && Y < 8;
}