using Microsoft.EntityFrameworkCore;
using UsersApi.Data;
using UsersApi.Domain.Entities;
using UsersApi.Dtos;

namespace UsersApi.Repositories;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    // EX03 - Implementando o Unit of Work - Aqui o AddAsync só marca a entidade (sem SaveChanges). Quem grava é o UnitOfWork
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);

        return user;
    }

    public Task<User?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<PagedResult<User>> ListPagedAsync(UserFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Query))
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{filter.Query}%"));

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.CreatedAt)
            .Skip((filter.Page - 1) * filter.Quantity)
            .Take(filter.Quantity)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(items, count);
    }

    // EX03 - Implementando o Unit of Work - Aqui o Remove só marca a entidade (sem SaveChanges)
    public void Remove(User user) => context.Users.Remove(user);
}
