namespace ChessGame.Enumerations;

public enum GameState
{
    Init,
    IntendingMove,
    MakingMove,
    Check,
    FiftyMoveDraw,
    CheckmateBlackWin,
    CheckmateWhiteWin,
    Stalemate,
    Resignation
}