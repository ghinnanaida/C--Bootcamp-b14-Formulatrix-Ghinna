namespace ChessGame //.Interfaces
{
    public interface IBoard
    {
        ISquare GetSquare(Point coordinate);
        void SetSquare(Point coordinate, IPiece? piece);
    }

    public interface ISquare
    {
        Point GetPosition();
        IPiece GetPiece();
        void SetPieces(IPiece? piece);
    }

    public interface IPiece
    {
        ColorType GetColor();
        PieceState GetState();
        PieceType GetPieceType();
        Point GetInitialCoordinate();
        void SetState(PieceState state);
    }

    public interface IPlayer
    { 
        ColorType GetColor();
        uint GetMoveCountNoCaptureNoPromotion();
        void SetMoveCountNoCaptureNoPromotion(uint increment);
    }
}