using ChessGame.RecordStructs;
using ChessGame.Enumerations;

namespace ChessGame.Interfaces;

public interface IPiece
{
    ColorType GetColor();
    PieceState GetState();
    PieceType GetPieceType();
    Point GetInitialCoordinate();
    Point GetCurrentCoordinate();
    bool GetHasMoved();
    void SetState(PieceState newState);
    void SetCurrentCoordinate(Point newCoordinate);
    void SetPieceType(PieceType newType);
    void SetHasMoved(bool moved);
}