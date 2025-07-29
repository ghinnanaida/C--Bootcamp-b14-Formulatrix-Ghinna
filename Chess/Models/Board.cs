using ChessGame.RecordStructs;
using ChessGame.Interfaces;

namespace ChessGame.Models;

public class Board : IBoard
{
    public ISquare[,] Squares { get; set; }
    public Board()
    {
        throw new NotImplementedException();
    }
    public ISquare GetSquare(Point coordinate)
    {
        throw new NotImplementedException();
    }

    public void SetSquare(Point coordinate, IPiece? piece)
    {
        throw new NotImplementedException();
    }

}