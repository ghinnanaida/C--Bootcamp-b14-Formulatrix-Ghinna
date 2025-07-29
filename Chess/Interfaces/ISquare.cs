using ChessGame.RecordStructs;

namespace ChessGame.Interfaces;

public interface ISquare
    {
        Point GetPosition();
        IPiece GetPiece();
        void SetPieces(IPiece? piece);
    }