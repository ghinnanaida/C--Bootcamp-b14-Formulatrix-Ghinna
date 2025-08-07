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

            var whitePieces = new List<IPiece>();
            var blackPieces = new List<IPiece>();

            var backRankPieceType = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

            for (int i = 0; i < 8; i++)
            {
                whitePieces.Add(new Piece(ColorType.White, PieceState.Active, backRankPieceType[i], new Point ()));
                blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, backRankPieceType[i], new Point ()));

                whitePieces.Add(new Piece(ColorType.White, PieceState.Active, PieceType.Pawn, new Point ()));
                blackPieces.Add(new Piece(ColorType.Black, PieceState.Active, PieceType.Pawn, new Point ()));
            }

            Dictionary<IPlayer, List<IPiece>> playerPieces = new Dictionary<IPlayer, List<IPiece>>
            {
                {new Player(ColorType.White), whitePieces},
                {new Player(ColorType.Black), blackPieces}
            };

            IBoard board = new Board();

            GameControl gameControl = new GameControl(playerPieces, board);
            
            ChessDisplay display = new ChessDisplay(gameControl);

            display.Run();
        }
    }
}