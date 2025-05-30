using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using twitter.api.data.DbContexts;
using twitter.api.web.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Adds Swagger Doc
builder.Services.AddTwitterSwagger();

// Adds AutoMapper.
builder.Services.AddAutoMapper(typeof(Program));

// Adds Database
builder.Services.AddTwitterDatabase(config: configuration);

// Adds Services
builder.Services.AddTwitterServices();


var app = builder.Build();

// Aplica migraciones de EF Core automáticamente
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TwitterApiDbContext>();
    db.Database.Migrate();
}

app.UseErrorHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Twitter API V1");
        options.RoutePrefix = string.Empty; // Para que Swagger este en la raíz "/"
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();