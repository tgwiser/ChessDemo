using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{
    public interface IGameEvaluator
    {
        int BestValue { get; }
        int Counter { get; }
        Move? SelectedMove { get; }

        Move? EvaluateBestMove(int depth, PieceColor selectedPlayer);
    }
}
