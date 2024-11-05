using ChessCommon.Evaluators.Contracts;
using ChessCommon.Evaluators;
using Microsoft.Extensions.DependencyInjection;

namespace ChessCommon
{
    public static class ChessServiceRegistration
    {
        public static IServiceCollection AddChessServices(this IServiceCollection services)
        {
            services.AddSingleton<IBoardManager, BoardManager>();
            services.AddSingleton<IPositionEvaluator, PositionEvaluator>();
            return services;
        }
    }
}
