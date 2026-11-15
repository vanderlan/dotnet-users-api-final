namespace UsersApi.Repositories;

// EX03 - Implementando o Unit of Work - Aqui é a interface do UnitOfWork (arquivo novo)
/// <summary>
/// Um UnitOfWork por requisição (Scoped). Todos os repositórios compartilham o mesmo
/// DbContext, então um único <see cref="CommitAsync"/> grava tudo de uma vez (ou nada).
/// </summary>
public interface IUnitOfWork
{
    IUserRepository Users { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
