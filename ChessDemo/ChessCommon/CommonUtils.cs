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
        static PieceType[] pieceTypes ={
            PieceType.Rook,
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.King,
            PieceType.Bishop,
            PieceType.Knight,
            PieceType.Rook
        };

        /// <summary>
        /// Board y-dimension
        /// </summary>
        public const int MAX_ROWS = 8;

        /// <summary>
        /// Board x-dimension
        /// </summary>
        public const int MAX_COLS = 8;

        public static Piece?[,] GetInitChessPices()
        {
            Piece?[,] pieces = new Piece[MAX_ROWS, MAX_COLS];
            for (int i = 0; i < 8; i++)
            {
                pieces[0, i] = new Piece(PieceColor.White, pieceTypes[i], new Position(0, i));
                pieces[7, i] = new Piece(PieceColor.Black, pieceTypes[i], new Position(7, i));
            }

            for (int i = 0; i < 8; i++)
            {
                pieces[1, i] = new Piece(PieceColor.White, PieceType.Pawn, new Position(1, i));
                pieces[6, i] = new Piece(PieceColor.Black, PieceType.Pawn, new Position(6, i));
            }
            return pieces;
        }

        public static void SaveBoard(string fileName, Piece?[,] pieces)
        {
            List<string> csvData = new List<string>();
            for (int x = 0; x < MAX_ROWS; x++)
            {
                for (int y = 0; y < MAX_COLS; y++)
                {
                    Piece? piece = pieces[x, y];
                    if (piece != null)
                        csvData.Add(piece.ToString());
                }
            }
            File.WriteAllLines(fileName, csvData);
        }

        public static Piece?[,] GetSavedPieces(string fileName)
        {
            var csvData = File.ReadAllLines(fileName);
            var pieces = new Piece[MAX_ROWS, MAX_COLS];
            foreach (var data in csvData)
            {
                var rawData = data.Split(',');

                Position position = new Position(rawData[2]);

                if (Enum.TryParse(rawData[0], out PieceColor color) &&
                    Enum.TryParse(rawData[1], out PieceType pieceType))
                {
                    pieces[position.Y, position.X] = new Piece(color, pieceType, position);
                }
            }
            return pieces;

        }



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


        public static (bool, bool) GetCastleState(PieceColor pieceColor, Board board)
        {
            bool smallCastlingEnabled = pieceColor == PieceColor.White ? board.whiteSmallCastlingEnabled : board.blackSmallCastlingEnabled;
            bool largeCastlingEnabled = pieceColor == PieceColor.White ? board.whiteLargeCastlingEnabled : board.blackLargeCastlingEnabled;
            return (smallCastlingEnabled, largeCastlingEnabled);
        }


        public static string Pretify(Position position)
        {
            byte[] intBytes = BitConverter.GetBytes(97 + position.X);
            var file = BitConverter.ToChar(intBytes).ToString();
            return file + (position.Y + 1);
        }

    }

}