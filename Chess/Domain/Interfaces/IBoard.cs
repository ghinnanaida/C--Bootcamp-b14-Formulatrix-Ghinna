using ChessGame.RecordStructs;

namespace ChessGame.Interfaces;

public interface IBoard
{
    ISquare GetSquare(Point coordinate);
    void SetSquare(Point coordinate, IPiece? piece);
}