using ChessGame.RecordStructs;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Square : ISquare
{
    public Point Coordinate { get; private set; }
    public IPiece? Piece { get; private set; }
    public Square(Point initCoordinate)
    {
        this.Coordinate = initCoordinate;
        this.Piece = null;
    }

    public Point GetPosition() => this.Coordinate;

    public IPiece? GetPiece() => this.Piece;
    public void SetPiece(IPiece? newPiece)
    {
        this.Piece = newPiece;
    }
}