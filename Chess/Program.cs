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
                Console.WriteLine("\nEnter your move (e.g., e2e4): ");
                string? input = Console.ReadLine()?.ToLower(); // Read player input

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid input. Please enter a move.");
                    continue; // Skip to next loop iteration
                }
                
                // Allow 'exit' command to quit the game
                if (input == "exit")
                {
                    Console.WriteLine("Exiting game.");
                    break; // Exit the game loop
                }

                // Parse the input string into source and destination coordinates
                Point? sourceCoord = ParseCoordinate(input.Substring(0, 2));
                Point? destCoord = ParseCoordinate(input.Substring(2, 2));

                if (sourceCoord == null || destCoord == null)
                {
                    Console.WriteLine("Invalid coordinate format. Use 'a1' to 'h8'.");
                    continue; // Skip to next loop iteration
                }

                // Get the ISquare objects corresponding to the parsed coordinates
                ISquare sourceSquare = _gameControl.Board.GetSquare(sourceCoord.Value);
                ISquare destSquare = _gameControl.Board.GetSquare(destCoord.Value);

                // --- Game Logic Flow for Console UI ---
                try
                {
                    // Step 1: Player intends to move a piece from the source square
                    _gameControl.IntendMove(sourceSquare);

                    // If IntendMove was successful and a piece is selected
                    if (_gameControl.State == GameState.MakingMove && _gameControl.CurrentLegalMoves != null)
                    {
                        // Step 2: Check if the chosen destination is a legal move
                        if (_gameControl.CurrentLegalMoves.Contains(destSquare))
                        {
                            // Step 3: Attempt to make the move
                            bool moveSuccessful = _gameControl.MakeMove(destSquare);
                            if (moveSuccessful)
                            {
                                DisplayBoard(); // Refresh the board display after a successful move
                                // Display whose turn it is next (GameControl.NextTurn already changed _currentPlayerIndex)
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

        // --- Helper Methods for Console UI ---

        // Converts an algebraic coordinate string (e.g., "a1") to a Point struct (e.g., X=0, Y=0)
        private Point? ParseCoordinate(string algebraicCoord)
        {
            if (string.IsNullOrWhiteSpace(algebraicCoord) || algebraicCoord.Length != 2)
                return null;

            char fileChar = algebraicCoord[0]; // 'a' through 'h'
            char rankChar = algebraicCoord[1]; // '1' through '8'

            int x = -1; // Corresponds to file (column 0-7)
            int y = -1; // Corresponds to rank (row 0-7)

            // Convert file char ('a'-'h') to X coordinate (0-7)
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
            Console.Clear(); // Clear console for a fresh board view (optional, but good for dynamic display)
            Console.WriteLine("\n    a b c d e f g h"); // Column headers
            Console.WriteLine("   -----------------"); // Separator
            for (int y = 7; y >= 0; y--) // Iterate from rank 8 (top) down to 1 (bottom)
            {
                Console.Write($"{y + 1} | "); // Rank number
                for (int x = 0; x < 8; x++) // Iterate from file a (left) to h (right)
                {
                    ISquare square = _gameControl.Board.GetSquare(new Point { X = x, Y = y });
                    IPiece? piece = square.GetPiece();

                    if (piece == null)
                    {
                        Console.Write(". "); // Represents an empty square
                    }
                    else
                    {
                        char pieceChar = GetPieceChar(piece); // Get character representation of the piece
                        Console.Write($"{pieceChar} ");
                    }
                }
                Console.WriteLine($"| {y + 1}"); // End of row, repeat rank number
            }
            Console.WriteLine("   -----------------"); // Separator
            Console.WriteLine("    a b c d e f g h\n"); // Column headers again
        }

        // Returns a character representation for a given piece (uppercase for White, lowercase for Black)
        private char GetPieceChar(IPiece piece)
        {
            char pieceChar = ' ';
            switch (piece.GetPieceType())
            {
                case PieceType.Pawn: pieceChar = 'P'; break;
                case PieceType.Rook: pieceChar = 'R'; break;
                case PieceType.Knight: pieceChar = 'N'; break;
                case PieceType.Bishop: pieceChar = 'B'; break;
                case PieceType.Queen: pieceChar = 'Q'; break;
                case PieceType.King: pieceChar = 'K'; break;
                // Add any other piece types if you have them
                default: pieceChar = '?'; break; // Fallback for unknown piece types
            }

            // Convert to lowercase if the piece is Black
            if (piece.GetColor() == ColorType.Black)
            {
                pieceChar = char.ToLower(pieceChar);
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