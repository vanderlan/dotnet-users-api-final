using UsersApi.Domain.Exceptions;
using UsersApi.Dtos;
using UsersApi.Repositories;
using UsersApi.Security;

namespace UsersApi.Services;

// EX05 - Autenticação com JWT - Aqui é a interface do serviço de login (arquivo novo)
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

// EX05 - Autenticação com JWT - Aqui é o serviço de login: confere o e-mail e a senha e gera o token
public sealed class AuthService(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.FindByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);

        // Mesma mensagem para "e-mail não existe" e "senha errada": não revela quais e-mails existem.
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedUserException("E-mail ou senha inválidos.");

        return tokenService.CreateAccessToken(user);
    }
}
