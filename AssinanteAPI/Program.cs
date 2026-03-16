using AssinanteAPI.Application.Interfaces;
using AssinanteAPI.Application.Services;
using AssinanteAPI.Infrastructure.Data;
using AssinanteAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração básica de controllers
builder.Services.AddControllers();

// Entity Framework Core - conecta no SQL Server LocalDB
// A connection string esta no appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injecao de dependencia dos repositorios
// Scoped = uma instancia por request HTTP
builder.Services.AddScoped<IAssinanteRepository, AssinanteRepository>();

// Injecao de dependencia dos servicos de negocio
// Usei GerenciadorAssinantesService para nome mais descritivo
builder.Services.AddScoped<IAssinanteService, GerenciadorAssinantesService>();

// Swagger/OpenAPI para documentacao automatica da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Assinante API", 
        Version = "v1",
        Description = "API para gerenciamento de assinantes"
    });
});

// CORS - permite requisicoes de outras origens (util para frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuracao do ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger UI apenas em dev
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assinante API V1");
        c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
    });
}

// Middleware pipeline
app.UseHttpsRedirection();  // Forca HTTPS
app.UseCors("AllowAll");     // Habilita CORS
app.UseAuthorization();     // Autenticacao (se precisar no futuro)
app.MapControllers();      // Mapeia os controllers

app.Run();
