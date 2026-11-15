using System.ComponentModel.DataAnnotations;

namespace UsersApi.Options;

// EX04 - Autenticação com JWT - Aqui são as opções do JWT, lidas da seção "JwtOptions" do appsettings (arquivo novo)
public sealed class JwtOptions
{
    public const string SectionName = "JwtOptions";

    /// <summary>HMAC-SHA512 exige pelo menos 64 caracteres.</summary>
    [Required, MinLength(64)]
    public string SymmetricSecurityKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int TokenExpirationMinutes { get; set; } = 60;
}
