using ChessGame.Interfaces;
namespace ChessGame.RecordStructs;

public record struct MovablePieceInfo
{
    public IPiece Piece { get; init; }
    public string Position { get; init; }
    public int MoveCount { get; init; }
}