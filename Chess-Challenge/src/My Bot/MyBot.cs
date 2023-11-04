using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    Move finalMove = Move.NullMove;
    Square mostValuablePiece;

    public Move Think(Board board, Timer timer)
    {
        Random random = new Random();
        Move[] moves = board.GetLegalMoves();
        int randomNumber = random.Next(0, moves.Length);

        List<Square> attackedSquares = new List<Square>();

        int capturedPieceValue = 0;
        int mostValuablePieceValue = 0;

        foreach (Move move in moves)
        {
            if (board.SquareIsAttackedByOpponent(move.StartSquare))
            {
                attackedSquares.Add(move.StartSquare);
            }
        }

        foreach (Square square in attackedSquares)
        {
            if (CheckPieceValue(board.GetPiece(square)) >= mostValuablePieceValue)
            {
                mostValuablePieceValue = CheckPieceValue(board.GetPiece(square));
                mostValuablePiece = square;
            }
        }
        Console.WriteLine("mvp: " + mostValuablePiece);

        foreach (Move move in moves)
        {
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }
            
            if (MoveIsCheck(board, move))
            {
                if (!board.SquareIsAttackedByOpponent(move.TargetSquare) || CheckPieceValue(board.GetPiece(move.StartSquare)) <= CheckPieceValue(board.GetPiece(move.TargetSquare)))
                {
                    if (!MoveIsDraw(board, move))
                    {
                        Console.WriteLine("Draw Prevented");
                        finalMove = move;
                    }
                }
            }

            // capture most valuable piece
            if (CheckPieceValue(board.GetPiece(move.TargetSquare)) >= capturedPieceValue)
            {
                if (board.SquareIsAttackedByOpponent(move.TargetSquare))
                {
                    if (CheckPieceValue(board.GetPiece(move.StartSquare)) <= capturedPieceValue)
                    {
                        capturedPieceValue = CheckPieceValue(board.GetPiece(move.TargetSquare));
                        finalMove = move;
                    } else if (CheckPieceValue(board.GetPiece(move.StartSquare)) > capturedPieceValue)
                    {
                        continue;
                    }
                }
                else
                {
                    capturedPieceValue = CheckPieceValue(board.GetPiece(move.TargetSquare));
                    finalMove = move;
                }
            }
        }

        // Move most important piece away to avoid capturing
        foreach (Move move in moves)
        {
            if (board.SquareIsAttackedByOpponent(move.StartSquare) && board.SquareIsAttackedByOpponent(mostValuablePiece) && mostValuablePieceValue >= capturedPieceValue && !board.SquareIsAttackedByOpponent(move.TargetSquare))
            {
                Console.WriteLine("protect mvp");
                finalMove = move;
            }
        }

        Console.WriteLine(capturedPieceValue);

        if (moves.Contains(finalMove))
        {
            return finalMove;
        } else
        {
            // prevents illegal move
            Console.WriteLine("Random move");
            return moves[randomNumber];
        }
    }

    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    bool MoveIsCheck(Board board, Move move)
    {
        board.MakeMove(move);
        bool isCheck = board.IsInCheck();
        board.UndoMove(move);
        return isCheck;
    }

    bool MoveIsDraw(Board board, Move move)
    {
        board.MakeMove(move);
        bool isDraw = board.IsDraw();
        board.UndoMove(move);
        return isDraw;
    }

    int CheckPieceValue(Piece piece)
    {
        int value;

        switch (piece.PieceType)
        {
            case PieceType.Pawn:
                value = 100;
                break;
            case PieceType.Bishop:
                value = 300;
                break;
            case PieceType.Knight:
                value = 300;
                break;
            case PieceType.Rook:
                value = 500;
                break;
            case PieceType.Queen:
                value = 700;
                break;
            case PieceType.King:
                value = 1000;
                break;
            default:
                value = 0;
                break;
        }

        return value;
    }

}