using System;
using System.Collections.Generic; // Make sure this is included for List and Dictionary
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs; // Make sure this namespace is correct for your Point struct
using ChessGame.Models; // Ensure this is included for Piece, Board, Square, Player etc. if they are in ChessGame.Models

namespace ChessGame
{
    // This `Program` class serves as the main entry point and console UI handler for your application.
    public class Program
    {
        // Private field to hold an instance of your GameControl
        private GameControl _gameControl;

        // Constructor for the Program class
        // This is where you initialize GameControl and subscribe to its events.
        public Program()
        {
            _gameControl = new GameControl();
            
            // Subscribe to GameControl events to get notifications and update the console UI
            _gameControl.OnMoveDone += GameControl_OnMoveDone;
            _gameControl.OnCapturePiece += GameControl_OnCapturePiece;
            _gameControl.OnCastling += GameControl_OnCastling;
            _gameControl.OnEnPassant += GameControl_OnEnPassant;
            _gameControl.OnPawnPromotion += GameControl_OnPawnPromotion;
            _gameControl.OnCheck += GameControl_OnCheck;
            _gameControl.OnCheckmate += GameControl_OnCheckmate;
            _gameControl.OnStalemate += GameControl_OnStalemate;
            // If you added a specific OnFiftyMoveDraw event, subscribe to it here:
            // _gameControl.OnFiftyMoveDraw += GameControl_OnFiftyMoveDraw;
        }

        // The 'Run' method contains the main game loop logic.
        // It drives the game, displays the board, and handles user input.
        public void Run()
        {
            _gameControl.InitGame(); // Initialize the chess board and pieces
            DisplayBoard(); // Show the initial board setup
            Console.WriteLine("Game Started!");
            Console.WriteLine($"{_gameControl.GetCurrentPlayer().GetColor()} to move.");

            // Main game loop: continues until a game end condition is met
            while (_gameControl.State != GameState.CheckmateWhiteWin &&
                   _gameControl.State != GameState.CheckmateBlackWin &&
                   _gameControl.State != GameState.Stalemate &&
                   _gameControl.State != GameState.FiftyMoveDraw && // Check for 50-move rule draw
                   _gameControl.State != GameState.Resignation)
            {
                Console.WriteLine("\nEnter your move (e.g., e2e4), 'resign' to concede, or 'exit' to quit: ");
                string? input = Console.ReadLine()?.ToLower(); // Read player input

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid input. Please enter a move.");
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

                Point? sourceCoord = ParseCoordinate(input.Substring(0, 2));
                Point? destCoord = ParseCoordinate(input.Substring(2, 2));

                if (sourceCoord == null || destCoord == null)
                {
                    Console.WriteLine("Invalid coordinate format. Use 'a1' to 'h8'.");
                    continue;
                }

                ISquare sourceSquare = _gameControl.Board.GetSquare(sourceCoord.Value);
                ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);

                try
                {
                    _gameControl.IntendMove(sourceSquare);

                    if (_gameControl.State == GameState.MakingMove && _gameControl.CurrentLegalMoves != null)
                    {
                        // Step 2: Check if the chosen destination is a legal move
                        if (_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            // Step 3: Attempt to make the move
                            bool moveSuccessful = _gameControl.MakeMove(destSquare);
                            if (moveSuccessful)
                            {
                                DisplayBoard(); 
                                Console.WriteLine($"{_gameControl.GetCurrentPlayer().GetColor()} to move.");
                            }
                            else
                            {
                                Console.WriteLine("Move failed unexpectedly. Please try again.");
                                _gameControl.CancelMove(); // Reset game state if move failed
                            }
                        }
                        else
                        {
                            Console.WriteLine("Illegal move for the selected piece. Choose a valid destination.");
                            _gameControl.CancelMove(); // Reset game state if illegal move
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
                    _gameControl.CancelMove(); // Attempt to reset state on unexpected error
                }
            } // End of while loop

            Console.WriteLine("\nGame Over!");
            // Display final game state message based on the ending state
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
                x = fileChar - 'a'; // 'a' becomes 0, 'b' becomes 1, etc.
            }
            else
            {
                return null; // Invalid file character
            }

            // Convert rank char ('1'-'8') to Y coordinate (0-7)
            if (char.IsDigit(rankChar))
            {
                y = int.Parse(rankChar.ToString()) - 1; // '1' becomes 0, '2' becomes 1, etc.
            }
            else
            {
                return null; // Invalid rank character
            }

            // Ensure coordinates are within board bounds (0-7 for both X and Y)
            if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
            {
                return new Point { X = x, Y = y };
            }

            return null; // Coordinates out of bounds or invalid format
        }

