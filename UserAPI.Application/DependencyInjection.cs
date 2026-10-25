using Microsoft.Extensions.DependencyInjection;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Application.Services;

namespace UserAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<ISubjectService, SubjectService>();
            return services;
        }
    }
}
