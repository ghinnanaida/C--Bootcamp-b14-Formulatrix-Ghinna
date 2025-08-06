using System;
using System.Collections.Generic;
using ChessGame.Controllers;
using ChessGame.Display;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.Models;

namespace ChessGame.Factories
{
    public class GameFactory
    {
        public static GameDependencies CreateGameDependencies()
        {
            var players = CreatePlayers();
            
            var playerPieces = CreatePlayerPieces(players);
            
            var board = CreateBoard();
            
            ChessDisplay display = null!;
            
            Func<ColorType, PieceType> promotionChoiceProvider = (color) => display.GetPromotionChoice(color);
            
            var gameControl = CreateGameControl(players, playerPieces, board, promotionChoiceProvider);
            
            display = CreateDisplay(gameControl);
            
            var program = CreateProgram(gameControl, display);
            
            GameDependencies result = new GameDependencies
            {
                Players = players,
                PlayerPieces = playerPieces,
                Board = board,
                GameControl = gameControl,
                Display = display,
                Program = program
            };
            
            return result;
        }

        private static List<IPlayer> CreatePlayers()
        {
            List<IPlayer> result = new List<IPlayer>
            {
                new Player(ColorType.White),
                new Player(ColorType.Black)
            };
            return result;
        }

        private static Dictionary<IPlayer, List<IPiece>> CreatePlayerPieces(List<IPlayer> players)
        {
            Dictionary<IPlayer, List<IPiece>> result = new Dictionary<IPlayer, List<IPiece>>
            {
                {players[0], new List<IPiece>()},
                {players[1], new List<IPiece>()}
            };
            return result;
        }

        private static IBoard CreateBoard()
        {
            IBoard result = new Board();
            return result;
        }

        private static GameControl CreateGameControl(
            List<IPlayer> players, 
            Dictionary<IPlayer, List<IPiece>> playerPieces, 
            IBoard board, 
            Func<ColorType, PieceType> promotionChoiceProvider)
        {
            GameControl result = new GameControl(players, playerPieces, board, promotionChoiceProvider);
            return result;
        }

        private static ChessDisplay CreateDisplay(GameControl gameControl)
        {
            ChessDisplay result = new ChessDisplay(gameControl);
            return result;
        }

        private static Program CreateProgram(GameControl gameControl, ChessDisplay display)
        {
            Program result = new Program(gameControl, display);
            return result;
        }
    }

    public class GameDependencies
    {
        public List<IPlayer> Players { get; set; } = null!;
        public Dictionary<IPlayer, List<IPiece>> PlayerPieces { get; set; } = null!;
        public IBoard Board { get; set; } = null!;
        public GameControl GameControl { get; set; } = null!;
        public ChessDisplay Display { get; set; } = null!;
        public Program Program { get; set; } = null!;
    }
}