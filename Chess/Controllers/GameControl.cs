using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;
using System.Linq;

namespace ChessGame.Controllers;

public class GameControl
{
    private const int BOARD_SIZE = 8;
    private const int FIFTY_MOVE_RULE_LIMIT = 100;
    private int _fiftyMoveCounter;

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

    public Dictionary<IPlayer, List<IPiece>> PlayerPieces { get; private set; }
    public IBoard Board { get; private set; }
    public GameState State { get; private set; }

    private int _currentPlayerIndex;
    private IPlayer _currentPlayer;
    private ISquare? _intendedSquareSource;

    public Dictionary<IPiece, List<ISquare>>? AllLegalMoves { get; private set; }
    public List<ISquare>? CurrentLegalMoves { get; private set; }
    public ISquare? LastMoveSource { get; private set; }
    public ISquare? LastMoveDestination { get; private set; }
    public IPiece? LastMovedPiece { get; private set; }

    public event Action<IPiece>? OnCapturePiece;
    public event Action<IPiece, IPiece>? OnCastling;
    public event Action<IPiece>? OnEnPassant;
    public event Action<IPiece>? OnPawnPromotion;
    public event Action? OnCheckmate;
    public event Action<ColorType>? OnResign;
    public event Action? OnStalemate;
    public event Action? OnDraw;
    public event Action? OnCheck;
    
    public GameControl(Dictionary<IPlayer, List<IPiece>> playerPieces,
                      IBoard board)
    {
        this.PlayerPieces = playerPieces;
        this.Board = board;
        this.State = GameState.Init;
        this._currentPlayerIndex = 0;
        this._currentPlayer = playerPieces.ToList()[0].Key;
        this._fiftyMoveCounter = 0;
        this._intendedSquareSource = null;
        this.CurrentLegalMoves = null;
        this.AllLegalMoves = null;
        this.LastMoveSource = null;
        this.LastMoveDestination = null;
        this.LastMovedPiece = null;
    }

    public bool IsValidCoordinate(Point coordinate)
    {
        bool isValid = coordinate.X >= 0 && coordinate.X < BOARD_SIZE && coordinate.Y >= 0 && coordinate.Y < BOARD_SIZE;
        return isValid;
    }

    public Point? ParseAlgebraicNotation(string algebraicCoord)
    {
        Point? coordinate = null;

        if (string.IsNullOrWhiteSpace(algebraicCoord) || algebraicCoord.Length != 2)
        {
            return coordinate;
        }

        char fileChar = algebraicCoord[0];
        char rankChar = algebraicCoord[1];

        if (fileChar < 'a' || fileChar > 'h' || !char.IsDigit(rankChar))
        {
            return coordinate;
        }

        int x = fileChar - 'a';
        int y = rankChar - '1';

        if (IsValidCoordinate(new Point { X = x, Y = y }))
        {
            coordinate = new Point { X = x, Y = y };
            return coordinate;
        }

        return coordinate;
    }

    public string CoordinateToAlgebraic(Point coordinate)
    {
        char file = (char)('a' + coordinate.X);
        int rank = coordinate.Y + 1;
        string algebraicCoordinate = $"{file}{rank}";
        return algebraicCoordinate;
    }

