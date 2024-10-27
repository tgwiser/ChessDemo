﻿using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{
    public class PositionEvaluator : IPositionEvaluator
    {
        Piece?[,]? Pieces;

        public IServiceProvider ServiceProvider { get; }

        public PositionEvaluator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Pieces = null;
        }

        public void InitPieces(Piece?[,]? pieces)
        {
            Pieces = pieces;
        }

        public List<Destination> GetLegalPositions(Piece piece, Board board)
        {
            List<Destination> legalPositions = new List<Destination>();
            Position position = piece.Position;
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    legalPositions = GetLegalPawnPositions(position, piece);
                    break;
                case PieceType.King:
                    legalPositions = GetLegalKingPositions(piece, piece.Color, board);
                    break;
                case PieceType.Rook:
                    legalPositions = GetLegalRockPositions(piece, piece.Color);
                    break;
                case PieceType.Bishop:
                    legalPositions = GetLegalBishopPositions(piece, piece.Color);
                    break;
                case PieceType.Knight:
                    legalPositions = GetLegalNightPositions(piece, piece.Color);
                    break;
                case PieceType.Queen:
                    legalPositions = GetLegalQueenPositions(piece, piece.Color);
                    break;
            }

            return legalPositions;
        }

        private List<Destination> GetLegalPawnPositions(Position position, Piece piece)
        {
            short direction = (short)(piece.Color == PieceColor.White ? 1 : -1);


            List<Destination> legalMoves = new();

            //Move up
            AddMovePawnPosition(piece, direction, position.X, position.Y, ref legalMoves);

            //Eat
            AddEatPawnPosition(piece, position.X - 1, position.Y + direction, ref legalMoves);
            AddEatPawnPosition(piece, position.X + 1, position.Y + direction, ref legalMoves);

            return legalMoves;
        }

        private void AddMovePawnPosition(Piece piece, short direction, int x, int y, ref List<Destination> legalMoves)
        {
            Destination newPosition = new Destination(y + direction, x);

            if (newPosition.HasValue)
            {
                var destPiece = Pieces[newPosition.Y, newPosition.X]!;

                if (destPiece == null)
                {
                    legalMoves.Add(newPosition);

                    int pawnInitPosition = piece.Color == PieceColor.White ? 1 : 6;
                    if (y == pawnInitPosition)
                        AddMovePawnPosition(piece, direction, x, newPosition.Y, ref legalMoves);
                }
            }
        }

        private void AddEatPawnPosition(Piece piece, int x, int y, ref List<Destination> legalMoves)
        {
            int legalMovesCount = legalMoves.Count;
            Destination newPosition = new Destination(y, x);
            if (newPosition.HasValue)
            {
                var destPiece = Pieces[y, x]!;

                if (destPiece != null && piece.Color != destPiece.Color)
                    legalMoves.Add(newPosition);
            }
        }

        private List<Destination> GetLegalKingPositions(Piece piece, PieceColor pieceColor, Board board)
        {
            List<Destination> legalMoves = new();

            (bool smallCastlingEnabled, bool largeCastlingEnabled) = CommonUtils.GetCastleState(piece.Color, board);

            for (short x = -1; x <= 1; x++)
            {
                for (short y = -1; y <= 1; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        Destination newPosition = new Destination(piece.Position.Y + y, piece.Position.X + x, smallCastlingEnabled, largeCastlingEnabled);
                        TryAddPosition(pieceColor, newPosition, ref legalMoves);
                    }
                }
            }

            if (smallCastlingEnabled)
                TryAppendSmallCastlePosition(piece.Position, ref legalMoves);

            if (largeCastlingEnabled)
                TryAppendLargeCastlePosition(piece.Position, ref legalMoves);


            return legalMoves;
        }

        private List<Destination> GetLegalRockPositions(Piece piece, PieceColor pieceColor)
        {
            List<Destination> legalMoves = new();
            var position = piece.Position;

            //right
            for (short x = 1; x < 8; x++)
            {
                if (!TryAddPosition(pieceColor, position.X + x, position.Y, ref legalMoves))
                    break;
            }

            //left
            for (short x = -1; x > -8; x--)
            {
                if (!TryAddPosition(pieceColor, position.X + x, position.Y, ref legalMoves))
                    break;
            }

            //up
            for (short y = 1; y < 8; y++)
            {
                if (!TryAddPosition(pieceColor, position.X, position.Y + y, ref legalMoves))
                    break;
            }

            //down
            for (short y = -1; y > -8; y--)
            {
                if (!TryAddPosition(pieceColor, position.X, position.Y + y, ref legalMoves))
                    break;
            }

            return legalMoves;
        }

        private List<Destination> GetLegalBishopPositions(Piece piece, PieceColor pieceColor)
        {
            List<Destination> legalMoves = new List<Destination>();
            var position = piece.Position;
            //right-up
            int idx = 1;
            while (TryAddPosition(pieceColor, position.X + idx, position.Y + idx, ref legalMoves))
                idx++;

            //right-down
            idx = 1;
            while (TryAddPosition(pieceColor, position.X + idx, position.Y - idx, ref legalMoves))
                idx++;


            //left-up
            for (short x = -1, y = 1; x > -8; x--, y++)
            {
                if (!TryAddPosition(pieceColor, position.X + x, position.Y + y, ref legalMoves))
                    break;
            }

            //left-down
            for (short y = -1; y > -8; y--)
            {
                if (!TryAddPosition(pieceColor, position.X + y, position.Y + y, ref legalMoves))
                    break;
            }

            return legalMoves;
        }

        private List<Destination> GetLegalNightPositions(Piece piece, PieceColor pieceColor)
        {
            List<Destination> legalMoves = new();
            var position = piece.Position;
            TryAddPosition(pieceColor, position.X + 1, position.Y + 2, ref legalMoves);
            TryAddPosition(pieceColor, position.X + 1, position.Y - 2, ref legalMoves);
            TryAddPosition(pieceColor, position.X - 1, position.Y + 2, ref legalMoves);
            TryAddPosition(pieceColor, position.X - 1, position.Y - 2, ref legalMoves);
            TryAddPosition(pieceColor, position.X + 2, position.Y + 1, ref legalMoves);
            TryAddPosition(pieceColor, position.X + 2, position.Y - 1, ref legalMoves);
            TryAddPosition(pieceColor, position.X - 2, position.Y + 1, ref legalMoves);
            TryAddPosition(pieceColor, position.X - 2, position.Y - 1, ref legalMoves);
            return legalMoves;
        }

        private List<Destination> GetLegalQueenPositions(Piece piece, PieceColor pieceColor)
        {
            List<Destination> legalRockMoves = GetLegalRockPositions(piece, pieceColor);
            List<Destination> legalBishopMoves = GetLegalBishopPositions(piece, pieceColor);

            legalRockMoves.AddRange(legalBishopMoves);
            return legalRockMoves;
        }

        private bool TryAddPosition(PieceColor pieceColor, int x, int y, ref List<Destination> legalMoves)
        {
            Destination newPosition = new Destination(y, x);
            return TryAddPosition(pieceColor, newPosition, ref legalMoves);
        }

        private bool TryAddPosition(PieceColor pieceColor, Destination newPosition, ref List<Destination> legalMoves)
        {
            if (!newPosition.HasValue)
                return false;

            var destPiece = Pieces[newPosition.Y, newPosition.X]!;

            if (destPiece == null || destPiece.Color != pieceColor)
            {
                legalMoves.Add(newPosition);
            }
            return destPiece == null;
        }


        public bool TryAppendSmallCastlePosition(Position position, ref List<Destination> legalMoves)
        {
            for (int i = 1; i <= 2; i++)
            {
                if (Pieces[position.Y, position.X + i] != null)
                    return false;

                if (i == 2)
                    legalMoves.Add(new Destination(position.Y, position.X + 2, true, false, true));
            }
            return true;
        }

        public bool TryAppendLargeCastlePosition(Position position, ref List<Destination> legalMoves)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (Pieces[position.Y, position.X - i] != null)
                    return false;

                if (i == 3)
                    legalMoves.Add(new Destination(position.Y, position.X - 3, false, true, true));
            }
            return true;
        }


    }
}