using ChessGame.RecordStructs;

namespace ChessGame.Interfaces;

public interface ISquare
{
    Point GetPosition();
    IPiece? GetPiece();
    void SetPiece(IPiece? newPiece);
    void SetPosition(Point newCoordinate);
}