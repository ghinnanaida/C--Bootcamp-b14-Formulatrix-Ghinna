using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.Models;

namespace ChessGame
{
    public class GameControl
    {
        public List<IPlayer> Players{ get; private set;}
        public Dictionary<IPlayer, List<IPiece>> PlayerPieces{ get; private set;}
        public IBoard Board{ get; private set;}
        public GameState State{ get; private set;}

        private int _currentPlayerIndex;
        private ISquare? _intendedSquareSource;

        public List<ISquare>? CurrentLegalMoves{ get; private set;}

        public event Action? OnMoveDone;
        public event Action<IPiece>? OnCapturePiece;
        public event Action<IPiece>? OnCastling;
        public event Action<IPiece>? OnEnPassant;
        public event Action<IPiece>? OnPawnPromotion;
        public event Action? OnCheckmate;
        public event Action? OnResign;
        public event Action? OnStalemate;

        public GameControl()
        {
            this.Players = new List<IPlayer>
            {
                new Player(ColorType.White),
                new Player(ColorType.Black)
            };
            this.PlayerPieces = new Dictionary<IPlayer, List<IPiece>>()
            {
                {Players[0], new List<IPiece>() },
                {Players[1], new List<IPiece>()}
            };
            this.Board = new Board();
            this.State = GameState.Init;
            this._currentPlayerIndex = 0;
            this._intendedSquareSource = null;
            this.CurrentLegalMoves = null;
        }

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