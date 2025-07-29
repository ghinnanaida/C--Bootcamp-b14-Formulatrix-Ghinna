using ChessGame.RecordStructs;
using ChessGame.Enumerations;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Piece : IPiece
{
    public readonly ColorType Color;
    public readonly PieceState State; 
    public readonly PieceType Type;
    public readonly Point InitialCoordinate;
    public Piece() { }

    public ColorType GetColor() => throw new NotImplementedException();
    public PieceState GetState() => throw new NotImplementedException();
    public PieceType GetPieceType() => throw new NotImplementedException();
    public Point GetInitialCoordinate() => throw new NotImplementedException();
    public void SetState(PieceState state) => throw new NotImplementedException();
}