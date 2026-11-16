using UsersApi.Domain.Entities;
using UsersApi.Domain.Exceptions;
using UsersApi.Dtos;
using UsersApi.Repositories;
using UsersApi.Security;

namespace UsersApi.Services;

// EX03 - Implementando o Unit of Work - Aqui o service recebe o IUnitOfWork (antes recebia o IUserRepository)
public sealed class UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<PagedResult<UserResponse>> GetPagedAsync(UserFilter filter, CancellationToken cancellationToken)
    {
        var users = await unitOfWork.Users.ListPagedAsync(filter, cancellationToken);

        return new PagedResult<UserResponse>(users.Items.Select(UserResponse.From).ToList(), users.Count);
    }

    public async Task<UserResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByPublicIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException(id);

        return UserResponse.From(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);

        await EnsureEmailIsAvailableAsync(email, ignoreId: null, cancellationToken);

        var user = await unitOfWork.Users.AddAsync(new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Profile = request.Profile,
            // EX06 - Novo campo no usuário - Aqui gravo o Phone ao criar (Trim tira espaços das pontas)
            Phone = request.Phone?.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password)
        }, cancellationToken);

        // EX03 - Implementando o Unit of Work - Aqui é o commit depois do AddAsync
        await unitOfWork.CommitAsync(cancellationToken);

        return UserResponse.From(user);
    }

    public async Task UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByPublicIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException(id);

        var email = NormalizeEmail(request.Email);

        await EnsureEmailIsAvailableAsync(email, ignoreId: id, cancellationToken);

        user.Name = request.Name.Trim();
        user.Email = email;
        user.Profile = request.Profile;

        // EX06 - Novo campo no usuário - Aqui atualizo o Phone
        user.Phone = request.Phone?.Trim();

        // EX03 - Implementando o Unit of Work - Aqui é o commit depois de alterar os campos (no lugar do UpdateAsync)
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByPublicIdAsync(id, cancellationToken)
                   ?? throw new NotFoundException(id);

        unitOfWork.Users.Remove(user);

        // EX03 - Implementando o Unit of Work - Aqui é o commit depois do Remove
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private async Task EnsureEmailIsAvailableAsync(string email, string? ignoreId, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Users.FindByEmailAsync(email, cancellationToken);

        if (existing is not null && existing.PublicId != ignoreId)
            throw new ConflictException("Já existe um usuário com este e-mail.");
    }
}
