using ChessCommon.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessCommon.Persistense
{
    internal class ChessRepository : IChessRepository
    {
        private readonly ChessDBContext _chessContext;

        public ChessRepository(ChessDBContext chessContext)
        {
            _chessContext = chessContext;
        }

        public async Task<List<string>> GetGamesNameAsync()
        {
            var games = await _chessContext.Games
                .Select(g => g.Name)
                .ToListAsync();

            return games;
        }


        public async Task<List<string>> GetGamesNameAsync(string filter)
        {
            var games = await _chessContext.Games
            .Select(g => g.Name)
            .Where(n => n.Contains(filter))
            .ToListAsync();

            return games;
        }

        public async Task<Game> GetGameAsync(string name)
        {
            var game = await _chessContext.Games.FirstOrDefaultAsync(g => g.Name == name);
            return game;
        }


        public async Task SaveAsync(Game game)
        {
            _chessContext.Add(game);
            await _chessContext.SaveChangesAsync();
        }

        public async Task DeleteGame(string name)
        {
            var game = await _chessContext.Games.FirstOrDefaultAsync(g => g.Name == name);
            var c1 = _chessContext.Games.Remove(game!);
            await _chessContext.SaveChangesAsync();

        }

        public async Task UpdateGame(string name, string moves)
        {
            var game = await _chessContext.Games.FirstOrDefaultAsync(g => g.Name == name);
            var c1 = _chessContext.Games.Update(game!);
            await _chessContext.SaveChangesAsync();
        }


    }


    public interface IChessRepository
    {
        Task<List<string>> GetGamesNameAsync();

        Task<List<string>> GetGamesNameAsync(string filter);

        Task<Game> GetGameAsync(string name);

        Task SaveAsync(Game game);

        Task DeleteGame(string name);

        Task UpdateGame(string name, string moves);
    }
}
