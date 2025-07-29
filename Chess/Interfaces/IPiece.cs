using ChessGame.RecordStructs;
using ChessGame.Enumerations;

namespace ChessGame.Interfaces;

public interface IPiece
{
    ColorType GetColor();
    PieceState GetState();
    PieceType GetPieceType();
    Point GetInitialCoordinate();
    void SetState(PieceState state);
}