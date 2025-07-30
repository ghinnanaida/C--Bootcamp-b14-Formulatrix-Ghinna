using System.Reflection.Metadata.Ecma335;
using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Player : IPlayer
{
    public ColorType Color { get; private set; }
    public uint MoveCountNoCaptureNoPromotion { get; private set; }
    public Player(ColorType color)
    {
        this.Color = color;
        this.MoveCountNoCaptureNoPromotion = 0;
    }

    public ColorType GetColor() => this.Color;
    public void SetColor(ColorType newColor)
    {
        this.Color = newColor;
    }

    public uint GetMoveCountNoCaptureNoPromotion() => this.MoveCountNoCaptureNoPromotion;
    public void SetMoveCountNoCaptureNoPromotion(uint counter)
    {
        this.MoveCountNoCaptureNoPromotion = counter;
    }
}