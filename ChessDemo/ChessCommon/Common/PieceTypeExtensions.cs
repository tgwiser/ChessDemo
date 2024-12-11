using ChessCommon.Models;

namespace ChessCommon.Extensions
{
    internal static class PieceTypeExtensions
    {
        public static string ToPgn(this PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Knight:
                    return "N";
                case PieceType.Bishop:
                    return "B";
                case PieceType.Rook:
                    return "R";
                case PieceType.Queen:
                    return "Q";
                case PieceType.King:
                    return "K";
                case PieceType.Pawn:
                    return "";
            }
            return "";

        }



    }
}
