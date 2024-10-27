using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Models
{
    public class Board
    {
        public const int MAX_ROWS = 8;

        public const int MAX_COLS = 8;

        public bool whiteSmallCastlingEnabled = true;
        public bool whiteLargeCastlingEnabled = true;
        public bool WhiteCastlingEnabled { get => whiteSmallCastlingEnabled || whiteLargeCastlingEnabled; }

        public bool blackSmallCastlingEnabled = true;
        public bool blackLargeCastlingEnabled = true;
        public bool BlackCastlingEnabled { get => blackSmallCastlingEnabled || blackLargeCastlingEnabled; }

        public Board(Piece?[,] pieces)
        {
            Pieces = pieces;
        }

        public Piece?[,] Pieces { get; private set; }

        /// <summary>
        /// Returns Piece on given position
        /// </summary>
        /// <param name="pos">Position on chess board</param>
        public Piece? this[Position pos] => Pieces[pos.Y, pos.X];

        /// <summary>
        /// Returns Piece on given position
        /// </summary>
        /// <param name="x">0->8</param>
        /// <param name="y">0->8</param>
        public Piece? this[int x, int y] => Pieces[y, x];

        public (bool, bool) GetCastleState(PieceColor color)
        {
            return (color == PieceColor.White) ?
                (whiteSmallCastlingEnabled, whiteLargeCastlingEnabled) :
                (blackSmallCastlingEnabled, blackLargeCastlingEnabled);
        }

        public void UpdateCastleState(PieceColor color, bool newStatus, bool isSmallCastlingEnabled, bool isLargeCastlingEnabled)
        {
            if (color == PieceColor.White)
            {

                if (isSmallCastlingEnabled)
                    whiteSmallCastlingEnabled = newStatus;

                if (isLargeCastlingEnabled)
                    whiteLargeCastlingEnabled = newStatus;
            }
            else
            {
                if (isSmallCastlingEnabled)
                    blackSmallCastlingEnabled = newStatus;

                if (isLargeCastlingEnabled)
                    blackLargeCastlingEnabled = newStatus;
            }
        }


    }

}
