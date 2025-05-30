using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using twitter.api.data.DbContexts;
using Microsoft.OpenApi.Models;

namespace twitter.api.web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTwitterServices(this IServiceCollection services)
        {
            //services.AddTransient<IUserService, UserService>();

            return services;
        }

        public static IServiceCollection AddTwitterDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ITwitterApiDbContext, TwitterApiDbContext>(options =>
                options.UseNpgsql(
                    config.GetConnectionString("TwitterApiDbContext"),
                    b => b.MigrationsAssembly("twitter.api.web"))
                );

            return services;
        }

        public static IServiceCollection AddTwitterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Twitter API",
                    Version = "v1"
                });
            });

            return services;
        }   
    }
}
