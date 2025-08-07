using ChessGame.RecordStructs;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Board : IBoard
{
    public ISquare[,] Squares { get; private set; }
    public Board()
    {
        this.Squares = new ISquare[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++)
            {
                this.Squares[x, y] = new Square(new Point { X = x, Y = y });
            }
        }
    }
    public ISquare GetSquare(Point coordinate)
    {
        if (coordinate.X < 0 || coordinate.X >= 8 || coordinate.Y < 0 || coordinate.Y >= 8)
        {
            throw new ArgumentOutOfRangeException(nameof(coordinate), "Coordinate is out of board bounds.");
        }
        var square = this.Squares[coordinate.X, coordinate.Y];
        return square;
    }

    public void SetSquare(Point coordinate, IPiece? piece)
    {
        if (coordinate.X < 0 || coordinate.X >= 8 || coordinate.Y < 0 || coordinate.Y >= 8)
        {
            throw new ArgumentOutOfRangeException(nameof(coordinate), "Coordinate is out of board bounds.");
        }
        this.Squares[coordinate.X, coordinate.Y].SetPiece(piece);
    }

}