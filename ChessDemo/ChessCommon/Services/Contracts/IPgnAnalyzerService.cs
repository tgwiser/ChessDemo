using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Services.Contracts
{
    public interface IPgnAnalyzerService
    {
        List<(Move WhiteMove, Move BlackMove)> LoadGame(string input);

    }
}
