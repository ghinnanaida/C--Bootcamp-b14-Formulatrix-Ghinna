using System;
using System.Collections.Generic;
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;

namespace ChessGame.Display
{
    public class ChessDisplay
    {

        private const string WHITE_KING = "\u2654"; 
        private const string BLACK_KING = "\u265a"; 
        private const string WHITE_QUEEN = "\u2655"; 
        private const string BLACK_QUEEN = "\u265b"; 
        private const string WHITE_ROOK = "\u2656"; 
        private const string BLACK_ROOK = "\u265c"; 
        private const string WHITE_BISHOP = "\u2657"; 
        private const string BLACK_BISHOP = "\u265d"; 
        private const string WHITE_KNIGHT = "\u2658"; 
        private const string BLACK_KNIGHT = "\u265e"; 
        private const string WHITE_PAWN = "\u2659"; 
        private const string BLACK_PAWN = "\u265f";

        private const string RESET = "\u001B[0m";
        
        private const string TEXT_BLUE = "\u001B[94m"; 
        private const string TEXT_GREEN = "\u001B[92m"; 
        private const string TEXT_RED = "\u001B[91m";
        private const string TEXT_YELLOW = "\u001B[93m"; 

        private readonly GameControl _gameControl;
        private PendingMessage? _pendingMessage;

        public ChessDisplay(GameControl gameControl)
        {
            _gameControl = gameControl;
            SubscribeToGameEvents();
        }

        public void Run()
        {
            _gameControl.InitGame();
            DisplayBoard(); 
            DisplayGameMessage("🎮 Game Started!", MessageType.Info);
            DisplayCurrentPlayer(_gameControl.GetCurrentPlayer().GetColor());

            while (!IsGameOver())
            {
                var movablePieces = _gameControl.GetMovablePiecesList();
                if (movablePieces.Count == 0)
                {
                    DisplayGameMessage("No legal moves available!", MessageType.Error);
                    break;
                }

                DisplayMovablePieces(movablePieces);
                
                DisplayPrompt("\nSelect a piece to move by entering its number, 'resign' to concede, or 'exit' to quit: ");
                string? input = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayGameMessage("Invalid input. Please enter a piece number.", MessageType.Error);
                    continue;
                }

                if (input == "resign")
                {
                    _gameControl.HandleResign(_gameControl.GetCurrentPlayer().GetColor());
                    break; 
                }
                
                if (input == "exit")
                {
                    DisplayGameMessage("👋 Exiting game.", MessageType.Info);
                    break;
                }

                if (!int.TryParse(input, out int pieceNumber) || pieceNumber < 1 || pieceNumber > movablePieces.Count)
                {
                    DisplayGameMessage($"Invalid selection. Please enter a number between 1 and {movablePieces.Count}.", MessageType.Error);
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
                            DisplayGameMessage("This piece has no legal moves. Please select another piece.", MessageType.Warning);
                            _gameControl.CancelMove();
                            continue;
                        }

                        DisplayBoardWithLegalMoves(_gameControl.CurrentLegalMoves);
                        
                        DisplayGameMessage("Legal moves are highlighted in green.", MessageType.Success);
                        DisplayPrompt("\nEnter the destination square (e.g., e4) or 'cancel' to select a different piece: ");
                        
                        string? destInput = Console.ReadLine()?.ToLower();

                        if (string.IsNullOrWhiteSpace(destInput))
                        {
                            DisplayGameMessage("Invalid input.", MessageType.Error);
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
                            DisplayGameMessage("Invalid coordinate format. Use 'a1' to 'h8'.", MessageType.Error);
                            _gameControl.CancelMove();
                            continue;
                        }

                        ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);
                        
                        if (!_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            DisplayGameMessage("Invalid move. Please enter a valid destination from the highlighted squares.", MessageType.Error);
                            continue;
                        }

