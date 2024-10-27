using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{
    public class BoardManager: IBoardManager
    {
        public Board Board { get; set; }

        public BoardManager()
        {
        }


        public void DropPiece(Move move)
        {
            //Change pawn to queen
            if (move.Piece.Type == PieceType.Pawn && (move.DestPosition.Y == 7 || move.DestPosition.Y == 0))
            {
                move.Piece.Type = PieceType.Queen;
                move.Piece.OriginalPieceType = PieceType.Pawn;
            }

            //Adding to new position
            Board.Pieces[move.DestPosition.Y, move.DestPosition.X] = move.Piece;
            move.Piece.Position= move.DestPosition;

            // Clearing old position
            Board.Pieces[move.SrcPosition.Y, move.SrcPosition.X] = null;

            //Update castle state (For rock\King move)
            Board.UpdateCastleState(move.Piece.Color, false, move.DestPosition.IsLeftCastleStateChanged, move.DestPosition.IsRightCastleStateChanged);

            //Castle change the rock position as well
            if (move.IsCastle)
            {
                var theRock = Board[move.Castle.SrcRock]!;
                theRock.Position = move.Castle.DestRock;
                Board.Pieces[move.DestPosition.Y, move.Castle.DestRock.X] = theRock;
                Board.Pieces[move.DestPosition.Y, move.Castle.SrcRock.X] = null;
            }
        }

        public void RestorePiece(Move move)
        {
            // Moving piece to its original position
            move.Piece.Position = move.SrcPosition;
            Board.Pieces[move.SrcPosition.Y, move.SrcPosition.X] = move.Piece;

            // Clearing dest position / or setting captured piece back
            Board.Pieces[move.DestPosition.Y, move.DestPosition.X] = move.CapturedPiece;

            //Update castle state (For rock\King move)
            Board.UpdateCastleState(move.Piece.Color, true, move.DestPosition.IsLeftCastleStateChanged, move.DestPosition.IsRightCastleStateChanged);

            //Castle change the rock position as well
            if (move.IsCastle)
            {
                var theRock = Board[move.Castle.DestRock]!;
                theRock.Position = move.Castle.SrcRock;
                Board.Pieces[move.Castle.SrcRock.Y, move.Castle.SrcRock.X] = theRock;
                Board.Pieces[move.Castle.DestRock.Y, move.Castle.DestRock.X] = null;
            }

            //Restoring pawn if needed
            if (move.Piece.OriginalPieceType == PieceType.Pawn)
                move.Piece.Type = PieceType.Pawn;
;
        }

        public (bool, bool) GetCastleState(Position position)
        {
            PieceColor pieceColor = Board[position]!.Color;
            bool rightCastlingEnabled = pieceColor == PieceColor.White ? Board.whiteRightCastlingEnabled : Board.blackRightCastlingEnabled;
            bool leftCastlingEnabled = pieceColor == PieceColor.White ? Board.whiteLeftCastlingEnabled : Board.blackLeftCastlingEnabled;
            return (rightCastlingEnabled, leftCastlingEnabled);
        }

        public Piece GetPiece(Position position)
        {
            return Board[position]!;
        }

    }
}
