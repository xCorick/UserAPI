using Microsoft.Extensions.DependencyInjection;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Security;
using UserAPI.Infrastructure.Persistence;
using UserAPI.Infrastructure.Persistence.Sql;
using UserAPI.Infrastructure.Repositories;
using UserAPI.Infrastructure.Security;

namespace UserAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISqlExecutor, NpgsqlSqlExecutor>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IScoreRepository, ScoreRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();
            return services;
        }
    }
}
