using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;

namespace ChessGame.Controllers;

public class GameControl
{
    // Constants for better maintainability
    private const int BOARD_SIZE = 8;
    private const int FIFTY_MOVE_RULE_LIMIT = 100; // 50 moves = 100 half-moves
    
    // Direction arrays for piece movement (cached for performance)
    private static readonly Point[] RookDirections = { 
        new Point { X = 0, Y = 1 }, new Point { X = 0, Y = -1 }, 
        new Point { X = 1, Y = 0 }, new Point { X = -1, Y = 0 } 
    };
    
    private static readonly Point[] BishopDirections = { 
        new Point { X = 1, Y = 1 }, new Point { X = 1, Y = -1 }, 
        new Point { X = -1, Y = 1 }, new Point { X = -1, Y = -1 } 
    };
    
    private static readonly Point[] KingDirections = {
        new Point{ X = 0, Y = 1}, new Point{X = 0, Y = -1}, new Point{ X = 1, Y = 0 }, new Point{ X = -1, Y = 0},
        new Point{ X = 1, Y = 1}, new Point{X = 1, Y = -1}, new Point{ X = -1, Y = 1 }, new Point{ X = -1, Y = -1}
    };
    
    private static readonly Point[] KnightMoves = {
        new Point{ X = 2, Y = 1}, new Point{ X = 2, Y = -1}, new Point{ X = -2, Y = 1}, new Point{ X = -2, Y = -1},
        new Point{ X = 1, Y = 2}, new Point{ X = 1, Y = -2}, new Point{ X = -1, Y = 2}, new Point{ X = -1, Y = -2}
    };

    public List<IPlayer> Players { get; private set; }
    public Dictionary<IPlayer, List<IPiece>> PlayerPieces { get; private set; }
    public IBoard Board { get; private set; }
    public GameState State { get; private set; }

    private int _currentPlayerIndex;
    private ISquare? _intendedSquareSource;

    public List<ISquare>? CurrentLegalMoves { get; private set; }
    public ISquare? LastMoveSource { get; private set; }
    public ISquare? LastMoveDestination { get; private set; }
    public IPiece? LastMovedPiece { get; private set; }

