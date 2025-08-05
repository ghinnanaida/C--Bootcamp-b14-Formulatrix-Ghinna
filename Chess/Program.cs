using System;
using System.Collections.Generic; 
using System.Linq;
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models; 
using ChessGame.Testing;

namespace ChessGame
{
    public class Program
    {
        private GameControl _gameControl;
        private string _lastGameMessage = "";

        public Program()
        {
            _gameControl = new GameControl();
            
            _gameControl.OnMoveDone += GameControl_OnMoveDone;
            _gameControl.OnCapturePiece += GameControl_OnCapturePiece;
            _gameControl.OnCastling += GameControl_OnCastling;
            _gameControl.OnEnPassant += GameControl_OnEnPassant;
            _gameControl.OnPawnPromotion += GameControl_OnPawnPromotion;
            _gameControl.OnCheck += GameControl_OnCheck;
            _gameControl.OnCheckmate += GameControl_OnCheckmate;
            _gameControl.OnStalemate += GameControl_OnStalemate;
            _gameControl.OnDraw += GameControl_OnDraw;
            _gameControl.OnResign += GameControl_OnResign; 
        }

        public void Run()
        {
            _gameControl.InitGame();
            DisplayBoard(); 
            Console.WriteLine("Game Started!");
            Console.WriteLine($"{_gameControl.GetCurrentPlayer().GetColor()} to move.");

            while (_gameControl.State != GameState.CheckmateWhiteWin &&
                   _gameControl.State != GameState.CheckmateBlackWin &&
                   _gameControl.State != GameState.Stalemate &&
                   _gameControl.State != GameState.FiftyMoveDraw && 
                   _gameControl.State != GameState.Resignation)
            {
                var movablePieces = _gameControl.GetMovablePiecesList();
                if (movablePieces.Count == 0)
                {
                    Console.WriteLine("No legal moves available!");
                    break;
                }

                GetMovablePiecesChoice(movablePieces);
                
                Console.WriteLine("\nSelect a piece to move by entering its number, 'resign' to concede, or 'exit' to quit: ");
                string? input = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid input. Please enter a piece number.");
                    continue;
                }

                if (input == "resign")
                {
                    _gameControl.HandleResign(_gameControl.GetCurrentPlayer().GetColor());
                    break; 
                }
                
                if (input == "exit")
                {
                    Console.WriteLine("Exiting game.");
                    break;
                }

                if (!int.TryParse(input, out int pieceNumber) || pieceNumber < 1 || pieceNumber > movablePieces.Count)
                {
                    Console.WriteLine($"Invalid selection. Please enter a number between 1 and {movablePieces.Count}.");
                    continue;
                }

                var selectedPieceInfo = movablePieces[pieceNumber - 1];
                ISquare sourceSquare = _gameControl.Board.GetSquare(selectedPieceInfo.Piece.GetCurrentCoordinate());

                try
                {
                    _gameControl.IntendMove(sourceSquare);

                    if (_gameControl.State == GameState.MakingMove && _gameControl.CurrentLegalMoves != null)
                    {
                        if (_gameControl.CurrentLegalMoves.Count == 0)
                        {
                            Console.WriteLine("This piece has no legal moves. Please select another piece.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        DisplayBoardWithLegalMoves(_gameControl.CurrentLegalMoves);
                        
                        Console.WriteLine($"Legal moves are highlighted in green.");
                        Console.WriteLine($"Enter the destination square (e.g., e4) or 'cancel' to select a different piece:");
                        
                        string? destInput = Console.ReadLine()?.ToLower();

                        if (string.IsNullOrWhiteSpace(destInput))
                        {
                            Console.WriteLine("Invalid input.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        if (destInput == "cancel")
                        {
                            _gameControl.CancelMove();
                            DisplayBoard();
                            continue;
                        }

                        Point? destCoord = _gameControl.ParseAlgebraicNotation(destInput);
                        if (destCoord == null)
                        {
                            Console.WriteLine("Invalid coordinate format. Use 'a1' to 'h8'.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);
                        
                        if (!_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            Console.WriteLine("Invalid move. Please enter a valid destination from the highlighted squares.");
                            continue;
                        }

                        bool moveSuccessful = _gameControl.MakeMove(destSquare);
                        if (moveSuccessful)
                        {
                            DisplayBoard(); 
                            Console.WriteLine($"{_gameControl.GetCurrentPlayer().GetColor()} to move.");
                        }
                        else
                        {
                            Console.WriteLine("Move failed unexpectedly. Please try again.");
                            _gameControl.CancelMove();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No piece at the selected source square, or it's not your piece. Please select a valid piece to move.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during move processing: {ex.Message}");
                    _gameControl.CancelMove();
                }
            }

            Console.WriteLine("\nGame Over!");
            switch (_gameControl.State)
            {
                case GameState.CheckmateWhiteWin:
                    Console.WriteLine("Checkmate! White wins!");
                    break;
                case GameState.CheckmateBlackWin:
                    Console.WriteLine("Checkmate! Black wins!");
                    break;
                case GameState.Stalemate:
                    Console.WriteLine("Stalemate! It's a draw.");
                    break;
                case GameState.FiftyMoveDraw:
                    Console.WriteLine("Draw by 50-move rule!");
                    break;
                case GameState.Resignation:
                    Console.WriteLine("Game ended by resignation.");
                    break;
                default:
                    Console.WriteLine("Game ended unexpectedly.");
                    break;
            }
        }

        private void GetMovablePiecesChoice(List<MovablePieceInfo> movablePieces)
        {
            Console.WriteLine("\nPieces that can move:");
            Console.WriteLine("---------------------");

            for (int i = 0; i < movablePieces.Count; i++)
            {
                var pieceInfo = movablePieces[i];
                var piece = pieceInfo.Piece;
                string pieceType = piece.GetPieceType().ToString();
                string position = pieceInfo.Position;
                int moveCount = pieceInfo.MoveCount;
                
                Console.WriteLine($"{i + 1}. {pieceType} {position} ({moveCount} legal move{(moveCount == 1 ? "" : "s")})");
            }
        }

        private void DisplayBoard()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8; 
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n    a  b  c  d  e  f  g  h ");
            Console.WriteLine("   -------------------------");

            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < 8; x++)
                {
                    ConsoleColor backgroundColor = (x + y) % 2 == 0 ? ConsoleColor.Blue : ConsoleColor.Gray;
                    Console.BackgroundColor = backgroundColor;

                    ISquare square = _gameControl.Board.GetSquare(new Point { X = x, Y = y });
                    IPiece? piece = square.GetPiece();
                    
                    string cellContent;
                    if (piece == null)
                    {
                        cellContent = "   ";
                    }
                    else
                    {
                        ConsoleColor foregroundColor = (piece.GetColor() == ColorType.White) ? ConsoleColor.White : ConsoleColor.Black;
                        Console.ForegroundColor = foregroundColor;
                        string pieceChar = GetPieceChar(piece);

                        cellContent = $" {pieceChar} ";
                    }
                    
                    Console.Write(cellContent);

                    Console.ResetColor();
                }
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"| {y + 1}");
            }
            
            Console.WriteLine("   -------------------------");
            Console.WriteLine("    a  b  c  d  e  f  g  h \n");
            Console.ResetColor();

            if (_gameControl.LastMoveSource != null && _gameControl.LastMoveDestination != null)
            {
                string sourceNotation = _gameControl.CoordinateToAlgebraic(_gameControl.LastMoveSource.GetPosition());
                string destNotation = _gameControl.CoordinateToAlgebraic(_gameControl.LastMoveDestination.GetPosition());
                string pieceType = _gameControl.LastMovedPiece?.GetPieceType().ToString() ?? "Piece";
                
                Console.WriteLine($"Last move: {pieceType} {sourceNotation} → {destNotation}");
            }
            
            if (!string.IsNullOrEmpty(_lastGameMessage))
            {
                Console.WriteLine($"{_lastGameMessage}");
                _lastGameMessage = "";
            }

        }

        private void DisplayBoardWithLegalMoves(List<ISquare> legalMoves)
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n    a  b  c  d  e  f  g  h ");
            Console.WriteLine("   -------------------------");

            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < 8; x++)
                {
                    ISquare square = _gameControl.Board.GetSquare(new Point { X = x, Y = y });
                    bool isLegalMove = legalMoves.Contains(square);

                    ConsoleColor backgroundColor;
                    if (isLegalMove)
                    {
                        backgroundColor = ConsoleColor.Green; 
                    }
                    else
                    {
                        backgroundColor = (x + y) % 2 == 0 ? ConsoleColor.Blue : ConsoleColor.Gray;
                    }
                    Console.BackgroundColor = backgroundColor;

                    IPiece? piece = square.GetPiece();

                    string cellContent;
                    if (piece == null)
                    {
                        if (isLegalMove)
                        {
                            cellContent = " • "; 
                        }
                        else
                        {
                            cellContent = "   ";
                        }
                    }
                    else
                    {
                        ConsoleColor foregroundColor = (piece.GetColor() == ColorType.White) ? ConsoleColor.White : ConsoleColor.Black;
                        Console.ForegroundColor = foregroundColor;
                        string pieceChar = GetPieceChar(piece);

                        cellContent = $" {pieceChar} ";
                    }

                    Console.Write(cellContent);

                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"| {y + 1}");
            }

            Console.WriteLine("   -------------------------");
            Console.WriteLine("    a  b  c  d  e  f  g  h \n");
            Console.ResetColor();
        }
        
