using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;

namespace ChessCommon.Evaluators;

internal class BoardManager: IBoardManager
{
    public Board? Board { get; set; }

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

            Piece theRock =GetPiece(move.Castle.SrcRock);
            theRock.Position = move.Castle.DestRock;
            Board[move.Castle.DestRock] = theRock;
            Board[move.Castle.SrcRock] = null;
        }
    }

    private void UpdateLeftCastleMoveAndBoard(Move move, Board board)
    {
        if (GetCastleState(move.Piece.Color).LeftCastlingEnabled)
        {
            move.LeftCastlingEnabled = false;
            board.UpdateLeftCastleState(move.Piece.Color, false);
        }
    }

    private void UpdateRightCastleMoveAndBoard(Move move, Board board)
    {
        if (GetCastleState(move.Piece.Color).RightCastlingEnabled)
        {
            move.RightCastlingEnabled = false;
            board.UpdateRightCastleState(move.Piece.Color, false);
        
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

    public (bool LeftCastlingEnabled, bool RightCastlingEnabled) GetCastleState(PieceColor color)
    {
        return (color == PieceColor.White) ?
            (Board!.whiteRightCastlingEnabled, Board!.whiteRightCastlingEnabled) :
            (Board!.blackLeftCastlingEnabled, Board!.blackRightCastlingEnabled);
    }

    public Piece GetPiece(Position position)
    {
        return Board![position]!;
    }

    public Piece? GetPiece(int y, int x)
    {
        return Board![y,x];
    }

    public Piece?[,] GetPieces()
    {
        return Board!.Pieces;
    }
}
