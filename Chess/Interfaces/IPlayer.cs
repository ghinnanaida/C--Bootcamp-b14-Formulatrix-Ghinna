using ChessGame.Enumerations;

namespace ChessGame.Interfaces;

public interface IPlayer
{ 
    ColorType GetColor();
    uint GetMoveCountNoCaptureNoPromotion();
    void SetMoveCountNoCaptureNoPromotion(uint increment);
}