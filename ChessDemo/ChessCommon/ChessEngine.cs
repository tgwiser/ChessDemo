using ChessCommon.Evaluators;
using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon
{
    public class ChessEngine
    {
        public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

        GameEvaluator gameEvaluator;

        public IPositionEvaluator PositionEvaluatorEngine { get; }

        public Board CB;

        private bool standardiseCastlingPositions;

        /// <summary>
        /// Creates new chess board with default pieces positions
        /// </summary>
        public ChessEngine(IPositionEvaluator positionEvaluator)
        {
            PositionEvaluatorEngine = positionEvaluator;
            CB = new Board(CommonUtils.GetInitChessPices());
            PositionEvaluatorEngine.InitPieces(CB.Pieces);
            gameEvaluator = new GameEvaluator(PositionEvaluatorEngine, CB);
        }


        public void DropPiece(Position srcPosition, Destination destPosition)
        {
            // Moving piece to its new position
            Move move = new Move(srcPosition, destPosition, CB[srcPosition]!, CB[destPosition]!);
            move.Piece.Position = move.DestPosition;
            DropPiece(move);
        }

        internal void DropPiece(Move move)
        {
            CB.Pieces[move.DestPosition.Y, move.DestPosition.X] = move.Piece;

            //Change pawn to queen
            if (move.Piece.Type == PieceType.Pawn && (move.DestPosition.Y == 7 || move.DestPosition.Y == 0))
            {
                move.Piece.Type = PieceType.Queen;
                move.Piece.OriginalPieceType = PieceType.Pawn;
            }

            CB.UpdateCastleState(move.Piece.Color, false, move.DestPosition.IsLeftCastleStateChanged, move.DestPosition.IsRightCastleStateChanged);

            if (move.IsCastle)
            {
                var theRock = CB[move.Castle.SrcRock]!;
                theRock.Position = move.Castle.DestRock;
                CB.Pieces[move.DestPosition.Y, move.Castle.DestRock.X] = theRock;
                CB.Pieces[move.DestPosition.Y, move.Castle.SrcRock.X] = null;
            }


            // Clearing old position
            CB.Pieces[move.SrcPosition.Y, move.SrcPosition.X] = null;

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        internal void RestorePiece(Move move)
        {
            // Moving piece to its original position
            move.Piece.Position = move.SrcPosition;
            CB.Pieces[move.SrcPosition.Y, move.SrcPosition.X] = move.Piece;

            // Clearing new position / or setting captured piece back
            CB.Pieces[move.DestPosition.Y, move.DestPosition.X] = move.CapturedPiece;

            CB.UpdateCastleState(move.Piece.Color, true, move.DestPosition.IsLeftCastleStateChanged, move.DestPosition.IsRightCastleStateChanged);

            if (move.IsCastle)
            {
                var theRock = CB[move.Castle.DestRock]!;
                theRock.Position = move.Castle.SrcRock;
                CB.Pieces[move.Castle.SrcRock.Y, move.Castle.SrcRock.X] = theRock;
                CB.Pieces[move.Castle.DestRock.Y, move.Castle.DestRock.X] = null;
            }


            if (move.Piece.OriginalPieceType == PieceType.Pawn)
                move.Piece.Type = PieceType.Pawn;

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        public (bool, bool) GetCastleState(Position position)
        {
            PieceColor pieceColor = CB[position]!.Color;
            bool smallCastlingEnabled = pieceColor == PieceColor.White ? CB.whiteSmallCastlingEnabled : CB.blackSmallCastlingEnabled;
            bool largeCastlingEnabled = pieceColor == PieceColor.White ? CB.whiteLargeCastlingEnabled : CB.blackLargeCastlingEnabled;
            return (smallCastlingEnabled, largeCastlingEnabled);
        }


        public bool IsLegalMove(Position position, Destination destPosition)
        {
            Piece piece = CB[position]!;

            if (piece == null || piece.Color != CurrentPlayer)
                return false;

            List<Destination> legalPositions = PositionEvaluatorEngine.GetLegalPositions(piece, CB);

            var isLegalMove = legalPositions.Exists(lp => lp == destPosition);
            return isLegalMove;
        }


        public void PlayBestMove(int depth)
        {
            GameEvaluator g = EvaluateBestMove(depth, CurrentPlayer);
            if (g.SelectedMove != null)
                DropPiece(g.SelectedMove);
        }


        public GameEvaluator EvaluateBestMove(int depth, PieceColor color)
        {
            var g = new GameEvaluator(PositionEvaluatorEngine, CB);
            var m = g.EvaluateBestMove(depth, color);
            return g;
        }


        public bool IsCheck()
        {
            EvaluateBestMove(1, CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black);
            return gameEvaluator.BestValue == 100;
        }

        public bool IsMate()
        {
            EvaluateBestMove(2, CurrentPlayer);
            return gameEvaluator.BestValue < -10;
        }

        public void SaveBoard(string fileName)
        {
            CommonUtils.SaveBoard(fileName, CB.Pieces);
        }

        public void LoadBoard(string fileName)
        {
            var pieces = CommonUtils.GetSavedPieces(fileName);
            CB = new Board(pieces);
            PositionEvaluatorEngine.InitPieces(pieces);
        }


    }
}