    public void InitGame()
    {
        var backRankOrder = new PieceType[] {
            PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
            PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
        };
        foreach (var playerEntry in PlayerPieces)
        {
            var pieces = playerEntry.Value;
            var playerColor = playerEntry.Key.GetColor();

            var pawns = pieces.Where(p => p.GetPieceType() == PieceType.Pawn).ToList();
            var rooks = pieces.Where(p => p.GetPieceType() == PieceType.Rook).ToList();
            var knights = pieces.Where(p => p.GetPieceType() == PieceType.Knight).ToList();
            var bishops = pieces.Where(p => p.GetPieceType() == PieceType.Bishop).ToList();
            var queen = pieces.First(p => p.GetPieceType() == PieceType.Queen);
            var king = pieces.First(p => p.GetPieceType() == PieceType.King);

            int backRank = (playerColor == ColorType.White) ? 0 : 7;
            int pawnRank = (playerColor == ColorType.White) ? 1 : 6;

            for (int i = 0; i < 8; i++)
            {
                var pawnToPlace = pawns[i];
                var position = new Point { X = i, Y = pawnRank };
                
                pawnToPlace.SetCurrentCoordinate(position);
                this.Board.SetSquare(position, pawnToPlace);
            }

            for (int i = 0; i < 8; i++)
            {
                IPiece pieceToPlace;
                var pieceType = backRankOrder[i];
                var position = new Point { X = i, Y = backRank };

                switch (pieceType)
                {
                    case PieceType.Rook:
                        pieceToPlace = (i == 0) ? rooks.First() : rooks.Last();
                        break;
                    case PieceType.Knight:
                        pieceToPlace = (i == 1) ? knights.First() : knights.Last();
                        break;
                    case PieceType.Bishop:
                        pieceToPlace = (i == 2) ? bishops.First() : bishops.Last();
                        break;
                    case PieceType.Queen:
                        pieceToPlace = queen;
                        break;
                    case PieceType.King:
                        pieceToPlace = king;
                        break;
                    default:
                        continue; 
                }
                
                pieceToPlace.SetCurrentCoordinate(position);
                this.Board.SetSquare(position, pieceToPlace);
            }
        }

        this.State = GameState.IntendingMove;
        this._currentPlayerIndex = 0;
        this.AllLegalMoves = GetAllPiecesLegalMoves();
    }

    public IPlayer GetCurrentPlayer()
    {
        return this._currentPlayer;
    }

    public void IntendMove(ISquare sourceSquare)
    {
        if (!CanIntendMove())
        {
            return;
        }

        var piece = sourceSquare.GetPiece();
        if (!IsValidPieceSelection(piece))
        {
            ResetMoveIntention();
            return;
        }

        this._intendedSquareSource = sourceSquare;
        this.State = GameState.MakingMove;
        this.CurrentLegalMoves = AllLegalMoves!.TryGetValue(piece!, out var moves) ? moves : null;
    }

    private bool CanIntendMove()
    {
        bool canIntend = State == GameState.IntendingMove || State == GameState.Check;
        return canIntend;
    }

