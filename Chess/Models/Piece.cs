using ChessGame.RecordStructs;
using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Piece : IPiece
{
    public ColorType Color { get; }
    public PieceState State { get; private set; }
    public PieceType Type { get; }
    public Point InitialCoordinate { get; private set; }
    public Piece(ColorType color, PieceState state, PieceType type, Point initialCoordinate)
    {
        this.Color = color;
        this.State = state;
        this.Type = type;
        this.InitialCoordinate = initialCoordinate;
    }

    public ColorType GetColor() => this.Color;
    public PieceState GetState() => this.State;
    public PieceType GetPieceType() => this.Type;
    public Point GetInitialCoordinate() => this.InitialCoordinate;

    public void SetState(PieceState newState)
    {
        this.State = newState;
    }

    public void SetInitialCoordinate(Point newCoordinate)
    {
        this.InitialCoordinate = newCoordinate;
    }
}