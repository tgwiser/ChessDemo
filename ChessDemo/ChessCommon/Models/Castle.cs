using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Models
{
    public class Castle
    {
        public bool IsCastle { get; private set; }
        public Position DestRock { get; internal set; }
        public Position SrcRock { get; internal set; }

        public int SrcRockKey { get { return SrcRock.Y * 8 + SrcRock.X; } }

        public int DestRockKey { get { return DestRock.Y * 8 + DestRock.X; } }

        public bool IsSmallCastle;
        public bool IsLargeCastle;

        public Castle(Position originalPosition)
        {
            IsSmallCastle = originalPosition.X == 6;
            IsLargeCastle = originalPosition.X == 1;
            SrcRock = new Position(originalPosition.Y, IsSmallCastle ? 7 : 0);
            DestRock = new Position(originalPosition.Y, IsSmallCastle ? 5 : 2);
        }
    }
}
