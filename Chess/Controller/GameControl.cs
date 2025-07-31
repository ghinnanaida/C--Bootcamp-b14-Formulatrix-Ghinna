using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;

namespace ChessGame.Controllers;

public class GameControl
{
    public List<IPlayer> Players{ get; private set;}
    public Dictionary<IPlayer, List<IPiece>> PlayerPieces{ get; private set;}
    public IBoard Board{ get; private set;}
    public GameState State{ get; private set;}

    private int _currentPlayerIndex;
    private ISquare? _intendedSquareSource;

    public List<ISquare>? CurrentLegalMoves{ get; private set;}
    public ISquare? LastMoveSource { get; private set; }
    public ISquare? LastMoveDestination { get; private set; }
    public IPiece? LastMovedPiece { get; private set; }

    public event Action? OnMoveDone;
    public event Action<IPiece>? OnCapturePiece;
    public event Action<IPiece, IPiece>? OnCastling;
    public event Action<IPiece>? OnEnPassant;
    public event Action<IPiece>? OnPawnPromotion;
    public event Action? OnCheckmate;
    public event Action? OnResign;
    public event Action? OnStalemate;
    public event Action? OnCheck;

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
        this.LastMoveSource = null;
        this.LastMoveDestination = null;
        this.LastMovedPiece = null;
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

    public IPlayer GetCurrentPlayer()
    {
        return Players[_currentPlayerIndex];
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
        this.CurrentLegalMoves = GetLegalMoves(this._intendedSquareSource);

    }

    public void CancelMove()
    {
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;
        this.State = GameState.IntendingMove;
    }
    
