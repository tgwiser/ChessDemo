using ChessCommon.Persistense;
using ChessCommon.Services;
using ChessCommon.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessCommon
{
    public static class ChessServiceRegistration
    {
        public static IServiceCollection AddChessServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Persistense
            var connectionString = configuration.GetConnectionString("ChessDatabaseConnectionString");
            services.AddDbContext<ChessDBContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IGamePersistenseService, GamePersistenseService>();


            services.AddScoped<IChessRepository, ChessRepository>();
            services.AddScoped<IBoardManagerService, BoardManagerService>();
            services.AddScoped<IPositionEvaluatorService, PositionEvaluatorService>();
            services.AddScoped<IGameEvaluatorService, GameEvaluatorService>();
            services.AddScoped<IPgnAnalyzerService, PgnAnalyzerService>();
            services.AddScoped<IChessEngine, ChessEngine>();



            return services;
        }
    }
}
