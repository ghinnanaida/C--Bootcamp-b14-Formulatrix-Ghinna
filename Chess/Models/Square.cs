using ChessGame.RecordStructs;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Square : ISquare
{
    public Point Coordinate { get; set; }
    public IPiece? Piece { get; set; }
    public Square() { }

    public Point GetPosition() => throw new NotImplementedException();
    public IPiece GetPiece() => throw new NotImplementedException();
    public void SetPieces(IPiece? piece) => throw new NotImplementedException();
}