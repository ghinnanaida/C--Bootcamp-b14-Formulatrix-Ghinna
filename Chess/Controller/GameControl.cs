namespace ChessGame
{
    public class GameController
    {
        public List<IPlayer> Players;
        public Dictionary<IPlayer, List<IPiece>> PlayerPieces;
        public IBoard Board;
        public GameState State;

        private int _currentPlayerIndex;
        private ISquare? _intendedSquareSource;

        public List<ISquare>? CurrentLegalMoves;
        public event Action? OnMoveDone;
        public event Action<IPiece>? OnCapturePiece;
        public event Action<IPiece>? OnCastling;
        public event Action<IPiece>? OnEnPassant;
        public event Action<IPiece>? OnPawnPromotion;
        public event Action? OnCheckmate;
        public event Action? OnResign;
        public event Action? OnStalemate;
        public GameController() => throw new NotImplementedException();

        public void InitGame() => throw new NotImplementedException();
        public void NextTurn() => throw new NotImplementedException();
        public void IntendMove(ISquare sourceSquare) => throw new NotImplementedException();
        public List<ISquare> GetLegalMove() => throw new NotImplementedException();
        public void CancelMove() => throw new NotImplementedException();
        public bool MakeMove(ISquare destinationSquare) => throw new NotImplementedException();
        public void HandleMoveDone() => throw new NotImplementedException();
        public void HandleCapturePiece(IPiece capturedPiece) => throw new NotImplementedException();
        public void HandleCastling(IPiece rook) => throw new NotImplementedException();
        public void HandleEnPassant(IPiece capturedPawn) => throw new NotImplementedException();
        public void HandlePawnPromotion(IPiece Pawn) => throw new NotImplementedException();
        public void HandleCheckmate() => throw new NotImplementedException();
        public void HandleResign(int ResignedPlayerIndex) => throw new NotImplementedException();
        public void HandleStalemate() => throw new NotImplementedException();
    }
}