using NUnit.Framework;
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;
using System.Linq;

namespace ChessGame.UnitTests.Controllers;

[TestFixture]
public class GameControllerTests
{
    private GameControl _gameControl;

    [SetUp]
    public void Setup()
    {
        var whitePieces = new List<IPiece>();
        var blackPieces = new List<IPiece>();

        var majorPieceType = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

        for (int i = 0; i < 8; i++)
        {
            whitePieces.Add(new Piece(ColorType.White, PieceState.Active, majorPieceType[i], new Point ()));
            blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, majorPieceType[i], new Point ()));

            whitePieces.Add(new Piece(ColorType.White, PieceState.Active, PieceType.Pawn, new Point ()));
            blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn, new Point ()));
        }

        Dictionary<IPlayer, List<IPiece>> playerPieces = new Dictionary<IPlayer, List<IPiece>>
        {
            {new Player(ColorType.White), whitePieces},
            {new Player(ColorType.Black), blackPieces}
        };
        var board = new Board();
        _gameControl = new GameControl(playerPieces, board);
    }

    [TestCase(4, 3, true)]
    [TestCase(0, 0, true)]
    [TestCase(7, 7, true)]
    [TestCase(8, 0, false)] 
    [TestCase(-1, 5, false)]
    public void IsValidCoordinate_Test(int x, int y, bool expectedResult)
    {
        // Arrange
        var coordinate = new Point { X = x, Y = y };

        // Act
        var result = _gameControl.IsValidCoordinate(coordinate);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase("e4", 4, 3)]
    [TestCase("a1", 0, 0)]
    [TestCase("h8", 7, 7)]
    public void ParseAlgebraicNotation_WithValidInput_ReturnsCorrectPoint(string notation, int expectedX, int expectedY)
    {
        // Act
        var result = _gameControl.ParseAlgebraicNotation(notation);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Value.X, Is.EqualTo(expectedX));
        Assert.That(result.Value.Y, Is.EqualTo(expectedY));
    }

    [TestCase("z9")] 
    [TestCase("a0")] 
    [TestCase("e")] 
    public void ParseAlgebraicNotation_WithInvalidInput_ReturnsNull(string invalidNotation)
    {
        // Act
        var result = _gameControl.ParseAlgebraicNotation(invalidNotation);

        // Assert
        Assert.IsNull(result);
    }

    [TestCase("e4", 4, 3)]
    [TestCase("a1", 0, 0)]
    [TestCase("h8", 7, 7)]
    public void CoordinateToAlgebraic_test(string notation, int x, int y)
    {
        // Act
        var coordinate = new Point { X = x, Y = y };
        var result = _gameControl.CoordinateToAlgebraic(coordinate);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result, Is.EqualTo(notation));
    }
}