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

            var whitePieces = new List<IPiece>();
            var blackPieces = new List<IPiece>();

            var backRankOrder = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

            for (int i = 0; i < 8; i++)
            {
                whitePieces.Add(new Piece(ColorType.White, PieceState.Active, backRankOrder[i], new Point ()));
                blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, backRankOrder[i], new Point ()));

                whitePieces.Add(new Piece(ColorType.White, PieceState.Active, PieceType.Pawn, new Point ()));
                blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn, new Point ()));
            }

            Dictionary<IPlayer, List<IPiece>> playerPieces = new Dictionary<IPlayer, List<IPiece>>
            {
                {players[0], whitePieces},
                {players[1], blackPieces}
            };

            IBoard board = new Board();

            GameControl gameControl = new GameControl(players, playerPieces, board);
            
            ChessDisplay display = new ChessDisplay(gameControl);

            Func<ColorType, PieceType> promotionChoiceProvider = (color) => display.GetPromotionChoice(color);

            gameControl.PromotionChoiceProvider = promotionChoiceProvider;

            display.Run();
        }
    }
}