        private string GetPieceChar(IPiece piece)
        {
            string pieceChar = " ";
            switch (piece.GetPieceType())
            {
                case PieceType.King:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2654" : "\u265a"; 
                    break;
                case PieceType.Queen:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2655" : "\u265b";
                    break;
                case PieceType.Rook:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2656" : "\u265c"; 
                    break;
                case PieceType.Bishop:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2657" : "\u265d"; 
                    break;
                case PieceType.Knight:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2658" : "\u265e"; 
                    break;
                case PieceType.Pawn:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2659" : "\u265F"; 
                    // pieceChar = "\u2659"; 
                    break;
            }
            return pieceChar;
        }

        private void GameControl_OnMoveDone()
        {
            _lastGameMessage += "\nMove successful!";
        }

        private void GameControl_OnCapturePiece(IPiece capturedPiece)
        {
            _lastGameMessage = $"A {capturedPiece.GetColor()} {capturedPiece.GetPieceType()} was captured!";
        }

        private void GameControl_OnCastling(IPiece king, IPiece rook)
        {
            _lastGameMessage = $"{king.GetColor()} King and Rook castled!";
        }

        private void GameControl_OnEnPassant(IPiece capturedPawn)
        {
            _lastGameMessage = $"{capturedPawn.GetColor()} pawn captured via En Passant!";
        }
        
