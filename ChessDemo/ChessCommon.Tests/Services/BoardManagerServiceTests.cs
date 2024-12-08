using ChessCommon.Services.Contracts;
using ChessCommon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessCommon.Models;
using Moq;

namespace ChessCommon.Tests.Services
{
    public class BoardManagerServiceTests
    {
        private readonly Mock<Board> _mockBoard;
        private readonly BoardManagerService _boardManagerService;

        public BoardManagerServiceTests() {
            _mockBoard = new Mock<Board>();
            _boardManagerService = new BoardManagerService();
            _boardManagerService.Reset();
        }
            


        [Theory]
        [MemberData(nameof(BoardPiecesData))]
        public void GetPiece_InitBoard_ValidatePiece(int x, int y, PieceColor color, PieceType type)
        {
            // Arrange
            _boardManagerService.Reset();

            //Act
            var pieceByCordination = _boardManagerService.GetPiece(y, x);
            var pieceByPossition = _boardManagerService.GetPiece(new Position(y, x));

            //Assert
            Assert.Equal(pieceByCordination!.Color, color);
            Assert.Equal(pieceByCordination!.Type, type);
            Assert.Equal(pieceByCordination, pieceByPossition);
        }

      
        [Fact]
        public void GetAllPieces_InitBoard_ValidatePieces()
        {
            // Arrange
            var allPieces = GetBoardPieces().ToList();

            //Act
            var allWhitePieces = _boardManagerService.GetAllPieces(PieceColor.White);
            foreach (var piece in allWhitePieces) {
                Assert.Contains(allPieces, p => p.Position == piece!.Position);
            }

            var allBlackPieces = _boardManagerService.GetAllPieces(PieceColor.White);
            foreach (var piece in allBlackPieces)
            {
                Assert.Contains(allPieces, p => p.Position == piece!.Position);
            }
        }



        public static IEnumerable<object[]> BoardPiecesData()
        {
            var pieces = GetBoardPieces();
            foreach (var piece in pieces)
            {
                yield return new object[] { piece.Position.X, piece.Position.Y, piece.Color, piece.Type };
            }
        }

        public static IEnumerable<Piece> GetBoardPieces()
        {
            List<Piece> pieces = new List<Piece>();

            for (int i = 0; i < 8; i++)
            {
                pieces.Add(new Piece(PieceColor.White, PieceType.Pawn, new Position(1, i)));
                pieces.Add(new Piece(PieceColor.Black, PieceType.Pawn, new Position(6, i)));
            }

            for (int i = 0; i < 8; i++)
            {
                var pType = CreatePieceTypeByPosition(i);
                pieces.Add(new Piece(PieceColor.White, pType, new Position(0, i)));
                pieces.Add(new Piece(PieceColor.Black, pType, new Position(7, i)));

            }
            return pieces;
        }



        static PieceType CreatePieceTypeByPosition(int x)
        {
            if (x == 0 || x == 7)
                return PieceType.Rook;

            if (x == 1 || x == 6)
                return PieceType.Knight;

            if (x == 2 || x == 5)
                return PieceType.Bishop;

            if (x == 3)
                return PieceType.Queen;

            if (x == 4)
                return PieceType.King;

            throw new Exception("Invalid unit name");
        }
    }
}
