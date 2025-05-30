using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using twitter.api.data.DbContexts;
using Microsoft.OpenApi.Models;
using twitter.api.application.Services.Abstractions;
using twitter.api.application.Services;
using twitter.api.web.Services;
using System;

namespace twitter.api.web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTwitterServices(this IServiceCollection services)
        {
            services.AddScoped<ITimelineQueryService, TimelineQueryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITweetService, TweetService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IUserContext, HeaderUserContext>();

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
                    Version = "v1",
                    Description = "API for a Twitter mini-clone. Note: The header 'X-User-Id' is required only for endpoints that act as an authenticated user (timeline, tweets, follow, unfollow, etc.). It is NOT required for admin/test endpoints (create user, etc)."
                });

                // Agrega el header como API Key global (se puede completar desde Swagger UI)
                options.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "X-User-Id",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "User identifier required in each some requests (not required for user creation obviously for example)"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-User-Id"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }   
    }
}
