namespace ChessGame //.Enumeration
{
    public enum ColorType
    {
        Black,
        White
    }

    public enum GameState
    {
        Init,
        IntendingMove,
        MakingMove,
        CheckmateBlackWin,
        CheckmateWhiteWin,
        Stalemate,
        Resignation
    }

    public enum PieceState
    {
        Active,
        Captured,
        Promoted
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Bishop,
        Queen,
        King,
        Knight
    }
}