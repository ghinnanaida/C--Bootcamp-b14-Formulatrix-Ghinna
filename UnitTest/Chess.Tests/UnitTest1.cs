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
        var board = new Board();
        var playerPieces = new Dictionary<IP;
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}