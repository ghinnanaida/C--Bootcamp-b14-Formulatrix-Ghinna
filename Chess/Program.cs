﻿using System;
using System.Collections.Generic; 
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models; 

namespace ChessGame
{
    public class Program
    {
        private GameControl _gameControl;

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
                // *** CHANGED: New input flow - first ask for source square ***
                Console.WriteLine("\nEnter the square of the piece you want to move (e.g., e2), 'resign' to concede, or 'exit' to quit: ");
                string? input = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid input. Please enter a square coordinate.");
                    continue;
                }

                if (input == "resign")
                {
                    _gameControl.Resign(_gameControl.GetCurrentPlayer().GetColor());
                    break; 
                }
                
                if (input == "exit")
                {
                    Console.WriteLine("Exiting game.");
                    break;
                }

                Point? sourceCoord = ParseCoordinate(input);
                if (sourceCoord == null)
                {
                    Console.WriteLine("Invalid coordinate format. Use 'a1' to 'h8'.");
                    continue;
                }

                ISquare sourceSquare = _gameControl.Board.GetSquare(sourceCoord.Value);

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

                        // Display board with highlighted legal moves
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

                        Point? destCoord = ParseCoordinate(destInput);
                        if (destCoord == null)
                        {
                            Console.WriteLine("Invalid coordinate format. Use 'a1' to 'h8'.");
                            _gameControl.CancelMove();
                            continue;
                        }

                        ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);
                        
                        // Check if the destination is a legal move
                        if (!_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            Console.WriteLine("Invalid move. Please enter a valid destination from the highlighted squares.");
                            // Don't cancel move, let them try again
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

        private Point? ParseCoordinate(string algebraicCoord)
        {
            if (string.IsNullOrWhiteSpace(algebraicCoord) || algebraicCoord.Length != 2)
                return null;

            char fileChar = algebraicCoord[0];
            char rankChar = algebraicCoord[1];

            int x = -1; 
            int y = -1;

            if (fileChar >= 'a' && fileChar <= 'h')
            {
                x = fileChar - 'a'; 
            }
            else
            {
                return null;
            }

            if (char.IsDigit(rankChar))
            {
                y = int.Parse(rankChar.ToString()) - 1; 
            }
            else
            {
                return null;
            }

            if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
            {
                return new Point { X = x, Y = y };
            }

            return null; 
        }

        // *** ADDED: Method to convert coordinates back to algebraic notation ***
        private string CoordinateToAlgebraic(Point coord)
        {
            char file = (char)('a' + coord.X);
            char rank = (char)('1' + coord.Y);
            return $"{file}{rank}";
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
        }

        // *** ADDED: Method to display board with highlighted legal moves ***
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
                        backgroundColor = ConsoleColor.Green; // *** Highlight legal moves in green ***
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
                            cellContent = " • "; // *** Show dot for empty legal move squares ***
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
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2659" : "\u265f"; 
                    break;
            }
            return pieceChar;
        }

        private void GameControl_OnMoveDone()
        {
            Console.WriteLine("Move successful!");
        }

        private void GameControl_OnCapturePiece(IPiece capturedPiece)
        {
            Console.WriteLine($"A {capturedPiece.GetColor()} {capturedPiece.GetPieceType()} was captured!");
        }

        private void GameControl_OnCastling(IPiece king, IPiece rook)
        {
            Console.WriteLine($"{king.GetColor()} King and Rook castled!");
        }

        private void GameControl_OnEnPassant(IPiece capturedPawn)
        {
            Console.WriteLine($"{capturedPawn.GetColor()} pawn captured via En Passant!");
        }
        
        private void GameControl_OnPawnPromotion(IPiece promotedPiece)
        {
            Console.WriteLine($"{promotedPiece.GetColor()} pawn promoted to {promotedPiece.GetPieceType()}!");
        }

        private void GameControl_OnCheck()
        {
            Console.WriteLine($"CHECK! {_gameControl.GetCurrentPlayer().GetColor()} king is under attack!");
        }

        private void GameControl_OnCheckmate()
        {
            Console.WriteLine("Checkmate condition met!");
        }

        private void GameControl_OnStalemate()
        {
            Console.WriteLine("Stalemate condition met!");
        }

        public static void Main(string[] args)
        {
            var game = new Program();
            game.Run();
        }
    }
}