    public bool MakeMove(ISquare destinationSquare)
    {
        if (State != GameState.MakingMove || _intendedSquareSource == null)
        {
            Console.WriteLine("Invalid game state for making a move.");
            return false;
        }

        if (CurrentLegalMoves == null || !CurrentLegalMoves.Contains(destinationSquare))
        {
            Console.WriteLine("Invalid move. The selected destination is not a legal move for the chosen piece.");
            this._intendedSquareSource = null; 
            this.State = GameState.IntendingMove; 
            return false;
        }

        IPiece? pieceToMove = this._intendedSquareSource.GetPiece();
        ISquare sourceSquare = this._intendedSquareSource;

        if (pieceToMove == null)
        {
            Console.WriteLine("Error: No piece at the intended source square.");
            return false;
        }

        IPiece? capturedPiece = null; 

        bool isEnPassantMove = false;
        IPiece? enPassantCapturedPawn = null;
        ISquare? enPassantCapturedSquare = null;

        
        if (pieceToMove.GetPieceType() == PieceType.Pawn && destinationSquare.GetPiece() == null &&
            Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 1 && 
            LastMovedPiece != null && LastMovedPiece.GetPieceType() == PieceType.Pawn &&
            LastMoveDestination != null &&
            LastMoveDestination.GetPosition().X == destinationSquare.GetPosition().X && 
            LastMoveDestination.GetPosition().Y == sourceSquare.GetPosition().Y) 
        {
            
            isEnPassantMove = true;
            enPassantCapturedPawn = LastMovedPiece; 
            enPassantCapturedSquare = LastMoveDestination;
        }

        if (isEnPassantMove && enPassantCapturedPawn != null && enPassantCapturedSquare != null)
        {
            enPassantCapturedPawn.SetState(PieceState.Captured);
            enPassantCapturedSquare.SetPiece(null); 
            HandleEnPassant(enPassantCapturedPawn); 
            
            foreach (var playerEntry in PlayerPieces)
            {
                if (playerEntry.Key.GetColor() == enPassantCapturedPawn.GetColor())
                {
                    playerEntry.Value.Remove(enPassantCapturedPawn);
                    break;
                }
            }
        }
        else 
        {
            
            capturedPiece = destinationSquare.GetPiece(); 
            if (capturedPiece != null)
            {
                capturedPiece.SetState(PieceState.Captured);
                HandleCapturePiece(capturedPiece);
                
                foreach (var playerEntry in this.PlayerPieces)
                {
                    if (playerEntry.Key.GetColor() == capturedPiece.GetColor())
                    {
                        playerEntry.Value.Remove(capturedPiece);
                        break;
                    }
                }
            }
        }

        if (pieceToMove.GetPieceType() == PieceType.King &&
            Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 2)
        {
            // This is a castling move
            ISquare rookSourceSquare;
            ISquare rookDestSquare;
            IPiece? rookToMove;

            if (destinationSquare.GetPosition().X > sourceSquare.GetPosition().X) // King-side castling
            {
                rookSourceSquare = Board.GetSquare(new Point { X = 7, Y = sourceSquare.GetPosition().Y });
                rookDestSquare = Board.GetSquare(new Point { X = sourceSquare.GetPosition().X + 1, Y = sourceSquare.GetPosition().Y });
            }
            else // Queen-side castling
            {
                rookSourceSquare = Board.GetSquare(new Point { X = 0, Y = sourceSquare.GetPosition().Y });
                rookDestSquare = Board.GetSquare(new Point { X = sourceSquare.GetPosition().X - 1, Y = sourceSquare.GetPosition().Y });
            }

            rookToMove = rookSourceSquare.GetPiece();
            if (rookToMove != null)
            {
                rookSourceSquare.SetPiece(null);
                rookDestSquare.SetPiece(rookToMove);
                rookToMove.SetCurrentCoordinate(rookDestSquare.GetPosition());
                rookToMove.SetHasMoved(true);
                HandleCastling(pieceToMove, rookToMove); 
            }
        }

        // bool isPawnPromotion = false;
        if (pieceToMove.GetPieceType() == PieceType.Pawn)
        {
            int promotionRank = (pieceToMove.GetColor() == ColorType.White) ? 7 : 0; 

            if (destinationSquare.GetPosition().Y == promotionRank)
            {
                // isPawnPromotion = true;
                
                IPiece newPromotedPiece = new Piece(
                    pieceToMove.GetColor(),
                    PieceState.Active,
                    PieceType.Queen, // Default to Queen for now, will allow choice later
                    destinationSquare.GetPosition() 
                );

                var currentPlayerPieces = PlayerPieces.First(p => p.Key.GetColor() == pieceToMove.GetColor()).Value;
                currentPlayerPieces.Remove(pieceToMove); 
                currentPlayerPieces.Add(newPromotedPiece); 

                pieceToMove = newPromotedPiece;

                HandlePawnPromotion(newPromotedPiece); 
            }
        }
        
        this._intendedSquareSource.SetPiece(null); 
        destinationSquare.SetPiece(pieceToMove);
        pieceToMove.SetCurrentCoordinate(destinationSquare.GetPosition());
        pieceToMove.SetHasMoved(true);

        this.LastMoveSource = sourceSquare;
        this.LastMoveDestination = destinationSquare;
        this.LastMovedPiece = pieceToMove; 

        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;

        HandleMoveDone(); 


        var currentPlayer = Players[_currentPlayerIndex];
        if (capturedPiece != null || isEnPassantMove || pieceToMove.GetPieceType() == PieceType.Pawn)
        {
            currentPlayer.SetMoveCountNoCaptureNoPromotion(0); 
        }
        else
        {
            currentPlayer.SetMoveCountNoCaptureNoPromotion(currentPlayer.GetMoveCountNoCaptureNoPromotion() + 1);
        }
        NextTurn();

        return true;
    }

    public void NextTurn()
    {
        this._currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;

        this.State = GameState.IntendingMove;
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;

        // After changing turn, check if the current player's king is in check
        if (IsKingInCheck(GetCurrentPlayer().GetColor()))
        {
            State = GameState.Check;
            HandleCheck(); // Notify subscribers of check
            // Further logic to check for checkmate/stalemate will go here
        }
        else
        {
            // If not in check, check for stalemate
            if (GetAllPiecesLegalMoves(GetCurrentPlayer().GetColor()).Count == 0)
            {
                State = GameState.Stalemate;
                HandleStalemate(); // Notify subscribers of stalemate
            }
        }

    }

