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

        public (bool Left, bool Right) BlackCastlingState { get{ return (_boardManager.Board.blackLeftCastlingEnabled, _boardManager.Board.blackRightCastlingEnabled); } }

        public (bool Left, bool Right) WhiteCastlingState { get { return (_boardManager.Board.whiteLeftCastlingEnabled, _boardManager.Board.whiteRightCastlingEnabled); } }


        GameEvaluator gameEvaluator;

        public IPositionEvaluator PositionEvaluatorEngine { get; }

        public IBoardManager _boardManager;

        private bool standardiseCastlingPositions;

        /// <summary>
        /// Creates new chess board with default pieces positions
        /// </summary>
        public ChessEngine(IPositionEvaluator positionEvaluator, IBoardManager boardManager)
        {
            PositionEvaluatorEngine = positionEvaluator;
            var board  = new Board(CommonUtils.GetInitChessPices());
            PositionEvaluatorEngine.InitPieces(board.Pieces);
            gameEvaluator = new GameEvaluator(PositionEvaluatorEngine, board);
            _boardManager = boardManager;
            _boardManager.Board = board;
        }


        public void DropPiece(Position srcPosition, Destination destPosition)
        {
            // Moving piece to its new position
            Move move = new Move(
                srcPosition,
                destPosition, 
                GetPiece(srcPosition),
                GetPiece(destPosition));

            move.Piece.Position = move.DestPosition;
            DropPiece(move);
        }

        public Piece GetPiece(Position position)
        {
            return _boardManager.GetPiece(position);
        }

        internal void DropPiece(Move move)
        {
            //Update the board.
            _boardManager.DropPiece(move);

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        internal void RestorePiece(Move move)
        {
            //Update the board.
            _boardManager.RestorePiece(move);

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        public bool IsLegalMove(Position position, Destination destPosition)
        {
            Piece piece = _boardManager.GetPiece(position);

            if (piece == null || piece.Color != CurrentPlayer)
                return false;

            List<Destination> legalPositions = PositionEvaluatorEngine.GetLegalPositions(piece, _boardManager.Board);

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
            var g = new GameEvaluator(PositionEvaluatorEngine, _boardManager.Board);
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
            CommonUtils.SaveBoard(fileName, _boardManager.Board.Pieces);
        }

        public void LoadBoard(string fileName)
        {
            var pieces = CommonUtils.GetSavedPieces(fileName);
            _boardManager.Board = new Board(pieces);
            PositionEvaluatorEngine.InitPieces(pieces);
        }


    }
}
