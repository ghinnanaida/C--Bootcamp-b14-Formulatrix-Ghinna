using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
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

        public void InitGame() {

            for (int i = 0; i < 8; i++){
                var whitePawn = new Piece(ColorType.White, PieceState.Active, PieceType.Pawn, new Point{X=i, Y=1});
                this.Board.SetSquare(new Point{X=i, Y=1}, whitePawn);
                PlayerPieces[Players[0]].Add(whitePawn);

                var blackPawn = new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn, new Point{X=i, Y=6});
                this.Board.SetSquare(new Point{X=i, Y=6}, blackPawn);
                PlayerPieces[Players[1]].Add(blackPawn);

            }   

            var whiteRookQueenSide = new Piece(ColorType.White, PieceState.Active, PieceType.Rook, new Point{X=0, Y=0});
            this.Board.SetSquare(new Point{X=0, Y=0}, whiteRookQueenSide);
            PlayerPieces[Players[0]].Add(whiteRookQueenSide);
            var whiteRookKingSide = new Piece(ColorType.White, PieceState.Active, PieceType.Rook, new Point{X=7, Y=0});
            this.Board.SetSquare(new Point{X=7, Y=0}, whiteRookKingSide);
            PlayerPieces[Players[0]].Add(whiteRookKingSide);

            var blackRookQueenSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Rook, new Point{X=0, Y=7});
            this.Board.SetSquare(new Point{X=0, Y=7}, blackRookQueenSide);
            PlayerPieces[Players[1]].Add(blackRookQueenSide);
            var blackRookKingSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Rook, new Point{X=7, Y=7});
            this.Board.SetSquare(new Point{X=7, Y=7}, blackRookKingSide);
            PlayerPieces[Players[1]].Add(blackRookKingSide);

            var whiteKnightQueenSide = new Piece(ColorType.White, PieceState.Active, PieceType.Knight, new Point{X=1, Y=0});
            this.Board.SetSquare(new Point{X=1, Y=0}, whiteKnightQueenSide);
            PlayerPieces[Players[0]].Add(whiteKnightQueenSide);
            var whiteKnightKingSide = new Piece(ColorType.White, PieceState.Active, PieceType.Knight, new Point{X=6, Y=0});
            this.Board.SetSquare(new Point{X=6, Y=0}, whiteKnightKingSide);
            PlayerPieces[Players[0]].Add(whiteKnightKingSide);

            var blackKnightQueenSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Knight, new Point{X=1, Y=7});
            this.Board.SetSquare(new Point{X=1, Y=7}, blackKnightQueenSide);
            PlayerPieces[Players[1]].Add(blackKnightQueenSide);
            var blackKnightKingSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Knight, new Point{X=6, Y=7});
            this.Board.SetSquare(new Point{X=6, Y=7}, blackKnightKingSide);
            PlayerPieces[Players[1]].Add(blackKnightKingSide);

            var whiteBishopQueenSide = new Piece(ColorType.White, PieceState.Active, PieceType.Bishop, new Point{X=2, Y=0});
            this.Board.SetSquare(new Point{X=2, Y=0}, whiteBishopQueenSide);
            PlayerPieces[Players[0]].Add(whiteBishopQueenSide);
            var whiteBishopKingSide = new Piece(ColorType.White, PieceState.Active, PieceType.Bishop, new Point{X=5, Y=0});
            this.Board.SetSquare(new Point{X=5, Y=0}, whiteBishopKingSide);
            PlayerPieces[Players[0]].Add(whiteBishopKingSide);

            var blackBishopQueenSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Bishop, new Point{X=2, Y=7});
            this.Board.SetSquare(new Point{X=2, Y=7}, blackBishopQueenSide);
            PlayerPieces[Players[1]].Add(blackBishopQueenSide);
            var blackBishopKingSide = new Piece(ColorType.Black, PieceState.Active, PieceType.Bishop, new Point{X=5, Y=7});
            this.Board.SetSquare(new Point{X=5, Y=7}, blackBishopKingSide);
            PlayerPieces[Players[1]].Add(blackBishopKingSide);
            
            var whiteQueen = new Piece(ColorType.White, PieceState.Active, PieceType.Queen, new Point{X=3, Y=0});
            this.Board.SetSquare(new Point{X=3, Y=0}, whiteQueen);
            PlayerPieces[Players[0]].Add(whiteQueen);
            var whiteKing = new Piece(ColorType.White, PieceState.Active, PieceType.King, new Point{X=4, Y=0});
            this.Board.SetSquare(new Point{X=4, Y=0}, whiteKing);
            PlayerPieces[Players[0]].Add(whiteKing);

            var blackQueen = new Piece(ColorType.Black, PieceState.Active, PieceType.Queen, new Point{X=3, Y=7});
            this.Board.SetSquare(new Point{X=3, Y=7}, blackQueen);
            PlayerPieces[Players[1]].Add(blackQueen);
            var blackKing = new Piece(ColorType.Black, PieceState.Active, PieceType.King, new Point{X=4, Y=7});
            this.Board.SetSquare(new Point{X=4, Y=7}, blackKing);
            PlayerPieces[Players[1]].Add(blackKing);

            this.State = GameState.IntendingMove;
            this._currentPlayerIndex = 0;
        } 

        public void NextTurn() {
            this._currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;

            this.State = GameState.IntendingMove;
            this._intendedSquareSource = null;
            this.CurrentLegalMoves = null;

        }
        public void IntendMove(ISquare sourceSquare)
        {
            if (State != GameState.IntendingMove)
            {
                Console.WriteLine("Cannot intend move in current game state.");
                return;
            }

            var piece = sourceSquare.GetPiece();
            if (piece == null || piece.GetColor() != Players[_currentPlayerIndex].GetColor())
            {
                Console.WriteLine("No piece or not your piece in selected square!");
                this._intendedSquareSource = null;
                this.State = GameState.IntendingMove;
                return;
            }

            this._intendedSquareSource = sourceSquare;
            this.State = GameState.MakingMove;
            this.CurrentLegalMoves = GetLegalMove(this._intendedSquareSource);
            
        }

        public List<ISquare> GetLegalMove(ISquare sourceSquare) {
            List<ISquare> legalMoves = new List<ISquare>();
            var piece = sourceSquare.GetPiece();
            var position = sourceSquare.GetPosition();

            if (piece == null || piece.GetState() != PieceState.Active)
                return legalMoves;

            var pieceType = piece.GetPieceType();
            var pieceColor = piece.GetColor();

            if (pieceType == PieceType.Pawn)
            {
                int direction = pieceColor == ColorType.White ? 1 : -1;
                int startY = pieceColor == ColorType.White ? 1 : 6;
                
                // Forward move
                var oneStep = new Point { X = position.X, Y = position.Y + direction };
                if (oneStep.IsValid)
                {
                    var oneStepSquare = this.Board.GetSquare(oneStep);
                    if (oneStepSquare.GetPiece() == null)
                    {
                        legalMoves.Add(oneStepSquare);
                        
                        // Double move from starting position
                        if (position.Y == startY)
                        {
                            var twoStep = new Point {X = position.X, Y = position.Y + 2 * direction };
                            if (twoStep.IsValid)
                            {
                                var twoStepSquare = Board.GetSquare(twoStep);
                                if (twoStepSquare.GetPiece() == null)
                                {
                                    legalMoves.Add(twoStepSquare);
                                }
                            }
                        }
                    }
                }
                
                // Diagonal captures
                var captureLeft = new Point { X = position.X - 1, Y = position.Y + direction };
                if (captureLeft.IsValid)
                {
                    var leftSquare = this.Board.GetSquare(captureLeft);
                    var target = leftSquare.GetPiece();
                    if (target != null && target.GetColor() != pieceColor && target.GetState() == PieceState.Active)
                    {
                        legalMoves.Add(leftSquare);
                    }
                }
                
                var captureRight = new Point { X = position.X + 1, Y = position.Y + direction };
                if (captureRight.IsValid)
                {
                    var rightSquare = this.Board.GetSquare(captureRight);
                    var target = rightSquare.GetPiece();
                    if (target != null && target.GetColor() != pieceColor && target.GetState() == PieceState.Active)
                    {
                        legalMoves.Add(rightSquare);
                    }
                }
            }
            else if (pieceType == PieceType.Rook)
            {
                // Horizontal and vertical directions
                Point[] directions = { new Point { X = 0, Y = 1 }, new Point { X = 0, Y = -1 }, new Point { X = 1, Y = 0 }, new Point { X = -1, Y = 0 } };
                
                foreach (var dir in directions)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        var newPos = new Point { X = position.X + i * dir.X, Y = position.Y + i * dir.Y };
                        
                        if (!newPos.IsValid) break;
                        
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null)
                        {
                            legalMoves.Add(square);
                        }
                        else if (targetPiece.GetState() == PieceState.Active)
                        {
                            if (targetPiece.GetColor() != pieceColor)
                            {
                                legalMoves.Add(square); // Capture
                            }
                            break; // Can't move further
                        }
                    }
                }
            }
            else if (pieceType == PieceType.Knight)
            {
                // Knight moves in L-shape
                Point[] knightMoves = {
                    new Point{ X = 2, Y = 1}, new Point{ X = 2, Y = -1}, new Point{ X = -2, Y = 1}, new Point{ X = -2, Y = -1},
                    new Point{ X = 1, Y = 2}, new Point{ X = 1, Y = -2}, new Point{ X = -1, Y = 2}, new Point{ X = -1, Y = -2}
                };
                
                foreach (var move in knightMoves)
                {
                    var newPos = new Point { X = position.X + move.X, Y = position.Y + move.Y };
                    
                    if (newPos.IsValid)
                    {
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null || 
                            (targetPiece.GetState() == PieceState.Active && targetPiece.GetColor() != pieceColor))
                        {
                            legalMoves.Add(square);
                        }
                    }
                }
            }
            else if (pieceType == PieceType.Bishop)
            {
                // Diagonal directions
                Point[] directions = { new Point { X = 1, Y = 1 }, new Point{ X = 1, Y = -1}, new Point{ X = -1, Y = 1}, new Point{ X = -1, Y = -1} };
                
                foreach (var dir in directions)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        var newPos = new Point { X = position.X + i * dir.X, Y = position.Y + i * dir.Y };
                        
                        if (!newPos.IsValid) break;
                        
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null)
                        {
                            legalMoves.Add(square);
                        }
                        else if (targetPiece.GetState() == PieceState.Active)
                        {
                            if (targetPiece.GetColor() != pieceColor)
                            {
                                legalMoves.Add(square); // Capture
                            }
                            break; // Can't move further
                        }
                    }
                }
            }
            else if (pieceType == PieceType.Queen)
            {
                // Queen combines rook and bishop moves
                // Horizontal and vertical directions (like rook)
                Point[] rookDirections = { new Point{ X = 0, Y = 1}, new Point{X = 0, Y = -1}, new Point{X = 1, Y = 0}, new Point{X = -1, Y = 0} };
                
                foreach (var dir in rookDirections)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        var newPos = new Point { X = position.X + i * dir.X, Y = position.Y + i * dir.Y };
                        
                        if (!newPos.IsValid) break;
                        
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null)
                        {
                            legalMoves.Add(square);
                        }
                        else if (targetPiece.GetState() == PieceState.Active)
                        {
                            if (targetPiece.GetColor() != pieceColor)
                            {
                                legalMoves.Add(square); // Capture
                            }
                            break; // Can't move further
                        }
                    }
                }
                
                // Diagonal directions (like bishop)
                Point[] bishopDirections = { new Point{ X = 1, Y = 1 }, new Point{ X = 1, Y = -1 }, new Point { X = -1, Y = 1 }, new Point { X = -1, Y = -1 } };
                
                foreach (var dir in bishopDirections)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        var newPos = new Point { X = position.X + i * dir.X, Y = position.Y + i * dir.Y };
                        
                        if (!newPos.IsValid) break;
                        
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null)
                        {
                            legalMoves.Add(square);
                        }
                        else if (targetPiece.GetState() == PieceState.Active)
                        {
                            if (targetPiece.GetColor() != pieceColor)
                            {
                                legalMoves.Add(square); // Capture
                            }
                            break; // Can't move further
                        }
                    }
                }
            }
            else if (pieceType == PieceType.King)
            {
                // King moves one square in any direction
                Point[] directions = {
                    new Point{ X = 0, Y = 1}, new Point{X = 0, Y = -1}, new Point{ X = 1, Y = 0 }, new Point{ X = -1, Y = 0},
                    new Point{ X = 1, Y = 1}, new Point{X = 1, Y = -1}, new Point{ X = -1, Y = 1 }, new Point{ X = -1, Y = -1}
                };
                
                foreach (var dir in directions)
                {
                    var newPos = new Point { X = position.X + dir.X, Y = position.Y + dir.Y };
                    
                    if (newPos.IsValid)
                    {
                        var square = this.Board.GetSquare(newPos);
                        var targetPiece = square.GetPiece();
                        
                        if (targetPiece == null || 
                            (targetPiece.GetState() == PieceState.Active && targetPiece.GetColor() != pieceColor))
                        {
                            legalMoves.Add(square);
                        }
                    }
                }
                
                // TODO: Add castling logic if needed
                // This would require checking if king and rook haven't moved,
                // no pieces between them, and king not in check
            }
            
            return legalMoves;  
        }
        public void CancelMove()
        {
            this._intendedSquareSource = null;
            this.CurrentLegalMoves = null;
            this.State = GameState.IntendingMove;
        }
        public bool MakeMove(ISquare destinationSquare){
            if (State != GameState.MakingMove || _intendedSquareSource == null)
            {
                Console.WriteLine("Cannot make move. Either not in MakingMove state or no source selected.");
                return false;
            }

            if (CurrentLegalMoves == null || !CurrentLegalMoves.Contains(destinationSquare))
            {
                Console.WriteLine("Invalid move: Destination square is not a legal move for the selected piece.");
                return false;
            }

            IPiece? pieceToMove = _intendedSquareSource.GetPiece();
            if (pieceToMove == null)
            {
                Console.WriteLine("Error: No piece on the intended source square.");
                return false;
            }

            // Handle potential capture
            IPiece? capturedPiece = destinationSquare.GetPiece();
            if (capturedPiece != null)
            {
                capturedPiece.SetState(PieceState.Captured);
                HandleCapturePiece(capturedPiece);
                // Remove captured piece from the player's pieces list
                var opponentPlayer = Players.First(p => p.GetColor() != pieceToMove.GetColor());
                PlayerPieces[opponentPlayer].Remove(capturedPiece);
            }

            // Move the piece
            _intendedSquareSource.SetPiece(null); // Remove piece from source
            destinationSquare.SetPiece(pieceToMove); // Place piece on destination
            pieceToMove.SetInitialCoordinate(destinationSquare.GetPosition()); // Update piece's "current" coordinate

            // Reset move related state
            _intendedSquareSource = null;
            CurrentLegalMoves = null;

            HandleMoveDone(); // Trigger OnMoveDone event

            // Handle special moves (Castling, En Passant, Pawn Promotion)
            // You would add specific logic here.
            // Example for pawn promotion (simplified):
            if (pieceToMove.GetType() == PieceType.Pawn)
            {
                // Check if pawn reached the end rank
                if ((pieceToMove.GetColor() == ColorType.White && destinationSquare.GetPosition().Y == 7) ||
                    (pieceToMove.GetColor() == ColorType.Black && destinationSquare.GetPosition().Y == 0))
                {
                    // This is where you'd typically prompt the user for promotion choice
                    // For now, let's just promote to Queen
                    pieceToMove.SetType(PieceType.Queen);
                    pieceToMove.SetState(PieceState.Promoted); // Mark as promoted
                    HandlePawnPromotion(pieceToMove);
                }
            }


            // Update player's move count (for 50-move rule)
            var currentPlayer = Players[_currentPlayerIndex];
            if (capturedPiece != null || pieceToMove.GetType() == PieceType.Pawn)
            {
                currentPlayer.SetMoveCountNoCaptureNoPromotion(0); // Reset if capture or pawn move
            }
            else
            {
                currentPlayer.SetMoveCountNoCaptureNoPromotion(currentPlayer.GetMoveCountNoCaptureNoPromotion() + 1);
            }

            // Check for game end conditions (Checkmate, Stalemate)
            // This is complex logic to be implemented later.
            // For now, just advance turn.
            NextTurn();

            return true;
        }
        public void HandleMoveDone()
        {
            OnMoveDone?.Invoke();
            Console.WriteLine("Move done.");
        }
        public void HandleCapturePiece(IPiece capturedPiece) {
            OnCapturePiece?.Invoke(capturedPiece);
            Console.WriteLine($"Piece captured: {capturedPiece.GetType()} {capturedPiece.GetColor()}");
        }
        public void HandleCastling(IPiece rook)
        {
            OnCastling?.Invoke(rook);
            Console.WriteLine($"Castling performed with rook: {rook.GetColor()}");
        }
        public void HandleEnPassant(IPiece capturedPawn) {
            OnEnPassant?.Invoke(capturedPawn);
            Console.WriteLine($"En passant performed, captured pawn: {capturedPawn.GetColor()}");

        }
        public void HandlePawnPromotion(IPiece pawn) {
            OnPawnPromotion?.Invoke(pawn);
            Console.WriteLine($"Pawn promoted: {pawn.GetColor()} {pawn.GetType()}");
        }
        public void HandleCheckmate()
        {
            OnCheckmate?.Invoke();
            Console.WriteLine("Checkmate! Game Over.");
        }
        public void HandleResign(int resignedPlayerIndex)
        {
            OnResign?.Invoke();
            Console.WriteLine($"Player {Players[resignedPlayerIndex].GetColor()} resigned. Game Over.");
            State = GameState.Resignation;
        }
        public void HandleStalemate()
        {
            OnStalemate?.Invoke();
            Console.WriteLine("Stalemate! Game Over.");
            State = GameState.Stalemate;
        }
    }
}