    public List<ISquare> GetPseudoLegalMoves(ISquare sourceSquare)
    {
        List<ISquare> pseudoLegalMoves = new List<ISquare>();
        var piece = sourceSquare.GetPiece();
        var position = sourceSquare.GetPosition();

        if (piece == null || piece.GetState() != PieceState.Active)
            return pseudoLegalMoves;

        var pieceType = piece.GetPieceType();
        var pieceColor = piece.GetColor();

        // Delegate to specific piece move generation methods
        return pieceType switch
        {
            PieceType.Pawn => GetPawnMoves(position, pieceColor),
            PieceType.Rook => GetRookMoves(position, pieceColor),
            PieceType.Knight => GetKnightMoves(position, pieceColor),
            PieceType.Bishop => GetBishopMoves(position, pieceColor),
            PieceType.Queen => GetQueenMoves(position, pieceColor),
            PieceType.King => GetKingMoves(position, pieceColor),
            _ => pseudoLegalMoves // Should not happen
        };
    }

    public List<ISquare> GetLegalMoves(ISquare sourceSquare)
    {
        List<ISquare> pseudoLegalMoves = GetPseudoLegalMoves(sourceSquare);
        List<ISquare> legalMoves = new List<ISquare>();
        var pieceToMove = sourceSquare.GetPiece();

        if (pieceToMove == null) return legalMoves;

        foreach (var destSquare in pseudoLegalMoves)
        {
            // Simulate the move on a temporary state to check for king safety
            ISquare originalSourceSquare = sourceSquare;
            ISquare originalDestSquare = destSquare;

            IPiece? originalPieceAtSource = originalSourceSquare.GetPiece();
            IPiece? originalPieceAtDest = originalDestSquare.GetPiece();

            // Store original coordinates and HasMoved state of the piece to restore later
            Point originalPieceCurrentCoord = originalPieceAtSource!.GetCurrentCoordinate();
            bool originalPieceHasMoved = originalPieceAtSource.GetHasMoved();


            // Temporarily make the move on the board model
            originalSourceSquare.SetPiece(null);
            originalDestSquare.SetPiece(originalPieceAtSource);
            originalPieceAtSource.SetCurrentCoordinate(originalDestSquare.GetPosition());
            originalPieceAtSource.SetHasMoved(true);

            // Simulate castling rook movement if it was a castling move
            IPiece? simulatedRook = null;
            ISquare? simulatedRookSource = null;
            ISquare? simulatedRookDest = null;
            Point originalRookCurrentCoord = new Point();
            bool originalRookHasMoved = false;

            if (pieceToMove.GetPieceType() == PieceType.King &&
                Math.Abs(sourceSquare.GetPosition().X - destSquare.GetPosition().X) == 2)
            {
                if (destSquare.GetPosition().X > sourceSquare.GetPosition().X) // King-side castling
                {
                    simulatedRookSource = Board.GetSquare(new Point { X = 7, Y = sourceSquare.GetPosition().Y });
                    simulatedRookDest = Board.GetSquare(new Point { X = sourceSquare.GetPosition().X + 1, Y = sourceSquare.GetPosition().Y });
                }
                else // Queen-side castling
                {
                    simulatedRookSource = Board.GetSquare(new Point { X = 0, Y = sourceSquare.GetPosition().Y });
                    simulatedRookDest = Board.GetSquare(new Point { X = sourceSquare.GetPosition().X - 1, Y = sourceSquare.GetPosition().Y });
                }

                simulatedRook = simulatedRookSource?.GetPiece();
                if (simulatedRook != null)
                {
                    originalRookCurrentCoord = simulatedRook.GetCurrentCoordinate();
                    originalRookHasMoved = simulatedRook.GetHasMoved();

                    simulatedRookSource!.SetPiece(null);
                    simulatedRookDest!.SetPiece(simulatedRook);
                    simulatedRook.SetCurrentCoordinate(simulatedRookDest.GetPosition());
                    simulatedRook.SetHasMoved(true);
                }
            }


            // Check if the current player's king is in check after the simulated move
            if (!IsKingInCheck(pieceToMove.GetColor()))
            {
                legalMoves.Add(destSquare);
            }

            // Undo the temporary move for the king
            originalSourceSquare.SetPiece(originalPieceAtSource);
            originalDestSquare.SetPiece(originalPieceAtDest); // Restore original piece at destination
            originalPieceAtSource.SetCurrentCoordinate(originalPieceCurrentCoord); // Restore original current coord
            originalPieceAtSource.SetHasMoved(originalPieceHasMoved); // Restore original HasMoved state

            // Undo the temporary move for the rook if castling was simulated
            if (simulatedRook != null && simulatedRookSource != null && simulatedRookDest != null)
            {
                simulatedRookDest.SetPiece(null); // Clear rook from its temporary destination
                simulatedRookSource.SetPiece(simulatedRook); // Restore rook to its original source
                simulatedRook.SetCurrentCoordinate(originalRookCurrentCoord); // Restore original current coord
                simulatedRook.SetHasMoved(originalRookHasMoved); // Restore original HasMoved state
            }
        }
        return legalMoves;
    }