                        bool moveSuccessful = _gameControl.MakeMove(destSquare);
                        if (moveSuccessful)
                        {
                            var piece = _gameControl.LastMovedPiece;
                            if (piece != null && _gameControl.IsPawnPromotion(piece))
                            {
                                var pieceType = GetPromotionChoice(piece.GetColor());
                                _gameControl.HandlePawnPromotion(piece, pieceType);
                            }
                            DisplayBoard(); 
                            DisplayCurrentPlayer(_gameControl.GetCurrentPlayer().GetColor());
                        }
                        else
                        {
                            DisplayGameMessage("Move failed unexpectedly. Please try again.", MessageType.Error);
                            _gameControl.CancelMove();
                        }
                    }
                    else
                    {
                        DisplayGameMessage("No piece at the selected source square, or it's not your piece. Please select a valid piece to move.", MessageType.Error);
                    }
                }
                catch (Exception ex)
                {
                    DisplayGameMessage($"An error occurred during move processing: {ex.Message}", MessageType.Error);
                    _gameControl.CancelMove();
                }
            }

            DisplayGameMessage("\n🏁 Game Over!", MessageType.Info);
            DisplayGameState(_gameControl.State);
        }

        private bool IsGameOver()
        {
            GameState[] gameOverStates = {
                GameState.CheckmateWhiteWin,
                GameState.CheckmateBlackWin,
                GameState.Stalemate,
                GameState.FiftyMoveDraw,
                GameState.Resignation
            };

            bool result = gameOverStates.Contains(_gameControl.State);
            return result;
        }

         private void SubscribeToGameEvents()
        {
            _gameControl.OnCapturePiece += (piece) => HandleGameEvent(GameEventType.CapturePiece, piece);
            _gameControl.OnCastling += (king, rook) => HandleGameEvent(GameEventType.Castling, new[] { king, rook });
            _gameControl.OnEnPassant += (pawn) => HandleGameEvent(GameEventType.EnPassant, pawn);
            _gameControl.OnPawnPromotion += (piece) => HandleGameEvent(GameEventType.PawnPromotion, piece);
            _gameControl.OnCheck += () => HandleGameEvent(GameEventType.Check);
            _gameControl.OnCheckmate += () => HandleGameEvent(GameEventType.Checkmate);
            _gameControl.OnStalemate += () => HandleGameEvent(GameEventType.Stalemate);
            _gameControl.OnDraw += () => HandleGameEvent(GameEventType.Draw);
            _gameControl.OnResign += (color) => HandleGameEvent(GameEventType.Resign, color);
        }

        private void HandleGameEvent(GameEventType eventType, object? data = null)
        {
            string text = "";
            MessageType messageType = MessageType.Info;

            switch (eventType)
            {
                case GameEventType.CapturePiece when data is IPiece piece:
                    text = $"🎯 A {piece.GetColor()} {piece.GetPieceType()} was captured!";
                    messageType = MessageType.Success;
                    break;
                case GameEventType.Castling when data is IPiece[] pieces:
                    text = $"🏰 {pieces[0].GetColor()} King and Rook castled!";
                    messageType = MessageType.Success;
                    break;
                case GameEventType.EnPassant when data is IPiece pawn:
                    text = $"⚡ {pawn.GetColor()} pawn captured via En Passant!";
                    messageType = MessageType.Success;
                    break;
                case GameEventType.PawnPromotion when data is IPiece promotedPiece:
                    text = $"👑 {promotedPiece.GetColor()} pawn promoted to {promotedPiece.GetPieceType()}!";
                    messageType = MessageType.Success;
                    break;
                case GameEventType.Check:
                    text = "⚠️ CHECK!";
                    messageType = MessageType.Warning;
                    break;
                case GameEventType.Checkmate:
                    text = "🎯 Checkmate condition met!";
                    messageType = MessageType.Info;
                    break;
                case GameEventType.Stalemate:
                    text = "🤝 Stalemate condition met!";
                    messageType = MessageType.Info;
                    break;
                case GameEventType.Draw:
                    text = "🤝 Draw by fifty move rule";
                    messageType = MessageType.Info;
                    break;
                case GameEventType.Resign when data is ColorType color:
                    text = color == ColorType.White 
                            ? "🏃 White has resigned. Black wins!"
                            : "🏃 Black has resigned. White wins!";
                    messageType = MessageType.Info;
                    break;
            }

            if (!string.IsNullOrEmpty(text))
            {
                _pendingMessage = new PendingMessage { Text = text, Type = messageType };
            }
        }

        private void DisplayPendingMessage()
        {
            if (_pendingMessage.HasValue)
            {
                var msg = _pendingMessage.Value;
                DisplayGameMessage(msg.Text, msg.Type);
                _pendingMessage = null;
            }
        }

        private void DrawSquare(int x, int y, List<ISquare>? legalMoves)
        {
            ISquare square = _gameControl.Board.GetSquare(new Point { X = x, Y = y });
            IPiece? piece = square.GetPiece();

            bool isLightSquare = (x + y) % 2 != 0;
            bool isLegalMove = legalMoves != null && legalMoves.Contains(square);

            if (isLegalMove)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.BackgroundColor = isLightSquare ? ConsoleColor.Gray : ConsoleColor.Blue;
            }

            if (piece == null)
            {
                if (isLegalMove)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" • ");
                }
                else
                {
                    Console.Write("   ");
                }
            }
            else
            {
                Console.ForegroundColor = (piece.GetColor() == ColorType.White) ? ConsoleColor.White : ConsoleColor.Black;
                
                if (isLegalMove)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                string pieceChar = GetPieceChar(piece);
                Console.Write($" {pieceChar} ");
            }
        }

        public void DisplayBoardWithLegalMoves(List<ISquare>? legalMoves = null)
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine($"{TEXT_BLUE}    a  b  c  d  e  f  g  h {RESET}");
            Console.WriteLine($"{TEXT_BLUE}   -------------------------{RESET}");

            for (int y = 7; y >= 0; y--)
            {
                Console.ResetColor();
                Console.Write($"{y + 1} |");
                
                for (int x = 0; x < 8; x++)
                {
                    DrawSquare(x, y, legalMoves);
                }
                
                Console.ResetColor();
                Console.WriteLine($"| {y + 1}");
            }

            Console.WriteLine($"{TEXT_BLUE}   -------------------------{RESET}");
            Console.WriteLine($"{TEXT_BLUE}    a  b  c  d  e  f  g  h {RESET}");
            
            DisplayLastMove();
            DisplayPendingMessage();
        }

        public void DisplayBoard()
        {
            DisplayBoardWithLegalMoves(null);
        }

        public void DisplayMovablePieces(List<MovablePieceInfo> movablePieces)
        {
            Console.WriteLine($"\n{TEXT_GREEN}Pieces that can move:{RESET}");
            Console.WriteLine($"{TEXT_GREEN}---------------------{RESET}");

            for (int i = 0; i < movablePieces.Count; i++)
            {
                var pieceInfo = movablePieces[i];
                var piece = pieceInfo.Piece;
                string pieceType = piece.GetPieceType().ToString();
                string position = pieceInfo.Position;
                int moveCount = pieceInfo.MoveCount;
                string colorName = piece.GetColor() == ColorType.White ? "White" : "Black";
                
                Console.Write($"{TEXT_YELLOW}{i + 1}.{RESET} ");
                
                Console.ForegroundColor = piece.GetColor() == ColorType.White ? ConsoleColor.White : ConsoleColor.DarkGray;
                Console.Write($"{colorName} {pieceType}");
                Console.ResetColor(); 
                
                Console.WriteLine($" at {TEXT_BLUE}{position}{RESET} ({TEXT_GREEN}{moveCount} legal move{(moveCount == 1 ? "" : "s")}{RESET})");
            }
        }

        private string GetColorCodeForMessageType(MessageType messageType)
        {
            var colorCode = messageType switch
            {
                MessageType.Success => TEXT_GREEN,
                MessageType.Warning => TEXT_YELLOW,
                MessageType.Error => TEXT_RED,
                MessageType.Info => TEXT_BLUE,
                _ => RESET
            };
            return colorCode; 
        }

        public void DisplayGameMessage(string message, MessageType messageType = MessageType.Info)
        {
            string icon = "";
            switch (messageType)
            {
                case MessageType.Success:
                    icon = "✅ ";
                    break;
                case MessageType.Warning:
                    icon = "⚠️ ";
                    break;
                case MessageType.Error:
                    icon = "❌ ";
                    break;
            }

            string colorCode = GetColorCodeForMessageType(messageType);
            Console.WriteLine($"{colorCode}{icon}{message}{RESET}");

        }
        
        public void DisplayPrompt(string prompt, MessageType messageType = MessageType.Info)
        {
            string color = GetColorCodeForMessageType(messageType);
            Console.Write($"{color}{prompt}{RESET}");
        }

        public void DisplayGameState(GameState state)
        {
            string message = state switch
            {
                GameState.CheckmateWhiteWin => "🏆 Checkmate! White wins!",
                GameState.CheckmateBlackWin => "🏆 Checkmate! Black wins!",
                GameState.Stalemate => "🤝 Stalemate! It's a draw.",
                GameState.FiftyMoveDraw => "🤝 Draw by 50-move rule!",
                GameState.Resignation => "🏃 Game ended by resignation.",
                GameState.Check => "⚠️ CHECK!",
                _ => ""
            };

            if (!string.IsNullOrEmpty(message))
            {
                MessageType messageType = state == GameState.Check ? MessageType.Warning : MessageType.Success;
                DisplayGameMessage(message, messageType);
            }
        }

        public void DisplayCurrentPlayer(ColorType playerColor)
        {
            Console.ResetColor();
            if (playerColor == ColorType.White)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nWhite to move.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\nBlack to move.");
            }
            Console.ResetColor();
        }

        public PieceType GetPromotionChoice(ColorType color)
        {
            Console.WriteLine($"\n{TEXT_YELLOW}🔄 {color} pawn reached the end! Choose promotion:{RESET}");
            Console.WriteLine($"{TEXT_GREEN}1.{RESET} Queen   {(color == ColorType.White ? WHITE_QUEEN : BLACK_QUEEN)}");
            Console.WriteLine($"{TEXT_GREEN}2.{RESET} Rook    {(color == ColorType.White ? WHITE_ROOK : BLACK_ROOK)}");
            Console.WriteLine($"{TEXT_GREEN}3.{RESET} Bishop  {(color == ColorType.White ? WHITE_BISHOP : BLACK_BISHOP)}");
            Console.WriteLine($"{TEXT_GREEN}4.{RESET} Knight  {(color == ColorType.White ? WHITE_KNIGHT : BLACK_KNIGHT)}");
            DisplayPrompt("Enter your choice (1-4): ", MessageType.Info);

            while (true)
            {
                string? input = Console.ReadLine();
                PieceType result = input switch
                {
                    "1" => PieceType.Queen,
                    "2" => PieceType.Rook,
                    "3" => PieceType.Bishop,
                    "4" => PieceType.Knight,
                    _ => PieceType.Pawn 
                };

                if (result != PieceType.Pawn)
                {
                    return result;
                }

                DisplayGameMessage("Invalid choice. Please enter 1-4: ", MessageType.Error);
            }
        }

        private void DisplayLastMove()
        {
            if (_gameControl.LastMoveSource != null && _gameControl.LastMoveDestination != null)
            {
                string sourceNotation = _gameControl.CoordinateToAlgebraic(_gameControl.LastMoveSource.GetPosition());
                string destNotation = _gameControl.CoordinateToAlgebraic(_gameControl.LastMoveDestination.GetPosition());
                string pieceType = _gameControl.LastMovedPiece?.GetPieceType().ToString() ?? "Piece";
                
                Console.WriteLine($"\n{TEXT_BLUE}Last move: {TEXT_YELLOW}{pieceType}{RESET} {TEXT_GREEN}{sourceNotation}{RESET} → {TEXT_GREEN}{destNotation}{RESET}");
            }
        }
        
        private string GetPieceChar(IPiece piece)
        {
            string pieceChar = piece.GetPieceType() switch
            {
                PieceType.King => piece.GetColor() == ColorType.White ? WHITE_KING : BLACK_KING,
                PieceType.Queen => piece.GetColor() == ColorType.White ? WHITE_QUEEN : BLACK_QUEEN,
                PieceType.Rook => piece.GetColor() == ColorType.White ? WHITE_ROOK : BLACK_ROOK,
                PieceType.Bishop => piece.GetColor() == ColorType.White ? WHITE_BISHOP : BLACK_BISHOP,
                PieceType.Knight => piece.GetColor() == ColorType.White ? WHITE_KNIGHT : BLACK_KNIGHT,
                PieceType.Pawn => piece.GetColor() == ColorType.White ? WHITE_PAWN : BLACK_PAWN,
                _ => " "
            };
            return pieceChar;
        }
    }
}