        // Renders the current state of the chess board to the console
        private void DisplayBoard()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Ensure Unicode support
            
            Console.ForegroundColor = ConsoleColor.White; // Set text color for borders
            Console.WriteLine("\n    a  b  c  d  e  f  g  h ");
            Console.WriteLine("   -------------------------");

            for (int y = 7; y >= 0; y--) // Iterate from rank 8 down to 1
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < 8; x++) // Iterate from file a to h
                {
                    // 1. Determine the background color for the square
                    // Use a new combination for better contrast: Blue and Gray
                    ConsoleColor backgroundColor = (x + y) % 2 == 0 ? ConsoleColor.Blue : ConsoleColor.Gray;
                    Console.BackgroundColor = backgroundColor;

                    // 2. Get the piece and its character
                    ISquare square = _gameControl.Board.GetSquare(new Point { X = x, Y = y });
                    IPiece? piece = square.GetPiece();
                    
                    string cellContent;
                    if (piece == null)
                    {
                        // For an empty square, just use spaces
                        cellContent = "   ";
                    }
                    else
                    {
                        // 3. Get the piece character and color
                        ConsoleColor foregroundColor = (piece.GetColor() == ColorType.White) ? ConsoleColor.White : ConsoleColor.Black;
                        Console.ForegroundColor = foregroundColor;
                        string pieceChar = GetPieceChar(piece);

                        // 4. Center the character within a fixed-width cell (3 characters wide)
                        cellContent = $" {pieceChar} ";
                    }
                    
                    // 5. Print the formatted cell content
                    Console.Write(cellContent);

                    // Reset colors for the next iteration
                    Console.ResetColor();
                }
                
                // Print the right border and rank number, then reset colors for the new line
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"| {y + 1}");
            }
            
            Console.WriteLine("   -------------------------");
            Console.WriteLine("    a  b  c  d  e  f  g  h \n");
            Console.ResetColor(); // Final reset to be safe
        }
        
        // Returns a character representation for a given piece (uppercase for White, lowercase for Black)
        private string GetPieceChar(IPiece piece)
        {
            string pieceChar = " ";
            switch (piece.GetPieceType())
            {
                case PieceType.King:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2654" : "\u265a"; // White King: ♔, Black King: ♚
                    break;
                case PieceType.Queen:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2655" : "\u265b"; // White Queen: ♕, Black Queen: ♛
                    break;
                case PieceType.Rook:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2656" : "\u265c"; // White Rook: ♖, Black Rook: ♜
                    break;
                case PieceType.Bishop:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2657" : "\u265d"; // White Bishop: ♗, Black Bishop: ♝
                    break;
                case PieceType.Knight:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2658" : "\u265e"; // White Knight: ♘, Black Knight: ♞
                    break;
                case PieceType.Pawn:
                    pieceChar = (piece.GetColor() == ColorType.White) ? "\u2659" : "\u265f"; // White Pawn: ♙, Black Pawn: ♟
                    break;
            }
            return pieceChar;
        }

        // --- Event Handlers for GameControl Notifications ---
        // These methods are called when corresponding events are raised in GameControl,
        // allowing the UI to display relevant messages to the player.

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
            // IMPORTANT: If you want to allow the player to choose promotion piece,
            // this is where you'd typically prompt for input. For now, it auto-promotes to Queen as per GameControl.
        }

        private void GameControl_OnCheck()
        {
            Console.WriteLine($"CHECK! {_gameControl.GetCurrentPlayer().GetColor()} king is under attack!");
        }

        // These handlers are mostly for simple notification, the `Run` loop handles the final game over messages.
        private void GameControl_OnCheckmate()
        {
            Console.WriteLine("Checkmate condition met!");
        }

        private void GameControl_OnStalemate()
        {
            Console.WriteLine("Stalemate condition met!");
        }

        // The `Main` method is the actual entry point where the application starts.
        // It creates an instance of the `Program` class and calls its `Run` method.
        public static void Main(string[] args)
        {
            var game = new Program(); // Creates an instance of the Program class (which sets up GameControl)
            game.Run(); // Starts the main game loop
        }
    }
}