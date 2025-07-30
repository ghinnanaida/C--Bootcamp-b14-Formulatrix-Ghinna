using ChessGame.Enumerations;

namespace ChessGame.Interfaces;

public interface IPlayer
{ 
    ColorType GetColor();
    void SetColor(ColorType newColor);
    uint GetMoveCountNoCaptureNoPromotion();
    void SetMoveCountNoCaptureNoPromotion(uint counter);
}