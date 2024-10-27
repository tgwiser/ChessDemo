using ChessCommon.Evaluators;
using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        List<Move> gameHistory = new List<Move>();
        int historyIndex = 0;
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
            _boardManager = boardManager;
            _boardManager.Board = board;

            PositionEvaluatorEngine.InitPieces();
            gameEvaluator = new GameEvaluator(PositionEvaluatorEngine, boardManager);
         
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

            if (gameHistory.Count == historyIndex)
            {
                gameHistory.Add(move);
                historyIndex = gameHistory.Count;
            }

            //Reset the game evaluator.
            gameEvaluator.InitPlayersPieces(_boardManager.Board);

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        internal void RestorePiece(Move move)
        {
            //Update the board.
            _boardManager.RestorePiece(move);

            //Reset the game evaluator.
            gameEvaluator.InitPlayersPieces(_boardManager.Board);

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        public bool IsLegalMove(Position position, Destination destPosition)
        {
            Piece piece = _boardManager.GetPiece(position);

            if (piece == null || piece.Color != CurrentPlayer)
                return false;

            List<Destination> legalPositions = PositionEvaluatorEngine.GetLegalPositions(piece);

            var isLegalMove = legalPositions.Exists(lp => lp == destPosition);
            return isLegalMove;
        }


        public void PlayBestMove(int depth)
        {
            if (IsMate(CurrentPlayer))
                return;
            var move = gameEvaluator.EvaluateBestMove(depth, CurrentPlayer);
            if(move!=null)
                DropPiece(move);
        }


        public (string SelectedMove, int Counter , int BestValue) EvaluateBestMove(int depth, PieceColor color)
        {
            gameEvaluator.EvaluateBestMove(depth, color);

            var selectedMove = gameEvaluator?.SelectedMove?.ToString() ?? string.Empty;
            var moveCounter = gameEvaluator?.Counter ?? 0;
            var moveValue = gameEvaluator?.BestValue ?? 0;
            return (selectedMove, moveCounter, moveValue);



        }


        public bool IsCheck(PieceColor color)
        {
            var  opnmentColor = color == PieceColor.Black ? PieceColor.White : PieceColor.Black;
            int bestValue = EvaluateBestMove(1, opnmentColor).BestValue;
            return bestValue == 100;
        }

        public bool IsMate(PieceColor color)
        {
            int bestValue = EvaluateBestMove(2, color).BestValue;
            return bestValue < -10;
        }

        public void SaveBoard(string fileName)
        {
            CommonUtils.SaveBoard(fileName, _boardManager.Board.Pieces);
        }

        public void Next()
        {
            if (historyIndex < gameHistory.Count)
            {
                DropPiece(gameHistory[historyIndex]);
                historyIndex++;
             
            }

        }
        public void Prev()
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                RestorePiece(gameHistory[historyIndex]);
            }

        }

        public void LoadBoard(string fileName)
        {
            var pieces = CommonUtils.GetSavedPieces(fileName);
            _boardManager.Board = new Board(pieces);
            PositionEvaluatorEngine.InitPieces();
        }


    }
}