    public event Action? OnMoveDone;
    public event Action<IPiece>? OnCapturePiece;
    public event Action<IPiece, IPiece>? OnCastling;
    public event Action<IPiece>? OnEnPassant;
    public event Action<IPiece>? OnPawnPromotion;
    public event Action? OnCheckmate;
    public event Action<ColorType>? OnResign;
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
        ResetGameState();
    }

    private void ResetGameState()
    {
        this.State = GameState.Init;
        this._currentPlayerIndex = 0;
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;
        this.LastMoveSource = null;
        this.LastMoveDestination = null;
        this.LastMovedPiece = null;
    }

    // Moved from Point struct to GameControl for better encapsulation
    public bool IsValidCoordinate(Point coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < BOARD_SIZE && coordinate.Y >= 0 && coordinate.Y < BOARD_SIZE;
    }

    public void InitGame()
    {
        // Clear existing pieces
        foreach (var playerPieces in PlayerPieces.Values)
        {
            playerPieces.Clear();
        }

        InitializePawns();
        InitializeBackRank(ColorType.White, 0);
        InitializeBackRank(ColorType.Black, 7);

        this.State = GameState.IntendingMove;
        this._currentPlayerIndex = 0;
    }

    private void InitializePawns()
    {
        int whitePawnRank = 1;
        int blackPawnRank = 6;

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            // White pawns
            var whitePawn = new Piece(ColorType.White, PieceState.Active, PieceType.Pawn, new Point { X = i, Y = whitePawnRank });
            this.Board.SetSquare(new Point { X = i, Y = whitePawnRank }, whitePawn);
            PlayerPieces[Players[0]].Add(whitePawn);

            // Black pawns
            var blackPawn = new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn, new Point { X = i, Y = blackPawnRank });
            this.Board.SetSquare(new Point { X = i, Y = blackPawnRank }, blackPawn);
            PlayerPieces[Players[1]].Add(blackPawn);
        }
    }

    private void InitializeBackRank(ColorType color, int rank)
    {
        var player = Players.First(p => p.GetColor() == color);
        var pieceOrder = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            var piece = new Piece(color, PieceState.Active, pieceOrder[x], new Point { X = x, Y = rank });
            this.Board.SetSquare(new Point { X = x, Y = rank }, piece);
            PlayerPieces[player].Add(piece);
        }
    }

    public IPlayer GetCurrentPlayer()
    {
        return Players[_currentPlayerIndex];
    }

    public void IntendMove(ISquare sourceSquare)
    {
        if (!CanIntendMove())
        {
            Console.WriteLine("Cannot intend move in current game state.");
            return;
        }

        var piece = sourceSquare.GetPiece();
        if (!IsValidPieceSelection(piece))
        {
            Console.WriteLine("No piece or not your piece in selected square!");
            ResetMoveIntention();
            return;
        }

        this._intendedSquareSource = sourceSquare;
        this.State = GameState.MakingMove;
        this.CurrentLegalMoves = GetLegalMoves(this._intendedSquareSource);
    }

    private bool CanIntendMove()
    {
        return State == GameState.IntendingMove || State == GameState.Check;
    }

    private bool IsValidPieceSelection(IPiece? piece)
    {
        return piece != null && piece.GetColor() == Players[_currentPlayerIndex].GetColor();
    }

    private void ResetMoveIntention()
    {
        this._intendedSquareSource = null;
        this.State = GameState.IntendingMove;
    }

    public void CancelMove()
    {
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;
        this.State = GameState.IntendingMove;
    }

    public PieceType GetPromotionChoice(ColorType color)
    {
        Console.WriteLine($"{color} pawn reached the end! Choose promotion:");
        Console.WriteLine("1. Queen");
        Console.WriteLine("2. Rook");
        Console.WriteLine("3. Bishop");
        Console.WriteLine("4. Knight");
        Console.Write("Enter your choice (1-4): ");

        while (true)
        {
            string? input = Console.ReadLine();
            switch (input)
            {
                case "1": return PieceType.Queen;
                case "2": return PieceType.Rook;
                case "3": return PieceType.Bishop;
                case "4": return PieceType.Knight;
                default:
                    Console.Write("Invalid choice. Please enter 1-4: ");
                    break;
            }
        }
    }

    public bool MakeMove(ISquare destinationSquare)
    {
        if (!ValidateMoveAttempt(destinationSquare))
        {
            return false;
        }

        IPiece pieceToMove = this._intendedSquareSource!.GetPiece()!;
        ISquare sourceSquare = this._intendedSquareSource;

        // Handle special moves
        HandleSpecialMoves(pieceToMove, sourceSquare, destinationSquare);

        // Move the piece
        MovePiece(sourceSquare, destinationSquare, pieceToMove);

        // Update game state
        UpdateMoveHistory(sourceSquare, destinationSquare, pieceToMove);
        UpdateFiftyMoveRule(pieceToMove, destinationSquare.GetPiece() != null);

        HandleMoveDone();
        NextTurn();

        return true;
    }

    private bool ValidateMoveAttempt(ISquare destinationSquare)
    {
        if (State != GameState.MakingMove || _intendedSquareSource == null)
        {
            Console.WriteLine("Invalid game state for making a move.");
            return false;
        }

        if (CurrentLegalMoves == null || !CurrentLegalMoves.Contains(destinationSquare))
        {
            Console.WriteLine("Invalid move. The selected destination is not a legal move for the chosen piece.");
            ResetMoveIntention();
            return false;
        }

        var pieceToMove = this._intendedSquareSource.GetPiece();
        if (pieceToMove == null)
        {
            Console.WriteLine("Error: No piece at the intended source square.");
            return false;
        }

        return true;
    }

    private void HandleSpecialMoves(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        // Handle En Passant
        if (IsEnPassantMove(pieceToMove, sourceSquare, destinationSquare))
        {
            HandleEnPassantCapture();
        }
        // Handle regular capture
        else
        {
            HandleRegularCapture(destinationSquare);
        }

        // Handle Castling
        if (IsCastlingMove(pieceToMove, sourceSquare, destinationSquare))
        {
            HandleCastlingMove(sourceSquare, destinationSquare);
        }

        // Handle Pawn Promotion
        if (IsPawnPromotion(pieceToMove, destinationSquare))
        {
            HandlePawnPromotionMove(pieceToMove, destinationSquare);
        }
    }

    private bool IsEnPassantMove(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        return pieceToMove.GetPieceType() == PieceType.Pawn &&
               destinationSquare.GetPiece() == null &&
               Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 1 &&
               LastMovedPiece != null &&
               LastMovedPiece.GetPieceType() == PieceType.Pawn &&
               LastMoveDestination != null &&
               LastMoveDestination.GetPosition().X == destinationSquare.GetPosition().X &&
               LastMoveDestination.GetPosition().Y == sourceSquare.GetPosition().Y;
    }

    private void HandleEnPassantCapture()
    {
        if (LastMovedPiece != null && LastMoveDestination != null)
        {
            LastMovedPiece.SetState(PieceState.Captured);
            LastMoveDestination.SetPiece(null);
            RemovePieceFromPlayer(LastMovedPiece);
            HandleEnPassant(LastMovedPiece);
        }
    }

    private void HandleRegularCapture(ISquare destinationSquare)
    {
        var capturedPiece = destinationSquare.GetPiece();
        if (capturedPiece != null)
        {
            capturedPiece.SetState(PieceState.Captured);
            RemovePieceFromPlayer(capturedPiece);
            HandleCapturePiece(capturedPiece);
        }
    }

    private void RemovePieceFromPlayer(IPiece piece)
    {
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == piece.GetColor())
            {
                playerEntry.Value.Remove(piece);
                break;
            }
        }
    }

    private bool IsCastlingMove(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        return pieceToMove.GetPieceType() == PieceType.King &&
               Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 2;
    }

    private void HandleCastlingMove(ISquare sourceSquare, ISquare destinationSquare)
    {
        bool isKingSide = destinationSquare.GetPosition().X > sourceSquare.GetPosition().X;
        int rookSourceX = isKingSide ? 7 : 0;
        int rookDestX = isKingSide ? sourceSquare.GetPosition().X + 1 : sourceSquare.GetPosition().X - 1;

        var rookSourceSquare = Board.GetSquare(new Point { X = rookSourceX, Y = sourceSquare.GetPosition().Y });
        var rookDestSquare = Board.GetSquare(new Point { X = rookDestX, Y = sourceSquare.GetPosition().Y });

        var rook = rookSourceSquare.GetPiece();
        if (rook != null)
        {
            rookSourceSquare.SetPiece(null);
            rookDestSquare.SetPiece(rook);
            rook.SetCurrentCoordinate(rookDestSquare.GetPosition());
            rook.SetHasMoved(true);
            HandleCastling(this._intendedSquareSource!.GetPiece()!, rook);
        }
    }

    private bool IsPawnPromotion(IPiece pieceToMove, ISquare destinationSquare)
    {
        int promotionRank = pieceToMove.GetColor() == ColorType.White ? 7 : 0;
        return pieceToMove.GetPieceType() == PieceType.Pawn &&
               destinationSquare.GetPosition().Y == promotionRank;
    }

    private IPiece HandlePawnPromotionMove(IPiece pieceToMove, ISquare destinationSquare)
    {
        var promotionType = GetPromotionChoice(pieceToMove.GetColor());
        var promotedPiece = new Piece(
            pieceToMove.GetColor(),
            PieceState.Active,
            promotionType,
            destinationSquare.GetPosition()
        );

        var currentPlayerPieces = PlayerPieces.First(p => p.Key.GetColor() == pieceToMove.GetColor()).Value;
        currentPlayerPieces.Remove(pieceToMove);
        currentPlayerPieces.Add(promotedPiece);

        HandlePawnPromotion(promotedPiece);
        return promotedPiece;
    }

    private void MovePiece(ISquare sourceSquare, ISquare destinationSquare, IPiece pieceToMove)
    {
        sourceSquare.SetPiece(null);
        destinationSquare.SetPiece(pieceToMove);
        pieceToMove.SetCurrentCoordinate(destinationSquare.GetPosition());
        pieceToMove.SetHasMoved(true);
    }

    private void UpdateMoveHistory(ISquare sourceSquare, ISquare destinationSquare, IPiece pieceToMove)
    {
        this.LastMoveSource = sourceSquare;
        this.LastMoveDestination = destinationSquare;
        this.LastMovedPiece = pieceToMove;
    }

    private void UpdateFiftyMoveRule(IPiece pieceToMove, bool isCapture)
    {
        var currentPlayer = Players[_currentPlayerIndex];
        if (isCapture || pieceToMove.GetPieceType() == PieceType.Pawn)
        {
            currentPlayer.SetMoveCountNoCaptureNoPromotion(0);
        }
        else
        {
            currentPlayer.SetMoveCountNoCaptureNoPromotion(currentPlayer.GetMoveCountNoCaptureNoPromotion() + 1);
        }
    }

    public void NextTurn()
    {
        this._currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        ResetTurnState();
        
        var currentPlayer = GetCurrentPlayer();
        var currentPlayerColor = currentPlayer.GetColor();
        
        bool isInCheck = IsKingInCheck(currentPlayerColor);
        var allLegalMoves = GetAllPiecesLegalMoves(currentPlayerColor);
        
        UpdateGameState(isInCheck, allLegalMoves.Count, currentPlayer);
    }

    private void ResetTurnState()
    {
        this.State = GameState.IntendingMove;
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;
    }

    private void UpdateGameState(bool isInCheck, int legalMovesCount, IPlayer currentPlayer)
    {
        if (isInCheck)
        {
            if (legalMovesCount == 0)
            {
                var currentPlayerColor = currentPlayer.GetColor();
                this.State = currentPlayerColor == ColorType.White ? GameState.CheckmateBlackWin : GameState.CheckmateWhiteWin;
                HandleCheckmate();
            }
            else
            {
                this.State = GameState.Check;
                HandleCheck();
            }
        }
        else
        {
            if (legalMovesCount == 0)
            {
                this.State = GameState.Stalemate;
                HandleStalemate();
            }
            else if (currentPlayer.GetMoveCountNoCaptureNoPromotion() >= FIFTY_MOVE_RULE_LIMIT)
            {
                this.State = GameState.FiftyMoveDraw;
                HandleStalemate();
            }
        }
    }

    public List<ISquare> GetPseudoLegalMoves(ISquare sourceSquare)
    {
        var piece = sourceSquare.GetPiece();
        if (piece == null || piece.GetState() != PieceState.Active)
            return new List<ISquare>();

        var position = sourceSquare.GetPosition();
        var pieceType = piece.GetPieceType();
        var pieceColor = piece.GetColor();

        return pieceType switch
        {
            PieceType.Pawn => GetPawnMoves(position, pieceColor),
            PieceType.Rook => GetSlidingMoves(position, pieceColor, RookDirections),
            PieceType.Knight => GetKnightMoves(position, pieceColor),
            PieceType.Bishop => GetSlidingMoves(position, pieceColor, BishopDirections),
            PieceType.Queen => GetQueenMoves(position, pieceColor),
            PieceType.King => GetKingMoves(position, pieceColor),
            _ => new List<ISquare>()
        };
    }

    // Consolidated sliding piece movement (Rook, Bishop, Queen)
    private List<ISquare> GetSlidingMoves(Point position, ColorType pieceColor, Point[] directions)
    {
        var legalMoves = new List<ISquare>();

        foreach (var direction in directions)
        {
            for (int i = 1; i < BOARD_SIZE; i++)
            {
                var newPos = new Point { X = position.X + i * direction.X, Y = position.Y + i * direction.Y };

                if (!IsValidCoordinate(newPos)) break;

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
                        legalMoves.Add(square);
                    }
                    break;
                }
            }
        }

        return legalMoves;
    }

    // Optimized Queen moves using both rook and bishop directions
    public List<ISquare> GetQueenMoves(Point position, ColorType pieceColor)
    {
        var rookMoves = GetSlidingMoves(position, pieceColor, RookDirections);
        var bishopMoves = GetSlidingMoves(position, pieceColor, BishopDirections);
        rookMoves.AddRange(bishopMoves);
        return rookMoves;
    }

    public List<ISquare> GetKnightMoves(Point position, ColorType pieceColor)
    {
        var legalMoves = new List<ISquare>();

        foreach (var move in KnightMoves)
        {
            var newPos = new Point { X = position.X + move.X, Y = position.Y + move.Y };

            if (IsValidCoordinate(newPos))
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

    public List<ISquare> GetPawnMoves(Point position, ColorType pieceColor)
    {
        var legalMoves = new List<ISquare>();
        int direction = pieceColor == ColorType.White ? 1 : -1;
        int startY = pieceColor == ColorType.White ? 1 : 6;
        int enPassantRank = pieceColor == ColorType.White ? 4 : 3;

        // Forward moves
        AddPawnForwardMoves(legalMoves, position, direction, startY);
        
        // Capture moves
        AddPawnCaptureMoves(legalMoves, position, direction, pieceColor);
        
        // En passant
        AddEnPassantMoves(legalMoves, position, direction, pieceColor, enPassantRank);

        return legalMoves;
    }

    private void AddPawnForwardMoves(List<ISquare> legalMoves, Point position, int direction, int startY)
    {
        var oneStep = new Point { X = position.X, Y = position.Y + direction };
        if (IsValidCoordinate(oneStep))
        {
            var oneStepSquare = this.Board.GetSquare(oneStep);
            if (oneStepSquare.GetPiece() == null)
            {
                legalMoves.Add(oneStepSquare);

                // Two-step move from starting position
                if (position.Y == startY)
                {
                    var twoStep = new Point { X = position.X, Y = position.Y + 2 * direction };
                    if (IsValidCoordinate(twoStep))
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
    }

    private void AddPawnCaptureMoves(List<ISquare> legalMoves, Point position, int direction, ColorType pieceColor)
    {
        var capturePositions = new Point[]
        {
            new Point { X = position.X - 1, Y = position.Y + direction },
            new Point { X = position.X + 1, Y = position.Y + direction }
        };

        foreach (var capturePos in capturePositions)
        {
            if (IsValidCoordinate(capturePos))
            {
                var square = this.Board.GetSquare(capturePos);
                var target = square.GetPiece();
                if (target != null && target.GetColor() != pieceColor && target.GetState() == PieceState.Active)
                {
                    legalMoves.Add(square);
                }
            }
        }
    }

    private void AddEnPassantMoves(List<ISquare> legalMoves, Point position, int direction, ColorType pieceColor, int enPassantRank)
    {
        if (position.Y != enPassantRank || LastMovedPiece == null || LastMoveSource == null || LastMoveDestination == null)
            return;

        if (LastMovedPiece.GetPieceType() != PieceType.Pawn || LastMovedPiece.GetColor() == pieceColor)
            return;

        var lastPawnSource = LastMoveSource.GetPosition();
        var lastPawnDest = LastMoveDestination.GetPosition();

        bool isTwoSquareMove = Math.Abs(lastPawnDest.Y - lastPawnSource.Y) == 2;
        bool landedAdjacent = lastPawnDest.Y == position.Y && Math.Abs(lastPawnDest.X - position.X) == 1;

        if (isTwoSquareMove && landedAdjacent)
        {
            var enPassantTarget = new Point { X = lastPawnDest.X, Y = position.Y + direction };
            if (IsValidCoordinate(enPassantTarget) && Board.GetSquare(enPassantTarget).GetPiece() == null)
            {
                legalMoves.Add(Board.GetSquare(enPassantTarget));
            }
        }
    }

    public List<ISquare> GetKingMoves(Point position, ColorType pieceColor)
    {
        var legalMoves = new List<ISquare>();

        // Regular king moves
        foreach (var direction in KingDirections)
        {
            var newPos = new Point { X = position.X + direction.X, Y = position.Y + direction.Y };

            if (IsValidCoordinate(newPos))
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

        // Castling moves
        AddCastlingMoves(legalMoves, position, pieceColor);

        return legalMoves;
    }

    private void AddCastlingMoves(List<ISquare> legalMoves, Point position, ColorType pieceColor)
    {
        var king = Board.GetSquare(position).GetPiece();
        if (king == null || king.GetHasMoved() || IsKingInCheck(pieceColor))
            return;

        // Kingside castling
        if (CanCastleKingSide(position, pieceColor))
        {
            legalMoves.Add(Board.GetSquare(new Point { X = position.X + 2, Y = position.Y }));
        }

        // Queenside castling
        if (CanCastleQueenSide(position, pieceColor))
        {
            legalMoves.Add(Board.GetSquare(new Point { X = position.X - 2, Y = position.Y }));
        }
    }

    private bool CanCastleKingSide(Point kingPosition, ColorType pieceColor)
    {
        var kingsideRook = Board.GetSquare(new Point { X = 7, Y = kingPosition.Y }).GetPiece();
        if (kingsideRook == null || kingsideRook.GetPieceType() != PieceType.Rook || 
            kingsideRook.GetHasMoved() || kingsideRook.GetColor() != pieceColor)
            return false;

        // Check path is clear
        for (int x = kingPosition.X + 1; x < 7; x++)
        {
            if (Board.GetSquare(new Point { X = x, Y = kingPosition.Y }).GetPiece() != null)
                return false;
        }

        // Check king doesn't pass through or land on attacked squares
        var opposingColor = pieceColor == ColorType.White ? ColorType.Black : ColorType.White;
        for (int x = kingPosition.X + 1; x <= kingPosition.X + 2; x++)
        {
            if (IsSquareAttacked(Board.GetSquare(new Point { X = x, Y = kingPosition.Y }), opposingColor))
                return false;
        }

        return true;
    }

    private bool CanCastleQueenSide(Point kingPosition, ColorType pieceColor)
    {
        var queensideRook = Board.GetSquare(new Point { X = 0, Y = kingPosition.Y }).GetPiece();
        if (queensideRook == null || queensideRook.GetPieceType() != PieceType.Rook || 
            queensideRook.GetHasMoved() || queensideRook.GetColor() != pieceColor)
            return false;

        // Check path is clear
        for (int x = 1; x < kingPosition.X; x++)
        {
            if (Board.GetSquare(new Point { X = x, Y = kingPosition.Y }).GetPiece() != null)
                return false;
        }

        // Check king doesn't pass through or land on attacked squares
        var opposingColor = pieceColor == ColorType.White ? ColorType.Black : ColorType.White;
        for (int x = kingPosition.X - 1; x >= kingPosition.X - 2; x--)
        {
            if (IsSquareAttacked(Board.GetSquare(new Point { X = x, Y = kingPosition.Y }), opposingColor))
                return false;
        }

        return true;
    }

    public List<ISquare> GetLegalMoves(ISquare sourceSquare)
    {
        var pseudoLegalMoves = GetPseudoLegalMoves(sourceSquare);
        var legalMoves = new List<ISquare>();
        var pieceToMove = sourceSquare.GetPiece();

        if (pieceToMove == null) return legalMoves;

        foreach (var destSquare in pseudoLegalMoves)
        {
            if (IsMoveLegal(sourceSquare, destSquare, pieceToMove))
            {
                legalMoves.Add(destSquare);
            }
        }

        return legalMoves;
    }

    private bool IsMoveLegal(ISquare sourceSquare, ISquare destSquare, IPiece pieceToMove)
    {
        // Save original state
        var gameState = SaveGameState(sourceSquare, destSquare);
        
        // Make temporary move
        MakeTemporaryMove(sourceSquare, destSquare, pieceToMove);
        
        // Check if king is in check after the move
        bool isLegal = !IsKingInCheck(pieceToMove.GetColor());
        
        // Restore original state
        RestoreGameState(gameState);
        
        return isLegal;
    }

    private GameStateSnapshot SaveGameState(ISquare sourceSquare, ISquare destSquare)
    {
        var originalPieceAtSource = sourceSquare.GetPiece();
        var originalPieceAtDest = destSquare.GetPiece();
        
        return new GameStateSnapshot
        {
            SourceSquare = sourceSquare,
            DestSquare = destSquare,
            OriginalPieceAtSource = originalPieceAtSource,
            OriginalPieceAtDest = originalPieceAtDest,
            OriginalPieceCurrentCoord = originalPieceAtSource?.GetCurrentCoordinate() ?? new Point(),
            OriginalPieceHasMoved = originalPieceAtSource?.GetHasMoved() ?? false
        };
    }

    private void MakeTemporaryMove(ISquare sourceSquare, ISquare destSquare, IPiece pieceToMove)
    {
        sourceSquare.SetPiece(null);
        destSquare.SetPiece(pieceToMove);
        pieceToMove.SetCurrentCoordinate(destSquare.GetPosition());
        pieceToMove.SetHasMoved(true);

        // Handle castling simulation
        if (IsCastlingMove(pieceToMove, sourceSquare, destSquare))
        {
            SimulateCastlingMove(sourceSquare, destSquare);
        }
    }

    private void SimulateCastlingMove(ISquare sourceSquare, ISquare destSquare)
    {
        bool isKingSide = destSquare.GetPosition().X > sourceSquare.GetPosition().X;
        int rookSourceX = isKingSide ? 7 : 0;
        int rookDestX = isKingSide ? sourceSquare.GetPosition().X + 1 : sourceSquare.GetPosition().X - 1;

        var rookSourceSquare = Board.GetSquare(new Point { X = rookSourceX, Y = sourceSquare.GetPosition().Y });
        var rookDestSquare = Board.GetSquare(new Point { X = rookDestX, Y = sourceSquare.GetPosition().Y });

        var rook = rookSourceSquare.GetPiece();
        if (rook != null)
        {
            rookSourceSquare.SetPiece(null);
            rookDestSquare.SetPiece(rook);
            rook.SetCurrentCoordinate(rookDestSquare.GetPosition());
            rook.SetHasMoved(true);
        }
    }

    private void RestoreGameState(GameStateSnapshot snapshot)
    {
        snapshot.SourceSquare.SetPiece(snapshot.OriginalPieceAtSource);
        snapshot.DestSquare.SetPiece(snapshot.OriginalPieceAtDest);
        
        if (snapshot.OriginalPieceAtSource != null)
        {
            snapshot.OriginalPieceAtSource.SetCurrentCoordinate(snapshot.OriginalPieceCurrentCoord);
            snapshot.OriginalPieceAtSource.SetHasMoved(snapshot.OriginalPieceHasMoved);
        }

        // Restore castling if it was simulated
        RestoreCastlingSimulation(snapshot);
    }

    private void RestoreCastlingSimulation(GameStateSnapshot snapshot)
    {
        if (snapshot.OriginalPieceAtSource?.GetPieceType() == PieceType.King &&
            Math.Abs(snapshot.SourceSquare.GetPosition().X - snapshot.DestSquare.GetPosition().X) == 2)
        {
            bool isKingSide = snapshot.DestSquare.GetPosition().X > snapshot.SourceSquare.GetPosition().X;
            int rookSourceX = isKingSide ? 7 : 0;
            int rookDestX = isKingSide ? snapshot.SourceSquare.GetPosition().X + 1 : snapshot.SourceSquare.GetPosition().X - 1;

            var rookSourceSquare = Board.GetSquare(new Point { X = rookSourceX, Y = snapshot.SourceSquare.GetPosition().Y });
            var rookDestSquare = Board.GetSquare(new Point { X = rookDestX, Y = snapshot.SourceSquare.GetPosition().Y });

            var rook = rookDestSquare.GetPiece();
            if (rook != null)
            {
                rookDestSquare.SetPiece(null);
                rookSourceSquare.SetPiece(rook);
                rook.SetCurrentCoordinate(rookSourceSquare.GetPosition());
                rook.SetHasMoved(false); // Assume rook hasn't moved for castling to be possible
            }
        }
    }

    public List<ISquare> GetAllPiecesLegalMoves(ColorType color)
    {
        var allLegalMoves = new List<ISquare>();
        
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == color)
            {
                foreach (var piece in playerEntry.Value)
                {
                    if (piece.GetState() == PieceState.Active)
                    {
                        var pieceSquare = Board.GetSquare(piece.GetCurrentCoordinate());
                        allLegalMoves.AddRange(GetLegalMoves(pieceSquare));
                    }
                }
                break;
            }
        }
        
        return allLegalMoves;
    }

    public bool IsSquareAttacked(ISquare targetSquare, ColorType attackingColor)
    {
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == attackingColor)
            {
                foreach (var attackingPiece in playerEntry.Value)
                {
                    if (attackingPiece.GetState() != PieceState.Active) continue;

                    if (CanPieceAttackSquare(attackingPiece, targetSquare))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CanPieceAttackSquare(IPiece attackingPiece, ISquare targetSquare)
    {
        var piecePos = attackingPiece.GetCurrentCoordinate();
        var pieceType = attackingPiece.GetPieceType();
        var targetPos = targetSquare.GetPosition();

        return pieceType switch
        {
            PieceType.Pawn => CanPawnAttackSquare(piecePos, targetPos, attackingPiece.GetColor()),
            PieceType.King => CanKingAttackSquare(piecePos, targetPos),
            PieceType.Knight => CanKnightAttackSquare(piecePos, targetPos),
            PieceType.Rook => IsInLineOfSight(piecePos, targetPos, true, false),
            PieceType.Bishop => IsInLineOfSight(piecePos, targetPos, false, true),
            PieceType.Queen => IsInLineOfSight(piecePos, targetPos, true, true),
            _ => false
        };
    }

    private bool CanPawnAttackSquare(Point pawnPos, Point targetPos, ColorType pawnColor)
    {
        int direction = pawnColor == ColorType.White ? 1 : -1;
        var attackLeft = new Point { X = pawnPos.X - 1, Y = pawnPos.Y + direction };
        var attackRight = new Point { X = pawnPos.X + 1, Y = pawnPos.Y + direction };

        return (targetPos.Equals(attackLeft) && IsValidCoordinate(attackLeft)) ||
               (targetPos.Equals(attackRight) && IsValidCoordinate(attackRight));
    }

    private bool CanKingAttackSquare(Point kingPos, Point targetPos)
    {
        int deltaX = Math.Abs(targetPos.X - kingPos.X);
        int deltaY = Math.Abs(targetPos.Y - kingPos.Y);
        return deltaX <= 1 && deltaY <= 1 && (deltaX != 0 || deltaY != 0);
    }

    private bool CanKnightAttackSquare(Point knightPos, Point targetPos)
    {
        foreach (var move in KnightMoves)
        {
            var attackPos = new Point { X = knightPos.X + move.X, Y = knightPos.Y + move.Y };
            if (IsValidCoordinate(attackPos) && targetPos.Equals(attackPos))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInLineOfSight(Point from, Point to, bool allowStraight, bool allowDiagonal)
    {
        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        bool isStraight = deltaX == 0 || deltaY == 0;
        bool isDiagonal = Math.Abs(deltaX) == Math.Abs(deltaY);

        if (isStraight && !allowStraight) return false;
        if (isDiagonal && !allowDiagonal) return false;
        if (!isStraight && !isDiagonal) return false;

        int stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
        int stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

        int currentX = from.X + stepX;
        int currentY = from.Y + stepY;

        while (currentX != to.X || currentY != to.Y)
        {
            var currentPos = new Point { X = currentX, Y = currentY };
            if (!IsValidCoordinate(currentPos)) return false;

            if (Board.GetSquare(currentPos).GetPiece() != null)
                return false;

            currentX += stepX;
            currentY += stepY;
        }

        return true;
    }

    public bool IsKingInCheck(ColorType kingColor)
    {
        var kingSquare = FindKingSquare(kingColor);
        if (kingSquare == null) return false;

        var opposingColor = kingColor == ColorType.White ? ColorType.Black : ColorType.White;
        return IsSquareAttacked(kingSquare, opposingColor);
    }

    private ISquare? FindKingSquare(ColorType kingColor)
    {
        foreach (var playerEntry in PlayerPieces)
        {
            if (playerEntry.Key.GetColor() == kingColor)
            {
                foreach (var piece in playerEntry.Value)
                {
                    if (piece.GetPieceType() == PieceType.King && piece.GetState() == PieceState.Active)
                    {
                        return Board.GetSquare(piece.GetCurrentCoordinate());
                    }
                }
            }
        }
        return null;
    }

    // Event handlers
    public void HandleMoveDone() => OnMoveDone?.Invoke();
    public void HandleCapturePiece(IPiece capturedPiece) => OnCapturePiece?.Invoke(capturedPiece);
    public void HandleCastling(IPiece king, IPiece rook) => OnCastling?.Invoke(king, rook);
    public void HandleEnPassant(IPiece capturedPawn) => OnEnPassant?.Invoke(capturedPawn);
    public void HandlePawnPromotion(IPiece pawn) => OnPawnPromotion?.Invoke(pawn);
    public void HandleCheck() => OnCheck?.Invoke();

    public void HandleCheckmate()
    {
        var currentPlayerColor = GetCurrentPlayer().GetColor();
        this.State = currentPlayerColor == ColorType.White ? GameState.CheckmateBlackWin : GameState.CheckmateWhiteWin;
        OnCheckmate?.Invoke();
    }

    public void HandleResign(ColorType resigningPlayerColor)
    {
        this.State = GameState.Resignation;
        OnResign?.Invoke(resigningPlayerColor);
    }

    public void HandleStalemate()
    {
        this.State = GameState.Stalemate;
        OnStalemate?.Invoke();
    }

    // Helper class for game state management
    private class GameStateSnapshot
    {
        public ISquare SourceSquare { get; set; } = null!;
        public ISquare DestSquare { get; set; } = null!;
        public IPiece? OriginalPieceAtSource { get; set; }
        public IPiece? OriginalPieceAtDest { get; set; }
        public Point OriginalPieceCurrentCoord { get; set; }
        public bool OriginalPieceHasMoved { get; set; }
    }
}