﻿using ChessCommon.Models;
using ChessCommon.Services.Contracts;

namespace ChessCommon.Services;

public class BoardManagerService : IBoardManagerService
{
    public Board? Board { get; private set; }

    public Piece GetPiece(Position position) => Board![position]!;

    public Piece? GetPiece(int y, int x) => Board![y, x];

    public List<Piece?> GetAllPieces(PieceColor color)
    {
        var pieces = new List<Piece?>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = GetPiece(y, x);
                if (piece != null && piece.Color == color)
                    pieces.Add(piece);
            }
        }
        return pieces;
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
        Board!.Pieces[move.DestPosition.Y, move.DestPosition.X] = move.Piece;
        move.Piece.Position = move.DestPosition;

        // Clearing old position
        Board[move.SrcPosition] = null;

        //Update castle state (For rock\King move)
        if (move.IsLeftRock || move.Piece.Type == PieceType.King)
            UpdateLeftCastleMoveAndBoard(move, Board);

        if (move.IsRightRock || move.Piece.Type == PieceType.King)
            UpdateRightCastleMoveAndBoard(move, Board);

        //Castle change the rock position as well
        if (move.IsCastle)
        {
            move.Castle = new Castle(move.DestPosition);

            Piece theRock = GetPiece(move.Castle.SrcRock);
            theRock.Position = move.Castle.DestRock;
            Board[move.Castle.DestRock] = theRock;
            Board[move.Castle.SrcRock] = null;
        }
    }

    public void RestorePiece(Move move)
    {
        // Moving piece to its original position
        move.Piece.Position = move.SrcPosition;
        Board![move.SrcPosition] = move.Piece;

        // Clearing dest position / or setting captured piece back
        Board[move.DestPosition] = move.CapturedPiece;


        //Update castle state (For rock\King move)
        if (move.LeftCastlingEnabled.HasValue)
            Board.UpdateLeftCastleState(move.Piece.Color, true);

        if (move.RightCastlingEnabled.HasValue)
            Board.UpdateRightCastleState(move.Piece.Color, true);

        //Castle change the rock position as well
        if (move.IsCastle && move.Castle != null)
        {
            var theRock = Board[move.Castle.DestRock]!;
            theRock.Position = move.Castle.SrcRock;
            Board[move.Castle.SrcRock] = theRock;
            Board[move.Castle.DestRock] = null;
        }

        //Restoring pawn if needed
        if (move.Piece.OriginalPieceType == PieceType.Pawn)
            move.Piece.Type = PieceType.Pawn;
    }

    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) GetCastleState(PieceColor color)
    {
        return (color == PieceColor.White) ?
            (Board!.IsWhiteRightCastlingEnabled, Board!.IsWhiteRightCastlingEnabled) :
            (Board!.IsBlackLeftCastlingEnabled, Board!.IsBlackRightCastlingEnabled);
    }

    public void Reset(Board board = null)
    {
        Board = board ?? CommonUtils.GetIDefaultBoard();
    }

    private void UpdateLeftCastleMoveAndBoard(Move move, Board board)
    {
        if (GetCastleState(move.Piece.Color).IsLeftCastlingEnabled)
        {
            move.LeftCastlingEnabled = false;
            board.UpdateLeftCastleState(move.Piece.Color, false);
        }
    }

    private void UpdateRightCastleMoveAndBoard(Move move, Board board)
    {
        if (GetCastleState(move.Piece.Color).IsRightCastlingEnabled)
        {
            move.RightCastlingEnabled = false;
            board.UpdateRightCastleState(move.Piece.Color, false);

        }
    }

}
