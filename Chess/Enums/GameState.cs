namespace ChessGame.Enumerations;

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