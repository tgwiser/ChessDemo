using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon
{
    public class CommonUtils
    {

        /// <summary>
        /// Board y-dimension
        /// </summary>
        public const int MAX_ROWS = 8;

        /// <summary>
        /// Board x-dimension
        /// </summary>
        public const int MAX_COLS = 8;

        /// <summary>
        /// Short horizontal position from file char<br/>
        /// 'a' => 0<br/>
        /// 'b' => 1<br/>
        /// 'c' => 2<br/>
        /// 'd' => 3<br/>
        /// 'e' => 4<br/>
        /// 'f' => 5<br/>
        /// 'g' => 6<br/>
        /// 'h' => 7<br/>
        /// </summary>
        public static short PositionFromFile(char file)
        {
            return (short)(file - 'a');
        }

        /// <summary>
        /// Short vertical position from rank char<br/>
        /// '1' => 0<br/>
        /// '2' => 1<br/>
        /// '3' => 2<br/>
        /// '4' => 3<br/>
        /// '5' => 4<br/>
        /// '6' => 5<br/>
        /// '7' => 6<br/>
        /// '8' => 7<br/>
        /// </summary>
        public static short PositionFromRank(char rank)
        {
            return (short)(rank - '1');
        }


        public static string Pretify(Position position)
        {
            byte[] intBytes = BitConverter.GetBytes(97 + position.X);
            var file = BitConverter.ToChar(intBytes).ToString();
            return file + (position.Y + 1);
        }

    }

}