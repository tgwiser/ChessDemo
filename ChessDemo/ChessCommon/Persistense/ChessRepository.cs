using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Persistense
{
    internal class ChessRepository : IChessRepository
    {
        private readonly ChessDBContext _chessContext;

        public ChessRepository(ChessDBContext chessContext)
        {
            _chessContext = chessContext;
        }
        public async Task SaveAsync(Game game)
        {
            _chessContext.Add(game);
            await _chessContext.SaveChangesAsync();
        }

        public async Task<Game> LoadAsync(string name)
        {
            var game = await _chessContext.Games.FindAsync(name);
            return game!;
        }

        public async Task<List<string>> GetGamesNameAsync(string name)
        {
            var games = await _chessContext.Games
                .Where(g=>g.Name == name)
                .Select(g=>g.Name)
                .ToListAsync();

            return games;
        }

        public async Task<List<string>> GetGamesNameAsync()
        {
            var games = await _chessContext.Games
                .Select(g => g.Name)
                .ToListAsync();

            return games;
        }

     
    }


    public interface IChessRepository
    {
        Task SaveAsync(Game game);

        Task<Game> LoadAsync(string name);

        Task<List<string>> GetGamesNameAsync(string name);

        Task<List<string>> GetGamesNameAsync();
    }
}
