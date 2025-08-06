using System;
using System.Collections.Generic; 
using System.Linq;
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;
using ChessGame.Display;
using ChessGame.Factories;

namespace ChessGame
{
    public class Program
    {
        private readonly GameControl _gameControl;
        private readonly ChessDisplay _display;

        public Program(GameControl gameControl, ChessDisplay display)
        {
            _gameControl = gameControl;
            _display = display;
            
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
            _display.DisplayBoard(); 
            _display.DisplayGameMessage("🎮 Game Started!", MessageType.Success);
            _display.DisplayCurrentPlayer(_gameControl.GetCurrentPlayer().GetColor());

            while (!IsGameOver())
            {
                var movablePieces = _gameControl.GetMovablePiecesList();
                if (movablePieces.Count == 0)
                {
                    _display.DisplayError("No legal moves available!");
                    break;
                }

                _display.DisplayMovablePieces(movablePieces);
                
                _display.DisplayPrompt("\nSelect a piece to move by entering its number, 'resign' to concede, or 'exit' to quit: ");
                string? input = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    _display.DisplayError("Invalid input. Please enter a piece number.");
                    continue;
                }

                if (input == "resign")
                {
                    _gameControl.HandleResign(_gameControl.GetCurrentPlayer().GetColor());
                    break; 
                }
                
                if (input == "exit")
                {
                    _display.DisplayGameMessage("👋 Exiting game.", MessageType.Info);
                    break;
                }

                if (!int.TryParse(input, out int pieceNumber) || pieceNumber < 1 || pieceNumber > movablePieces.Count)
                {
                    _display.DisplayError($"Invalid selection. Please enter a number between 1 and {movablePieces.Count}.");
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
                            _display.DisplayWarning("This piece has no legal moves. Please select another piece.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        _display.DisplayBoardWithLegalMoves(_gameControl.CurrentLegalMoves);
                        
                        _display.DisplayGameMessage("Legal moves are highlighted in green.", MessageType.Success);
                        _display.DisplayPrompt("Enter the destination square (e.g., e4) or 'cancel' to select a different piece: ");
                        
                        string? destInput = Console.ReadLine()?.ToLower();

                        if (string.IsNullOrWhiteSpace(destInput))
                        {
                            _display.DisplayError("Invalid input.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        if (destInput == "cancel")
                        {
                            _gameControl.CancelMove();
                            _display.DisplayBoard();
                            continue;
                        }

                        Point? destCoord = _gameControl.ParseAlgebraicNotation(destInput);
                        if (destCoord == null)
                        {
                            _display.DisplayError("Invalid coordinate format. Use 'a1' to 'h8'.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);
                        
                        if (!_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            _display.DisplayError("Invalid move. Please enter a valid destination from the highlighted squares.");
                            continue;
                        }

                        bool moveSuccessful = _gameControl.MakeMove(destSquare);
                        if (moveSuccessful)
                        {
                            _display.DisplayBoard(); 
                            _display.DisplayCurrentPlayer(_gameControl.GetCurrentPlayer().GetColor());
                        }
                        else
                        {
                            _display.DisplayError("Move failed unexpectedly. Please try again.");
                            _gameControl.CancelMove();
                        }
                    }
                    else
                    {
                        _display.DisplayError("No piece at the selected source square, or it's not your piece. Please select a valid piece to move.");
                    }
                }
                catch (Exception ex)
                {
                    _display.DisplayError($"An error occurred during move processing: {ex.Message}");
                    _gameControl.CancelMove();
                }
            }

            _display.DisplayGameMessage("\n🏁 Game Over!", MessageType.Info);
            _display.DisplayGameState(_gameControl.State);
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

        private void GameControl_OnMoveDone()
        {
            _display.DisplaySuccess("Move successful!");
        }

        private void GameControl_OnCapturePiece(IPiece capturedPiece)
        {
            _display.DisplayGameMessage($"🎯 A {capturedPiece.GetColor()} {capturedPiece.GetPieceType()} was captured!", MessageType.Success);
        }

        private void GameControl_OnCastling(IPiece king, IPiece rook)
        {
            _display.DisplayGameMessage($"🏰 {king.GetColor()} King and Rook castled!", MessageType.Success);
        }

        private void GameControl_OnEnPassant(IPiece capturedPawn)
        {
            _display.DisplayGameMessage($"⚡ {capturedPawn.GetColor()} pawn captured via En Passant!", MessageType.Success);
        }
        
        private void GameControl_OnPawnPromotion(IPiece promotedPiece)
        {
            _display.DisplayGameMessage($"👑 {promotedPiece.GetColor()} pawn promoted to {promotedPiece.GetPieceType()}!", MessageType.Success);
        }

        private void GameControl_OnCheck()
        {
            _display.DisplayGameState(GameState.Check);
        }

        private void GameControl_OnCheckmate()
        {
            _display.DisplayGameMessage("🎯 Checkmate condition met!", MessageType.Info);
        }

        private void GameControl_OnStalemate()
        {
            _display.DisplayGameMessage("🤝 Stalemate condition met!", MessageType.Info);
        }

        private void GameControl_OnDraw()
        {
            _display.DisplayGameMessage("🤝 Draw by fifty move rule", MessageType.Info);
        }

        private void GameControl_OnResign(ColorType resigningPlayerColor)
        {
            string message = resigningPlayerColor == ColorType.White 
                ? "🏃 White has resigned. Black wins!"
                : "🏃 Black has resigned. White wins!";
            _display.DisplayGameMessage(message, MessageType.Info);
        }

        public static void Main(string[] args)
        {
            var dependencies = GameFactory.CreateGameDependencies();
            dependencies.Program.Run();
        }
    }
}