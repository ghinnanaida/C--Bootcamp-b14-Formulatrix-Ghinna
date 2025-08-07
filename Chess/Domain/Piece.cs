using ChessGame.RecordStructs;
using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Piece : IPiece
{
    public ColorType Color { get; init;}
    public PieceState State { get; private set; }
    public PieceType Type { get; private set; }
    public Point CurrentCoordinate { get; private set; }
    public bool HasMoved { get; private set; }

    public Piece(ColorType color, PieceState state, PieceType type, Point coordinate)
    {
        this.Color = color;
        this.State = state;
        this.Type = type;
        this.CurrentCoordinate = coordinate;
        this.HasMoved = false;
    }

    public ColorType GetColor() => this.Color;
    public PieceState GetState() => this.State;
    public PieceType GetPieceType() => this.Type;
    public Point GetCurrentCoordinate() => this.CurrentCoordinate;
    public bool GetHasMoved() => this.HasMoved;

    public void SetState(PieceState newState)
    {
        this.State = newState;
    }

    public void SetCurrentCoordinate(Point newCoordinate)
    {
        this.CurrentCoordinate = newCoordinate;
    }

    public void SetPieceType(PieceType newType)
    {
        this.Type = newType;
    }
    
    public void SetHasMoved(bool moved)
    {
        this.HasMoved = moved;
    }
}