        private void GameControl_OnPawnPromotion(IPiece promotedPiece)
        {
            _lastGameMessage = $"{promotedPiece.GetColor()} pawn promoted to {promotedPiece.GetPieceType()}!";
        }

        private void GameControl_OnCheck()
        {
            _lastGameMessage = $"CHECK! {_gameControl.GetCurrentPlayer().GetColor()} king is under attack!";
        }

        private void GameControl_OnCheckmate()
        {
            _lastGameMessage = "Checkmate condition met!";
        }

        private void GameControl_OnStalemate()
        {
            _lastGameMessage = "Stalemate condition met!";
        }

        private void GameControl_OnDraw()
        {
            _lastGameMessage = "Draw by fifty move rule";
        }

        private void GameControl_OnResign(ColorType resigningPlayerColor)
        {
            if (resigningPlayerColor == ColorType.White)
            {
                _lastGameMessage = "White has resigned. Black wins!";
            }
            else 
            {
                _lastGameMessage = "Black has resigned. White wins!";
            }
        }

        public static void Main(string[] args)
        {
            var game = new Program();
            game.Run();
            // var simulator = new ChessGameSimulator();
            // simulator.RunAllTests();
            
            // Console.WriteLine("\nPress any key to exit...");
            // Console.ReadKey();
        }
    }

}