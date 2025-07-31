// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame
{
    // Enumerations
    public enum ColorType
    {
        Black,
        White
    }

    public enum PieceState
    {
        Active,
        Captured,
        Promoted
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

    public enum PieceType
    {
        Pawn,
        Rook,
        Bishop,
        Queen,
        King,
        Knight
    }

    // Record struct
    public record struct Point(int X, int Y);

    // Interfaces
    public interface IBoard
    {
        ISquare GetSquare(Point coordinate);
        void SetSquare(Point coordinate, IPiece? piece);
    }

    public interface IPiece
    {
        ColorType GetColor();
        PieceState GetState();
        PieceType GetType();
        void SetState(PieceState newstate);
        Point GetInitialCoordinate();
        void SetInitialCoordinate(Point newCoordinate);
    }

    public interface ISquare
    {
        Point GetPosition();
        IPiece? GetPiece();
        void SetPiece(IPiece? piece);
        void SetPosition(Point coordinate);
    }

    public interface IPlayer
    {
        ColorType GetColor();
        void SetColor(ColorType newColor);
        uint GetMoveCountNoCaptureNoPromotion();
        void SetMoveCountNoCaptureNoPromotion(uint counter);
    }

    // Implementations
    public class Player : IPlayer
    {
        public ColorType Color { get; private set; }
        public uint MoveCountNoCaptureNoPromotion { get; private set; }

        public Player(ColorType color)
        {
            Color = color;
            MoveCountNoCaptureNoPromotion = 0;
        }

        public ColorType GetColor() => Color;
        public void SetColor(ColorType newColor) => Color = newColor;
        public uint GetMoveCountNoCaptureNoPromotion() => MoveCountNoCaptureNoPromotion;
        public void SetMoveCountNoCaptureNoPromotion(uint counter) => MoveCountNoCaptureNoPromotion = counter;
    }

    public class Piece : IPiece
    {
        public ColorType Color { get; private set; }
        public PieceState State { get; private set; }
        public PieceType Type { get; private set; }
        public Point InitialCoordinate { get; private set; }

        public Piece(ColorType color, PieceState state, PieceType type)
        {
            Color = color;
            State = state;
            Type = type;
        }

        public ColorType GetColor() => Color;
        public PieceState GetState() => State;
        public PieceType GetType() => Type;
        public Point GetInitialCoordinate() => InitialCoordinate;
        public void SetState(PieceState state) => State = state;
        public void SetInitialCoordinate(Point newCoordinate) => InitialCoordinate = newCoordinate;
    }

    public class Square : ISquare
    {
        public Point Coordinate { get; private set; }
        public IPiece? Piece { get; private set; }

        public Square(Point coordinate)
        {
            Coordinate = coordinate;
        }

        public Point GetPosition() => Coordinate;
        public void SetPosition(Point coordinate) => Coordinate = coordinate;
        public IPiece? GetPiece() => Piece;
        public void SetPiece(IPiece? piece) => Piece = piece;
    }

    public class Board : IBoard
    {
        public ISquare[,] Squares { get; private set; }

        public Board()
        {
            Squares = new ISquare[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Squares[x, y] = new Square(new Point(x, y));
                }
            }
        }

        public ISquare GetSquare(Point coordinate)
        {
            if (coordinate.X >= 0 && coordinate.X < 8 && coordinate.Y >= 0 && coordinate.Y < 8)
                return Squares[coordinate.X, coordinate.Y];
            throw new ArgumentException("Invalid coordinate");
        }

        public void SetSquare(Point coordinate, IPiece? piece)
        {
            GetSquare(coordinate).SetPiece(piece);
        }
    }

    public class GameController
    {
        public List<IPlayer> Players { get; private set; }
        public Dictionary<IPlayer, List<IPiece>> PlayerPieces { get; private set; }
        public IBoard Board { get; private set; }
        public GameState State { get; private set; }
        private int _currentPlayerIndex;
        private ISquare? _intendedSquareSource;
        public List<ISquare>? CurrentLegalMoves { get; private set; }

        public Action? OnMoveDone;
        public Action<IPiece>? OnCapturePiece;
        public Action<IPiece>? OnCastling;
        public Action<IPiece>? OnEnPassant;
        public Action<IPiece>? OnPawnPromotion;
        public Action? OnCheckmate;
        public Action? OnResign;
        public Action? OnStalemate;

        public GameController()
        {
            Players = new List<IPlayer>();
            PlayerPieces = new Dictionary<IPlayer, List<IPiece>>();
            Board = new Board();
            State = GameState.Init;
            _currentPlayerIndex = 0;
        }

        public void InitGame()
        {
            // Create players
            Players.Add(new Player(ColorType.White));
            Players.Add(new Player(ColorType.Black));

            foreach (var player in Players)
            {
                PlayerPieces[player] = new List<IPiece>();
            }

            // Initialize pieces
            InitializePieces();
            State = GameState.IntendingMove;
        }

        private void InitializePieces()
        {
            var whitePlayer = Players.First(p => p.GetColor() == ColorType.White);
            var blackPlayer = Players.First(p => p.GetColor() == ColorType.Black);

            // Initialize white pieces
            for (int x = 0; x < 8; x++)
            {
                var pawn = new Piece(ColorType.White, PieceState.Active, PieceType.Pawn);
                pawn.SetInitialCoordinate(new Point(x, 1));
                Board.SetSquare(new Point(x, 1), pawn);
                PlayerPieces[whitePlayer].Add(pawn);
            }

            // White back row
            var whiteBackRow = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
            for (int x = 0; x < 8; x++)
            {
                var piece = new Piece(ColorType.White, PieceState.Active, whiteBackRow[x]);
                piece.SetInitialCoordinate(new Point(x, 0));
                Board.SetSquare(new Point(x, 0), piece);
                PlayerPieces[whitePlayer].Add(piece);
            }

            // Initialize black pieces
            for (int x = 0; x < 8; x++)
            {
                var pawn = new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn);
                pawn.SetInitialCoordinate(new Point(x, 6));
                Board.SetSquare(new Point(x, 6), pawn);
                PlayerPieces[blackPlayer].Add(pawn);
            }

            // Black back row
            var blackBackRow = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
            for (int x = 0; x < 8; x++)
            {
                var piece = new Piece(ColorType.Black, PieceState.Active, blackBackRow[x]);
                piece.SetInitialCoordinate(new Point(x, 7));
                Board.SetSquare(new Point(x, 7), piece);
                PlayerPieces[blackPlayer].Add(piece);
            }
        }

        public void NextTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
            State = GameState.IntendingMove;
            _intendedSquareSource = null;
            CurrentLegalMoves = null;
        }

        public void IntendMove(ISquare sourceSquare)
        {
            if (sourceSquare.GetPiece() == null || sourceSquare.GetPiece()!.GetColor() != Players[_currentPlayerIndex].GetColor())
                return;

            _intendedSquareSource = sourceSquare;
            CurrentLegalMoves = GetLegalMoves();
            State = GameState.MakingMove;
        }

        public List<ISquare> GetLegalMoves()
        {
            if (_intendedSquareSource?.GetPiece() == null)
                return new List<ISquare>();

            var piece = _intendedSquareSource.GetPiece()!;
            var moves = new List<ISquare>();

            switch (piece.GetType())
            {
                case PieceType.Pawn:
                    moves.AddRange(GetPawnMoves(_intendedSquareSource));
                    break;
                case PieceType.Rook:
                    moves.AddRange(GetRookMoves(_intendedSquareSource));
                    break;
                case PieceType.Bishop:
                    moves.AddRange(GetBishopMoves(_intendedSquareSource));
                    break;
                case PieceType.Queen:
                    moves.AddRange(GetQueenMoves(_intendedSquareSource));
                    break;
                case PieceType.King:
                    moves.AddRange(GetKingMoves(_intendedSquareSource));
                    break;
                case PieceType.Knight:
                    moves.AddRange(GetKnightMoves(_intendedSquareSource));
                    break;
            }

            return moves;
        }

        private List<ISquare> GetPawnMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            var piece = source.GetPiece()!;
            var pos = source.GetPosition();
            int direction = piece.GetColor() == ColorType.White ? 1 : -1;

            // Forward move
            var forward = new Point(pos.X, pos.Y + direction);
            if (IsValidPosition(forward) && Board.GetSquare(forward).GetPiece() == null)
            {
                moves.Add(Board.GetSquare(forward));

                // Double forward from starting position
                if ((piece.GetColor() == ColorType.White && pos.Y == 1) || 
                    (piece.GetColor() == ColorType.Black && pos.Y == 6))
                {
                    var doubleForward = new Point(pos.X, pos.Y + 2 * direction);
                    if (IsValidPosition(doubleForward) && Board.GetSquare(doubleForward).GetPiece() == null)
                    {
                        moves.Add(Board.GetSquare(doubleForward));
                    }
                }
            }

            // Captures
            var captureLeft = new Point(pos.X - 1, pos.Y + direction);
            if (IsValidPosition(captureLeft) && Board.GetSquare(captureLeft).GetPiece() != null &&
                Board.GetSquare(captureLeft).GetPiece()!.GetColor() != piece.GetColor())
            {
                moves.Add(Board.GetSquare(captureLeft));
            }

            var captureRight = new Point(pos.X + 1, pos.Y + direction);
            if (IsValidPosition(captureRight) && Board.GetSquare(captureRight).GetPiece() != null &&
                Board.GetSquare(captureRight).GetPiece()!.GetColor() != piece.GetColor())
            {
                moves.Add(Board.GetSquare(captureRight));
            }

            return moves;
        }

        private List<ISquare> GetRookMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            var pos = source.GetPosition();
            var piece = source.GetPiece()!;

            // Horizontal and vertical directions
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int i = 0; i < 4; i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];

                for (int step = 1; step < 8; step++)
                {
                    var newPos = new Point(pos.X + dx * step, pos.Y + dy * step);
                    if (!IsValidPosition(newPos)) break;

                    var square = Board.GetSquare(newPos);
                    if (square.GetPiece() == null)
                    {
                        moves.Add(square);
                    }
                    else
                    {
                        if (square.GetPiece()!.GetColor() != piece.GetColor())
                            moves.Add(square);
                        break;
                    }
                }
            }

            return moves;
        }

        private List<ISquare> GetBishopMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            var pos = source.GetPosition();
            var piece = source.GetPiece()!;

            // Diagonal directions
            int[,] directions = { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

            for (int i = 0; i < 4; i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];

                for (int step = 1; step < 8; step++)
                {
                    var newPos = new Point(pos.X + dx * step, pos.Y + dy * step);
                    if (!IsValidPosition(newPos)) break;

                    var square = Board.GetSquare(newPos);
                    if (square.GetPiece() == null)
                    {
                        moves.Add(square);
                    }
                    else
                    {
                        if (square.GetPiece()!.GetColor() != piece.GetColor())
                            moves.Add(square);
                        break;
                    }
                }
            }

            return moves;
        }

        private List<ISquare> GetQueenMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            moves.AddRange(GetRookMoves(source));
            moves.AddRange(GetBishopMoves(source));
            return moves;
        }

        private List<ISquare> GetKingMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            var pos = source.GetPosition();
            var piece = source.GetPiece()!;

            // All 8 directions around the king
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

            for (int i = 0; i < 8; i++)
            {
                var newPos = new Point(pos.X + directions[i, 0], pos.Y + directions[i, 1]);
                if (IsValidPosition(newPos))
                {
                    var square = Board.GetSquare(newPos);
                    if (square.GetPiece() == null || square.GetPiece()!.GetColor() != piece.GetColor())
                    {
                        moves.Add(square);
                    }
                }
            }

            return moves;
        }

        private List<ISquare> GetKnightMoves(ISquare source)
        {
            var moves = new List<ISquare>();
            var pos = source.GetPosition();
            var piece = source.GetPiece()!;

            // Knight moves in L-shape
            int[,] knightMoves = { { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 } };

            for (int i = 0; i < 8; i++)
            {
                var newPos = new Point(pos.X + knightMoves[i, 0], pos.Y + knightMoves[i, 1]);
                if (IsValidPosition(newPos))
                {
                    var square = Board.GetSquare(newPos);
                    if (square.GetPiece() == null || square.GetPiece()!.GetColor() != piece.GetColor())
                    {
                        moves.Add(square);
                    }
                }
            }

            return moves;
        }

        private bool IsValidPosition(Point pos)
        {
            return pos.X >= 0 && pos.X < 8 && pos.Y >= 0 && pos.Y < 8;
        }

        public void CancelMove()
        {
            _intendedSquareSource = null;
            CurrentLegalMoves = null;
            State = GameState.IntendingMove;
        }

        public bool MakeMove(ISquare destinationSquare)
        {
            if (_intendedSquareSource == null || CurrentLegalMoves == null || !CurrentLegalMoves.Contains(destinationSquare))
                return false;

            var piece = _intendedSquareSource.GetPiece()!;
            var capturedPiece = destinationSquare.GetPiece();

            // Handle capture
            if (capturedPiece != null)
            {
                capturedPiece.SetState(PieceState.Captured);
                HandleCapturePiece(capturedPiece);
            }

            // Move the piece
            _intendedSquareSource.SetPiece(null);
            destinationSquare.SetPiece(piece);

            // Handle pawn promotion
            if (piece.GetType() == PieceType.Pawn)
            {
                var destPos = destinationSquare.GetPosition();
                if ((piece.GetColor() == ColorType.White && destPos.Y == 7) ||
                    (piece.GetColor() == ColorType.Black && destPos.Y == 0))
                {
                    HandlePawnPromotion(piece);
                }
            }

            HandleMoveDone();
            NextTurn();

            // Simple checkmate/stalemate detection (simplified)
            if (!HasValidMoves())
            {
                if (IsInCheck(Players[_currentPlayerIndex].GetColor()))
                {
                    State = Players[_currentPlayerIndex].GetColor() == ColorType.White ? GameState.CheckmateBlackWin : GameState.CheckmateWhiteWin;
                    HandleCheckmate();
                }
                else
                {
                    State = GameState.Stalemate;
                    HandleStalemate();
                }
            }

            return true;
        }

        private bool HasValidMoves()
        {
            var currentPlayer = Players[_currentPlayerIndex];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var square = Board.GetSquare(new Point(x, y));
                    var piece = square.GetPiece();
                    if (piece != null && piece.GetColor() == currentPlayer.GetColor())
                    {
                        _intendedSquareSource = square;
                        var moves = GetLegalMoves();
                        if (moves.Count > 0)
                        {
                            _intendedSquareSource = null;
                            return true;
                        }
                    }
                }
            }
            _intendedSquareSource = null;
            return false;
        }

        private bool IsInCheck(ColorType color)
        {
            // Find the king
            Point kingPos = new Point(-1, -1);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = Board.GetSquare(new Point(x, y)).GetPiece();
                    if (piece != null && piece.GetType() == PieceType.King && piece.GetColor() == color)
                    {
                        kingPos = new Point(x, y);
                        break;
                    }
                }
                if (kingPos.X != -1) break;
            }

            // Check if any enemy piece can attack the king
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = Board.GetSquare(new Point(x, y)).GetPiece();
                    if (piece != null && piece.GetColor() != color)
                    {
                        var tempSource = _intendedSquareSource;
                        _intendedSquareSource = Board.GetSquare(new Point(x, y));
                        var moves = GetLegalMoves();
                        _intendedSquareSource = tempSource;

                        if (moves.Any(m => m.GetPosition().Equals(kingPos)))
                            return true;
                    }
                }
            }
            return false;
        }

        public void HandleMoveDone() => OnMoveDone?.Invoke();
        public void HandleCapturePiece(IPiece capturedPiece) => OnCapturePiece?.Invoke(capturedPiece);
        public void HandleCastling(IPiece rook) => OnCastling?.Invoke(rook);
        public void HandleEnPassant(IPiece capturedPawn) => OnEnPassant?.Invoke(capturedPawn);
        public void HandlePawnPromotion(IPiece pawn) => OnPawnPromotion?.Invoke(pawn);
        public void HandleCheckmate() => OnCheckmate?.Invoke();
        public void HandleResign(int resignedPlayerIndex) => OnResign?.Invoke();
        public void HandleStalemate() => OnStalemate?.Invoke();

        public IPlayer GetCurrentPlayer() => Players[_currentPlayerIndex];
    }

    // Console UI Class
    public class ChessConsoleUI
    {
        private GameController _gameController;

        public ChessConsoleUI()
        {
            _gameController = new GameController();
            _gameController.OnMoveDone += () => Console.WriteLine("Move completed!");
            _gameController.OnCapturePiece += (piece) => Console.WriteLine($"{piece.GetColor()} {piece.GetType()} captured!");
            _gameController.OnCheckmate += () => Console.WriteLine("Checkmate!");
            _gameController.OnStalemate += () => Console.WriteLine("Stalemate!");
        }

        public void StartGame()
        {
            _gameController.InitGame();
            Console.WriteLine("Chess Game Started!");
            Console.WriteLine("Enter moves in format: source destination (e.g., a2 a4)");
            Console.WriteLine("Type 'quit' to exit\n");

            while (_gameController.State != GameState.CheckmateBlackWin && 
                   _gameController.State != GameState.CheckmateWhiteWin && 
                   _gameController.State != GameState.Stalemate)
            {
                DisplayBoard();
                Console.WriteLine($"{_gameController.GetCurrentPlayer().GetColor()}'s turn");
                Console.Write("Enter move: ");
                
                string input = Console.ReadLine()?.Trim() ?? "";
                if (input.ToLower() == "quit") break;

                if (ProcessMove(input))
                {
                    Console.WriteLine("Move successful!\n");
                }
                else
                {
                    Console.WriteLine("Invalid move! Try again.\n");
                }
            }

            Console.WriteLine("Game Over!");
        }

        private bool ProcessMove(string input)
        {
            var parts = input.Split(' ');
            if (parts.Length != 2) return false;

            try
            {
                var sourcePos = ParsePosition(parts[0]);
                var destPos = ParsePosition(parts[1]);

                var sourceSquare = _gameController.Board.GetSquare(sourcePos);
                var destSquare = _gameController.Board.GetSquare(destPos);

                _gameController.IntendMove(sourceSquare);
                return _gameController.MakeMove(destSquare);
            }
            catch
            {
                return false;
            }
        }

        private Point ParsePosition(string pos)
        {
            if (pos.Length != 2) throw new ArgumentException();
            
            int x = pos[0] - 'a';
            int y = pos[1] - '1';
            
            if (x < 0 || x > 7 || y < 0 || y > 7) throw new ArgumentException();
            
            return new Point(x, y);
        }

        private void DisplayBoard()
        {
            Console.Clear();
            Console.WriteLine("  a b c d e f g h");
            
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} ");
                for (int x = 0; x < 8; x++)
                {
                    var piece = _gameController.Board.GetSquare(new Point(x, y)).GetPiece();
                    Console.Write(GetPieceSymbol(piece) + " ");
                }
                Console.WriteLine($"{y + 1}");
            }
            
            Console.WriteLine("  a b c d e f g h");
            Console.WriteLine();
        }

        private char GetPieceSymbol(IPiece? piece)
        {
            if (piece == null) return '.';

            char symbol = piece.GetType() switch
            {
                PieceType.Pawn => 'P',
                PieceType.Rook => 'R',
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => '?'
            };

            return piece.GetColor() == ColorType.White ? symbol : char.ToLower(symbol);
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            var game = new ChessConsoleUI();
            game.StartGame();
        }
    }
}