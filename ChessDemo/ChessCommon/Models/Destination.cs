using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Models
{
    public class Destination : Position
    {
        public bool IsLeftCastleStateChanged { get; set; }
        public bool IsRightCastleStateChanged { get; set; }

        public bool IsCastle { get; set; }


        public Destination(int y, int x) : base(y, x)
        {
        }

        public Destination(int y, int x, bool isLeftCastleStateChanged, bool isRightCastleStateChanged) : base(y, x)
        {
            IsLeftCastleStateChanged = isLeftCastleStateChanged;
            IsRightCastleStateChanged = isRightCastleStateChanged;
        }

        public Destination(int y, int x, bool isLeftCastleStateChanged, bool isRightCastleStateChanged, bool isCastle) : this(y, x, isLeftCastleStateChanged, isRightCastleStateChanged)
        {
            IsCastle = isCastle;
        }

        public static bool operator ==(Destination b1, Destination b2)
        {
            return b1.X == b2.X && b1.Y == b2.Y;
        }

        public static bool operator !=(Destination b1, Destination b2)
        {
            return b1.X != b2.X || b1.Y != b2.Y;
        }
    }
}
