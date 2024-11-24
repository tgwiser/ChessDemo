using ChessCommon.Services;
using ChessCommon.Services.Contracts;
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
            services.AddScoped<IGamePersistenseService, GamePersistenseService>();


            services.AddScoped<IChessRepository, ChessRepository>();
            services.AddScoped<IBoardManager, BoardManagerService>();
            services.AddScoped<IPositionEvaluator, PositionEvaluatorService>();
            services.AddScoped<IGameEvaluator, GameEvaluatorService>();
            services.AddScoped<IChessEngine, ChessEngine>();



            return services;
        }
    }
}
