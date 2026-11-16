using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Domain.Entities;
using UsersApi.Dtos;
using UsersApi.Options;

namespace UsersApi.Security;

// EX05 - Autenticação com JWT - Aqui é a interface do gerador de token (arquivo novo)
public interface ITokenService
{
    LoginResponse CreateAccessToken(User user);
}

// EX05 - Autenticação com JWT - Aqui é a classe que gera o token JWT (no mesmo arquivo novo)
public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public LoginResponse CreateAccessToken(User user)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_options.TokenExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expires,
            SigningCredentials = new SigningCredentials(CreateKey(_options), SecurityAlgorithms.HmacSha512),

            // EX05 - Autenticação com JWT - Aqui são as claims: o que o token afirma sobre o usuário (o Role alimenta o [Authorize])
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Sid, user.PublicId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Profile.ToString())
            ])
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new LoginResponse(token, expires);
    }

    // EX05 - Autenticação com JWT - Aqui são as regras de validação do token (usadas pelo Program.cs no AddJwtBearer)
    public static TokenValidationParameters CreateValidationParameters(JwtOptions options) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = options.Issuer,
        ValidateAudience = true,
        ValidAudience = options.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = CreateKey(options),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha512],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };

    private static SymmetricSecurityKey CreateKey(JwtOptions options) =>
        new(Encoding.UTF8.GetBytes(options.SymmetricSecurityKey));
}
