using System;

namespace ChessGame //.Classes
{
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

    public class Square : ISquare
    {
        public Point Coordinate { get; set; }
        public IPiece? Piece { get; set; }
        public Square(){}

        public Point GetPosition() => throw new NotImplementedException();
        public IPiece GetPiece() => throw new NotImplementedException();
        public void SetPieces(IPiece? piece) => throw new NotImplementedException();
    }

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

    public class Player : IPlayer
    {
        public ColorType Color { get; set; }
        public uint MoveCountNoCaptureNoPromotion { get; set; }
        public Player() { }

        public ColorType GetColor() => throw new NotImplementedException();
        public uint GetMoveCountNoCaptureNoPromotion() => throw new NotImplementedException();
        public void SetMoveCountNoCaptureNoPromotion(uint increment) => throw new NotImplementedException();
    }
}