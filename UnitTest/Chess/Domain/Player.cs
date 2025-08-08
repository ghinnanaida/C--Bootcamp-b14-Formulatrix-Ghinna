using System.Reflection.Metadata.Ecma335;
using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Player : IPlayer
{
    public ColorType Color { get; private set; }
    public Player(ColorType color)
    {
        this.Color = color;
    }

    public ColorType GetColor() => this.Color;
}