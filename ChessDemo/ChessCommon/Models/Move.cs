using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Models
{
    public class Move
    {
        public bool IsCastle { get { return Castle != null; } }

        public Castle? Castle;

        /// <summary>
        /// Initializes new Move object by given positions
        /// </summary>
        public Move(Position originalPosition, Destination newPosition, Piece piece, Piece? capturedPiece)
        {
            SrcPosition = originalPosition;
            DestPosition = newPosition;

            Piece = piece;
            CapturedPiece = capturedPiece;

            if (newPosition.IsCastle)
                Castle = new Castle(DestPosition);
        }


        /// <summary>
        /// Moved Piece
        /// </summary>
        public Piece Piece { get; set; }

        /// <summary>
        /// Original position of moved piece
        /// </summary>
        public Position SrcPosition { get; set; }

        /// <summary>
        /// New Position of moved piece
        /// </summary>
        public Destination DestPosition { get; set; }

        /// <summary>
        /// Captured piece (if exist) or null
        /// </summary>
        public Piece? CapturedPiece { get; set; }

        /// <summary>
        /// Move places opponent's king in check? => true
        /// </summary>
        public bool IsCheck { get; internal set; }

        /// <summary>
        /// Move places opponent's king in checkmate => true
        /// </summary>
        public bool IsMate { get; internal set; }

        /// <summary>
        /// Whether Positions are initialized
        /// </summary>
        /// 
        public bool HasValue => SrcPosition.HasValue && DestPosition.HasValue;

        public bool? OriginalSmallCastleEnable { get; set; }
        public bool? OriginalLargeCastleEnable { get; set; }

        public override string ToString()
        {
            return $"{SrcPosition} + ->  + {DestPosition}";
        }

    }
}
