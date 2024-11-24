using ChessCommon.Evaluators;
using ChessCommon.Evaluators.Contracts;
using ChessCommon.Persistense;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessCommon
{
    public static class ChessServiceRegistration
    {
        public static IServiceCollection AddChessServices(this IServiceCollection services,IConfiguration configuration)
        {
            //Persistense
            var connectionString = configuration.GetConnectionString("ChessDatabaseConnectionString");
            services.AddDbContext<ChessDBContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IGamePersistenseManager, GamePersistenseManager>();


            services.AddScoped<IChessRepository, ChessRepository>();
            services.AddScoped<IBoardManager, BoardManager>();
            services.AddScoped<IPositionEvaluator, PositionEvaluator>();
            services.AddScoped<IGameEvaluator, GameEvaluator>();
 
    



            return services;
        }
    }
}