    private bool IsValidPieceSelection(IPiece? piece)
    {
        bool isValid = piece != null && piece.GetColor() == this._currentPlayer.GetColor();
        return isValid;
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

    public bool MakeMove(ISquare destinationSquare)
    {
        bool moveSuccessful;

        if (!ValidateMoveAttempt(destinationSquare))
        {
            moveSuccessful = false;
            return moveSuccessful;
        }

        IPiece pieceToMove = this._intendedSquareSource!.GetPiece()!;
        ISquare sourceSquare = this._intendedSquareSource;

        UpdateFiftyMoveRule(pieceToMove.GetPieceType(), destinationSquare.GetPiece() != null);
        HandleSpecialMoves(pieceToMove, sourceSquare, destinationSquare);
        MovePiece(sourceSquare, destinationSquare, pieceToMove);
        UpdateMoveHistory(sourceSquare, destinationSquare, pieceToMove);

        NextTurn();

        moveSuccessful = true;
        return moveSuccessful;
    }

    private bool ValidateMoveAttempt(ISquare destinationSquare)
    {
        bool isValid;

        if (State != GameState.MakingMove || _intendedSquareSource == null)
        {
            isValid = false;
            return isValid;
        }

        if (CurrentLegalMoves == null || !CurrentLegalMoves.Contains(destinationSquare))
        {
            ResetMoveIntention();
            isValid = false;
            return isValid;
        }

        var pieceToMove = this._intendedSquareSource.GetPiece();
        if (pieceToMove == null)
        {
            isValid = false;
            return isValid;
        }

        isValid = true;
        return isValid;
    }

    private void HandleSpecialMoves(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        if (IsEnPassantMove(pieceToMove, sourceSquare, destinationSquare))
        {
            HandleEnPassant();
        }
        else
        {
            HandleCapturePiece(destinationSquare);
        }

        if (IsCastlingMove(pieceToMove, sourceSquare, destinationSquare))
        {
            HandleCastling(sourceSquare, destinationSquare);
        }
    }

    private bool IsEnPassantMove(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        bool isEnPassant = pieceToMove.GetPieceType() == PieceType.Pawn &&
               destinationSquare.GetPiece() == null &&
               Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 1 &&
               LastMovedPiece != null &&
               LastMovedPiece.GetPieceType() == PieceType.Pawn &&
               LastMoveDestination != null &&
               LastMoveDestination.GetPosition().X == destinationSquare.GetPosition().X &&
               LastMoveDestination.GetPosition().Y == sourceSquare.GetPosition().Y;
        return isEnPassant;
    }

    private void HandleEnPassant()
    {
        if (LastMovedPiece != null && LastMoveDestination != null)
        {
            LastMovedPiece.SetState(PieceState.Captured);
            LastMoveDestination.SetPiece(null);
            OnEnPassant?.Invoke(LastMovedPiece);
        }
    }

    private void HandleCapturePiece(ISquare destinationSquare)
    {
        var capturedPiece = destinationSquare.GetPiece();
        if (capturedPiece != null)
        {
            capturedPiece.SetState(PieceState.Captured);
            destinationSquare.SetPiece(null);
            OnCapturePiece?.Invoke(capturedPiece);
        }
    }

    private bool IsCastlingMove(IPiece pieceToMove, ISquare sourceSquare, ISquare destinationSquare)
    {
        bool isCastling = pieceToMove.GetPieceType() == PieceType.King &&
               Math.Abs(sourceSquare.GetPosition().X - destinationSquare.GetPosition().X) == 2;
        return isCastling;
    }

    private void HandleCastling(ISquare sourceSquare, ISquare destinationSquare)
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
            OnCastling?.Invoke(this._intendedSquareSource!.GetPiece()!, rook);
        }
    }

    public bool IsPawnPromotion(IPiece piece)
    {
        int promotionRank = piece!.GetColor() == ColorType.White ? 7 : 0;
        var lastPosition = this.LastMoveDestination!.GetPosition();
        bool isPromotion = piece!.GetPieceType() == PieceType.Pawn &&
                         lastPosition.Y ==promotionRank;
        return isPromotion;
    }

    public IPiece HandlePawnPromotion(IPiece piece, PieceType pieceType)
    {
        piece.SetPieceType(pieceType);
        OnPawnPromotion?.Invoke(piece);
        return piece;
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

    private void UpdateFiftyMoveRule(PieceType pieceToMoveType, bool isCapture)
    {
        if (isCapture || pieceToMoveType == PieceType.Pawn)
        {
            this._fiftyMoveCounter = 0;
        }
        else
        {
            this._fiftyMoveCounter += 1;
        }
    }

    public void NextTurn()
    {
        this._currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
        ResetTurnState();
        
        this._currentPlayer = PlayerPieces.ToList()[this._currentPlayerIndex].Key;
        bool isInCheck = IsKingInCheck(this._currentPlayer.GetColor());
        this.AllLegalMoves = GetAllPiecesLegalMoves();
        int totalMoveCount = this.AllLegalMoves.Values.Sum(moves => moves.Count);

        UpdateGameState(isInCheck, totalMoveCount, this._currentPlayer);
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
                HandleCheckmate();
            }
            else
            {
                HandleCheck();
            }
        }
        else
        {
            if (this._fiftyMoveCounter >= FIFTY_MOVE_RULE_LIMIT)
            {
                HandleFiftyMoveDraw();
            }
            else if (legalMovesCount == 0)
            {
                HandleStalemate();
            }
        }
    }

    public List<ISquare> GetPossibleLegalMoves(ISquare sourceSquare)
    {
        var piece = sourceSquare.GetPiece();
        List<ISquare> possibleMoves;

        if (piece == null || piece.GetState() != PieceState.Active)
        {
            possibleMoves = new List<ISquare>();
            return possibleMoves;
        }

        var position = sourceSquare.GetPosition();
        var pieceType = piece.GetPieceType();
        var pieceColor = piece.GetColor();

        possibleMoves = pieceType switch
        {
            PieceType.Pawn => GetPawnMoves(position, pieceColor),
            PieceType.Rook => GetSlidingMoves(position, pieceColor, RookDirections),
            PieceType.Knight => GetKnightMoves(position, pieceColor),
            PieceType.Bishop => GetSlidingMoves(position, pieceColor, BishopDirections),
            PieceType.Queen => GetQueenMoves(position, pieceColor),
            PieceType.King => GetKingMoves(position, pieceColor),
            _ => new List<ISquare>()
        };
        return possibleMoves;
    }

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

                if (targetPiece == null || targetPiece.GetColor() != pieceColor)
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

        AddPawnForwardMoves(legalMoves, position, direction, startY);
        AddPawnCaptureMoves(legalMoves, position, direction, pieceColor);
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

        foreach (var direction in KingDirections)
        {
            var newPos = new Point { X = position.X + direction.X, Y = position.Y + direction.Y };

            if (IsValidCoordinate(newPos))
            {
                var square = this.Board.GetSquare(newPos);
                var targetPiece = square.GetPiece();

                if (targetPiece == null || (targetPiece.GetState() == PieceState.Active && targetPiece.GetColor() != pieceColor))
                {
                    legalMoves.Add(square);
                }
            }
        }

        AddCastlingMoves(legalMoves, position, pieceColor);

        return legalMoves;
    }

    private void AddCastlingMoves(List<ISquare> legalMoves, Point position, ColorType pieceColor)
    {
        var king = Board.GetSquare(position).GetPiece();
        if (king == null || king.GetHasMoved() || IsKingInCheck(pieceColor))
            return;

        if (CanCastleKingSide(position, pieceColor))
        {
            legalMoves.Add(Board.GetSquare(new Point { X = position.X + 2, Y = position.Y }));
        }

        if (CanCastleQueenSide(position, pieceColor))
        {
            legalMoves.Add(Board.GetSquare(new Point { X = position.X - 2, Y = position.Y }));
        }
    }

    private bool CanCastleKingSide(Point kingPosition, ColorType pieceColor)
    {
        bool canCastle;
        var kingsideRook = Board.GetSquare(new Point { X = 7, Y = kingPosition.Y }).GetPiece();
        if (kingsideRook == null || kingsideRook.GetPieceType() != PieceType.Rook ||
            kingsideRook.GetHasMoved() || kingsideRook.GetColor() != pieceColor)
        {
            canCastle = false;
            return canCastle;
        }

        for (int x = kingPosition.X + 1; x < 7; x++)
        {
            if (Board.GetSquare(new Point { X = x, Y = kingPosition.Y }).GetPiece() != null)
            {
                canCastle = false; 
                return canCastle;
            }
        }

        var opposingColor = pieceColor == ColorType.White ? ColorType.Black : ColorType.White;
        for (int x = kingPosition.X; x <= kingPosition.X + 2; x++)
        {
            if (IsSquareAttacked(Board.GetSquare(new Point { X = x, Y = kingPosition.Y }), opposingColor))
            {
                canCastle = false;
                return canCastle;
            }
        }
        canCastle = true;
        return canCastle;
    }

    private bool CanCastleQueenSide(Point kingPosition, ColorType pieceColor)
    {
        bool canCastle;
        var queensideRook = Board.GetSquare(new Point { X = 0, Y = kingPosition.Y }).GetPiece();
        if (queensideRook == null || queensideRook.GetPieceType() != PieceType.Rook ||
            queensideRook.GetHasMoved() || queensideRook.GetColor() != pieceColor)
        {
            canCastle = false;
            return canCastle;
        }

        for (int x = 1; x < kingPosition.X; x++)
        {
            if (Board.GetSquare(new Point { X = x, Y = kingPosition.Y }).GetPiece() != null)
            {   
                canCastle = false;
                return canCastle;
            }
        }

        var opposingColor = pieceColor == ColorType.White ? ColorType.Black : ColorType.White;
        for (int x = kingPosition.X; x >= kingPosition.X - 2; x--)
        {
            if (IsSquareAttacked(Board.GetSquare(new Point { X = x, Y = kingPosition.Y }), opposingColor))
            {
                canCastle = false;
                return canCastle;
            }
        }
        canCastle = true;
        return canCastle;
    }

    public List<ISquare> GetLegalMoves(ISquare sourceSquare)
    {
        var legalMoves = new List<ISquare>();
        var possibleLegalMoves = GetPossibleLegalMoves(sourceSquare);
        var pieceToMove = sourceSquare.GetPiece();

        if (pieceToMove == null)
        {
            return legalMoves;
        }

        foreach (var destSquare in possibleLegalMoves)
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
        var originalPieceAtDest = destSquare.GetPiece();
        var originalCoord = pieceToMove.GetCurrentCoordinate();
        var originalHasMoved = pieceToMove.GetHasMoved();

        sourceSquare.SetPiece(null);
        destSquare.SetPiece(pieceToMove);
        pieceToMove.SetCurrentCoordinate(destSquare.GetPosition());
        if (originalPieceAtDest != null)
        {
            originalPieceAtDest.SetState(PieceState.Captured);
        }

        bool isLegal = !IsKingInCheck(pieceToMove.GetColor());

        sourceSquare.SetPiece(pieceToMove);
        destSquare.SetPiece(originalPieceAtDest);
        pieceToMove.SetCurrentCoordinate(originalCoord);
        pieceToMove.SetHasMoved(originalHasMoved);
        if (originalPieceAtDest != null)
        {
            originalPieceAtDest.SetState(PieceState.Active);
        }

        return isLegal;
    }

    public Dictionary<IPiece, List<ISquare>> GetAllPiecesLegalMoves()
    {
        var allLegalMoves = new Dictionary<IPiece, List<ISquare>>();
        var activePieces = PlayerPieces[_currentPlayer].Where(p => p.GetState() == PieceState.Active);

        foreach (var piece in activePieces)
        {
            var pieceSquare = Board.GetSquare(piece.GetCurrentCoordinate());
            allLegalMoves[piece] = GetLegalMoves(pieceSquare);
        }

        return allLegalMoves;
    }

    public bool IsSquareAttacked(ISquare targetSquare, ColorType attackingColor)
    {
        bool isAttacked = false;
        var attackingPlayer = PlayerPieces.Keys.ToList().First(p => p.GetColor() == attackingColor);

        foreach (var attackingPiece in PlayerPieces[attackingPlayer])
        {
            if (attackingPiece.GetState() != PieceState.Active) continue;

            if (CanPieceAttackSquare(attackingPiece, targetSquare))
            {
                isAttacked = true;
                return isAttacked;
            }
        }
        return isAttacked;
    }

    private bool CanPieceAttackSquare(IPiece attackingPiece, ISquare targetSquare)
    {
        var piecePos = attackingPiece.GetCurrentCoordinate();
        var pieceType = attackingPiece.GetPieceType();
        var targetPos = targetSquare.GetPosition();

        bool canAttack = pieceType switch
        {
            PieceType.Pawn => CanPawnAttackSquare(piecePos, targetPos, attackingPiece.GetColor()),
            PieceType.King => CanKingAttackSquare(piecePos, targetPos),
            PieceType.Knight => CanKnightAttackSquare(piecePos, targetPos),
            PieceType.Rook => IsInLineOfSight(piecePos, targetPos, true, false),
            PieceType.Bishop => IsInLineOfSight(piecePos, targetPos, false, true),
            PieceType.Queen => IsInLineOfSight(piecePos, targetPos, true, true),
            _ => false
        };
        return canAttack;
    }

    private bool CanPawnAttackSquare(Point pawnPos, Point targetPos, ColorType pawnColor)
    {
        int direction = pawnColor == ColorType.White ? 1 : -1;
        var attackLeft = new Point { X = pawnPos.X - 1, Y = pawnPos.Y + direction };
        var attackRight = new Point { X = pawnPos.X + 1, Y = pawnPos.Y + direction };

        bool canAttack = (targetPos.Equals(attackLeft) && IsValidCoordinate(attackLeft)) ||
                         (targetPos.Equals(attackRight) && IsValidCoordinate(attackRight));
        return canAttack;
    }

    private bool CanKingAttackSquare(Point kingPos, Point targetPos)
    {
        int deltaX = Math.Abs(targetPos.X - kingPos.X);
        int deltaY = Math.Abs(targetPos.Y - kingPos.Y);
        bool canAttack = deltaX <= 1 && deltaY <= 1 && (deltaX != 0 || deltaY != 0);
        return canAttack;
    }

    private bool CanKnightAttackSquare(Point knightPos, Point targetPos)
    {
        bool canAttack = false;
        foreach (var move in KnightMoves)
        {
            var attackPos = new Point { X = knightPos.X + move.X, Y = knightPos.Y + move.Y };
            if (IsValidCoordinate(attackPos) && targetPos.Equals(attackPos))
            {
                canAttack = true;
                return canAttack;
            }
        }
        return canAttack;
    }

    private bool IsInLineOfSight(Point from, Point to, bool allowStraight, bool allowDiagonal)
    {
        bool isInLine;
        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        bool isStraight = deltaX == 0 || deltaY == 0;
        bool isDiagonal = Math.Abs(deltaX) == Math.Abs(deltaY);

        if ((isStraight && !allowStraight) || (isDiagonal && !allowDiagonal) || (!isStraight && !isDiagonal))
        {
            isInLine = false;
            return isInLine;
        }

        int stepX = Math.Sign(deltaX);
        int stepY = Math.Sign(deltaY);

        int currentX = from.X + stepX;
        int currentY = from.Y + stepY;

        while (currentX != to.X || currentY != to.Y)
        {
            if (Board.GetSquare(new Point { X = currentX, Y = currentY }).GetPiece() != null)
            {
                isInLine = false;
                return isInLine;
            }
            currentX += stepX;
            currentY += stepY;
        }

        isInLine = true;
        return isInLine;
    }

    public bool IsKingInCheck(ColorType kingColor)
    {
        bool isInCheck;
        var kingSquare = FindKingSquare(kingColor);
        if (kingSquare == null)
        {
            isInCheck = false;
            return isInCheck;
        }

        var opposingColor = kingColor == ColorType.White ? ColorType.Black : ColorType.White;
        isInCheck = IsSquareAttacked(kingSquare, opposingColor);
        return isInCheck;
    }

    private ISquare? FindKingSquare(ColorType kingColor)
    {
        var player = PlayerPieces.Keys.ToList().First(p => p.GetColor() == kingColor);
        var king = PlayerPieces[player].FirstOrDefault(p => p.GetPieceType() == PieceType.King && p.GetState() == PieceState.Active);

        ISquare? kingSquare = king != null ? Board.GetSquare(king.GetCurrentCoordinate()) : null;
        return kingSquare;
    }

    public List<MovablePieceInfo> GetMovablePiecesList()
    {
        var movablePieces = new List<MovablePieceInfo>();

        if (this.AllLegalMoves == null)
        {
            return movablePieces;
        }

        foreach (var move in this.AllLegalMoves)
        {
            if (move.Value != null && move.Value.Count > 0)
            {
                var piece = move.Key;
                var position = piece.GetCurrentCoordinate();
                var algebraicPosition = CoordinateToAlgebraic(position);

                movablePieces.Add(new MovablePieceInfo
                {
                    Piece = piece,
                    Position = algebraicPosition,
                    MoveCount = move.Value.Count
                });
            }
        }

        List<MovablePieceInfo> movablePiecesOrdered = movablePieces.OrderBy(p => p.Piece.GetPieceType())
                           .ThenBy(p => p.Position)
                           .ToList();
        return movablePiecesOrdered;
    }

    public void HandleCheck() 
    {
        this.State = GameState.Check;
        OnCheck?.Invoke();
    }

    public void HandleCheckmate()
    {
        var currentPlayerColor = this._currentPlayer.GetColor();
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

    public void HandleFiftyMoveDraw()
    {
        this.State = GameState.FiftyMoveDraw;
        OnDraw?.Invoke();
    }
}