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
        void GetGame(string fileName);

        (Move MoveWhite, Move MoveBlack) Next();
    }
}
