using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Player : IPlayer
{
    public ColorType Color { get; set; }
    public uint MoveCountNoCaptureNoPromotion { get; set; }
    public Player() { }

    public ColorType GetColor() => throw new NotImplementedException();
    public uint GetMoveCountNoCaptureNoPromotion() => throw new NotImplementedException();
    public void SetMoveCountNoCaptureNoPromotion(uint increment) => throw new NotImplementedException();
}