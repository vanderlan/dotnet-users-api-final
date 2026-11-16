using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer; // EX04 - Autenticação com JWT - using do JwtBearer
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;                  // EX04 - Autenticação com JWT - using do IOptions (JwtOptions)
using Scalar.AspNetCore;
using UsersApi.Data;
using UsersApi.Data.Interceptors;
using UsersApi.ExceptionHandling;
using UsersApi.Filters;
using UsersApi.Options;                              // EX04 - Autenticação com JWT - using da JwtOptions
using UsersApi.Repositories;
using UsersApi.Security;
using UsersApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ───────────────────────────── 1. SERVIÇOS (container de DI) ─────────────────────────────

// Controllers + filtros globais (rodam antes de toda action)
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<AutoValidationActionFilter>();

        // EX02 - Adicionando filters - Aqui registro o filter (TimingFilter)
        options.Filters.Add<TimingFilter>();
    })
    // Enums trafegam como texto ("Admin") em vez de número.
    .AddJsonOptions(json => json.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter()));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Erros padronizados: Problem Details (RFC 9457) + nosso handler de exceções de domínio
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

// Entity Framework Core + PostgreSQL
// EX01 - Criando as migrations - O esquema do banco vem das migrations (pasta Data/Migrations), não deste código
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<TimestampsInterceptor>();
builder.Services.AddDbContext<AppDbContext>((provider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
    options.AddInterceptors(
        provider.GetRequiredService<TimestampsInterceptor>());
});

// Camadas da aplicação. Scoped = uma instância por requisição HTTP.
// EX03 - Implementando o Unit of Work - Aqui registro o UnitOfWork (no lugar do IUserRepository, que ele mesmo cria)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

// EX04 - Autenticação com JWT - Aqui registro o serviço de login e o gerador de token
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

// EX04 - Autenticação com JWT - Aqui registro as opções do JWT (validadas na inicialização)
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// EX04 - Autenticação com JWT - Aqui configuro a autenticação Bearer e a autorização
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
        bearer.TokenValidationParameters =
            JwtTokenService.CreateValidationParameters(jwt.Value));
builder.Services.AddAuthorization();

// Documentação: OpenAPI + Scalar
builder.Services.AddOpenApi();

var app = builder.Build();

// ───────────────────────────── 2. PIPELINE HTTP (ordem importa!) ─────────────────────────────

app.UseExceptionHandler();   // 1º: captura qualquer exceção das etapas seguintes
app.UseStatusCodePages();    // 404 e outros status vazios viram Problem Details

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                 // /openapi/v1.json
    app.MapScalarApiReference();      // /scalar/v1
}

// EX04 - Autenticação com JWT - Aqui entram a autenticação e a autorização no pipeline (antes do MapControllers)
app.UseAuthentication();     // QUEM é você?  (lê e valida o JWT)
app.UseAuthorization();      // O QUE você pode fazer?  ([Authorize])

app.MapControllers();

await app.RunAsync();
