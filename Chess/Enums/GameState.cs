namespace ChessGame.Enumerations;

public enum GameState
{
    Init,
    IntendingMove,
    MakingMove,
    Check,
    CheckmateBlackWin,
    CheckmateWhiteWin,
    Stalemate,
    Resignation
}