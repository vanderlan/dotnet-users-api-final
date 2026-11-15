using UsersApi.Data;

namespace UsersApi.Repositories;

// EX03 - Implementando o Unit of Work - Aqui é a implementação do UnitOfWork (arquivo novo)
public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IUserRepository? _users;

    // EX03 - Implementando o Unit of Work - Aqui o UnitOfWork cria o repositório, com o MESMO DbContext
    public IUserRepository Users => _users ??= new UserRepository(context);

    // EX03 - Implementando o Unit of Work - Aqui é o commit: grava tudo de uma vez
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
