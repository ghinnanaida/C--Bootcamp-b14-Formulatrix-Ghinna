using System;
using System.Collections.Generic; 
using System.Linq;
using ChessGame.Controllers;
using ChessGame.Enumerations;
using ChessGame.Interfaces;
using ChessGame.RecordStructs;
using ChessGame.Models;
using ChessGame.Display;

namespace ChessGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<IPlayer> players = new List<IPlayer>
            {
                new Player(ColorType.White),
                new Player(ColorType.Black)
            };

            Dictionary<IPlayer, List<IPiece>> playerPieces = new Dictionary<IPlayer, List<IPiece>>
            {
                {players[0], new List<IPiece>()},
                {players[1], new List<IPiece>()}
            };

            IBoard board = new Board();
            ChessDisplay _display = null;

            Func<ColorType, PieceType> promotionChoiceProvider = (color) => _display.GetPromotionChoice(color);

            GameControl _gameControl = new GameControl(players, playerPieces, board, promotionChoiceProvider);
            
            _display = new ChessDisplay(_gameControl);

            _display.Run();
        }
    }
}