    public List<ISquare> GetAllPiecesLegalMoves(ColorType color)
    {
        List<ISquare> allLegalMoves = new List<ISquare>();
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == color)
            {
                foreach (var piece in playerEntry.Value)
                {
                    if (piece.GetState() == PieceState.Active)
                    {
                        ISquare pieceSquare = Board.GetSquare(piece.GetCurrentCoordinate());
                        allLegalMoves.AddRange(GetLegalMoves(pieceSquare));
                    }
                }
                break; // Found the player, no need to check other entries
            }
        }
        return allLegalMoves;
    }

    public bool IsSquareAttacked(ISquare targetSquare, ColorType attackingColor)
    {
        // Iterate through all pieces of the attacking color
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == attackingColor)
            {
                foreach (var attackingPiece in playerEntry.Value)
                {
                    if (attackingPiece.GetState() != PieceState.Active) continue;

                    // Handle pawn attacks specifically as their "pseudo-legal" moves
                    // often include forward moves that aren't attacks, but diagonals are attacks.
                    if (attackingPiece.GetPieceType() == PieceType.Pawn)
                    {
                        var pawnPos = attackingPiece.GetCurrentCoordinate();
                        int direction = (attackingPiece.GetColor() == ColorType.White) ? 1 : -1;

                        Point attackLeft = new Point { X = pawnPos.X - 1, Y = pawnPos.Y + direction };
                        Point attackRight = new Point { X = pawnPos.X + 1, Y = pawnPos.Y + direction };

                        if ((targetSquare.GetPosition().Equals(attackLeft) && attackLeft.IsValid) ||
                            (targetSquare.GetPosition().Equals(attackRight) && attackRight.IsValid))
                        {
                            return true;
                        }
                        continue; // Skip GetPseudoLegalMoves for pawns, as we handled their attacks
                    }

                    // For other pieces, check their pseudo-legal moves (which include captures)
                    List<ISquare> attackingMoves = GetPseudoLegalMoves(Board.GetSquare(attackingPiece.GetCurrentCoordinate()));

                    if (attackingMoves.Contains(targetSquare))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsKingInCheck(ColorType kingColor)
    {
        ISquare? kingSquare = null; // Can be null if king is not found
        // Find the king's current position
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == kingColor)
            {
                foreach (var piece in playerEntry.Value)
                {
                    if (piece.GetPieceType() == PieceType.King && piece.GetState() == PieceState.Active)
                    {
                        kingSquare = Board.GetSquare(piece.GetCurrentCoordinate());
                        break;
                    }
                }
            }
            if (kingSquare != null) break;
        }

        if (kingSquare == null)
        {
            // This should ideally not happen in a valid game state unless the king has been captured
            // which would mean the game is over.
            return false;
        }

        // Determine the opposing color
        ColorType opposingColor = (kingColor == ColorType.White) ? ColorType.Black : ColorType.White;

        // Check if the king's square is attacked by any opposing piece
        return IsSquareAttacked(kingSquare, opposingColor);
    }

    public List<ISquare> GetPawnMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
        int direction = pieceColor == ColorType.White ? 1 : -1;
        int startY = pieceColor == ColorType.White ? 1 : 6;
        int enPassantRank = pieceColor == ColorType.White ? 4 : 3; // Rank where En Passant capture happens

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
                    var twoStep = new Point { X = position.X, Y = position.Y + 2 * direction };
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

        // Check for En Passant opportunity
        if (position.Y == enPassantRank && LastMovedPiece != null &&
            LastMovedPiece.GetPieceType() == PieceType.Pawn &&
            LastMovedPiece.GetColor() != pieceColor &&
            LastMoveSource != null && LastMoveDestination != null)
        {
            // Check if the last moved piece was a pawn that just moved two squares
            // AND landed directly to the left or right of the current pawn.
            Point lastPawnSource = LastMoveSource.GetPosition();
            Point lastPawnDest = LastMoveDestination.GetPosition();

            bool isTwoSquareMove = Math.Abs(lastPawnDest.Y - lastPawnSource.Y) == 2;
            bool landedAdjacent = lastPawnDest.Y == position.Y && (lastPawnDest.X == position.X - 1 || lastPawnDest.X == position.X + 1);

            if (isTwoSquareMove && landedAdjacent)
            {
                // The square the current pawn moves *to* for en passant is behind the captured pawn
                Point enPassantTargetSquare = new Point { X = lastPawnDest.X, Y = position.Y + direction };
                if (enPassantTargetSquare.IsValid)
                {
                    // Ensure the target square for the moving pawn is empty (it should be)
                    if (Board.GetSquare(enPassantTargetSquare).GetPiece() == null)
                    {
                        legalMoves.Add(Board.GetSquare(enPassantTargetSquare));
                    }
                }
            }
        }

        return legalMoves;
    }

    public List<ISquare> GetRookMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
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

        return legalMoves;
    }

    public List<ISquare> GetKnightMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
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
        return legalMoves;
    }

    public List<ISquare> GetBishopMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
        // Diagonal directions
        Point[] directions = { new Point { X = 1, Y = 1 }, new Point { X = 1, Y = -1 }, new Point { X = -1, Y = 1 }, new Point { X = -1, Y = -1 } };

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
        return legalMoves;
    }

    public List<ISquare> GetQueenMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
        // Queen combines rook and bishop moves
        // Horizontal and vertical directions (like rook)
        Point[] rookDirections = { new Point { X = 0, Y = 1 }, new Point { X = 0, Y = -1 }, new Point { X = 1, Y = 0 }, new Point { X = -1, Y = 0 } };

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
        Point[] bishopDirections = { new Point { X = 1, Y = 1 }, new Point { X = 1, Y = -1 }, new Point { X = -1, Y = 1 }, new Point { X = -1, Y = -1 } };

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
        return legalMoves;
    }

    public List<ISquare> GetKingMoves(Point position, ColorType pieceColor)
    {
        List<ISquare> legalMoves = new List<ISquare>();
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
        return legalMoves;
    }

    public void HandleMoveDone()
    {
        OnMoveDone?.Invoke();
        Console.WriteLine("Move done.");
    }
    
    public void HandleCapturePiece(IPiece capturedPiece)
    {
        OnCapturePiece?.Invoke(capturedPiece);
        Console.WriteLine($"Piece captured: {capturedPiece.GetPieceType()} {capturedPiece.GetColor()}");
    }

    public void HandleCastling(IPiece king, IPiece rook)
    {
        OnCastling?.Invoke(king, rook);
        Console.WriteLine($"Castling performed with king {king.GetColor()} and rook {rook.GetColor()}");
    }
    
    public void HandleEnPassant(IPiece capturedPawn)
    {
        OnEnPassant?.Invoke(capturedPawn);
        Console.WriteLine($"En passant performed, captured pawn: {capturedPawn.GetColor()}");
    }
    
    public void HandlePawnPromotion(IPiece pawn)
    {
        OnPawnPromotion?.Invoke(pawn);
        Console.WriteLine($"Pawn promoted: {pawn.GetColor()} {pawn.GetPieceType()}");
    }

    public void HandleCheck()
    {
        OnCheck?.Invoke();
        Console.WriteLine($"Check! {GetCurrentPlayer().GetColor()} king is in check.");
    }

    public void HandleCheckmate()
    {
        OnCheckmate?.Invoke();
        Console.WriteLine("Checkmate! Game Over.");
        State = GetCurrentPlayer().GetColor() == ColorType.White ? GameState.CheckmateBlackWin : GameState.CheckmateWhiteWin;
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