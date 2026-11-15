using UsersApi.Domain.Entities;
using UsersApi.Dtos;

namespace UsersApi.Repositories;

/// <summary>
/// Repository: esconde o acesso a dados atrás de uma interface.
/// O service pede "um usuário por e-mail" e não sabe se por trás há PostgreSQL, SQL Server ou outro banco.
/// </summary>
public interface IUserRepository
{
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> ListPagedAsync(UserFilter filter, CancellationToken cancellationToken = default);

    // EX03 - Implementando o Unit of Work - Aqui o UpdateAsync sumiu (a entidade já é rastreada) e o DeleteAsync virou Remove, sem gravar
    void Remove(User user);
}
