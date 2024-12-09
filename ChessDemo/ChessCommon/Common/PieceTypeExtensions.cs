using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
