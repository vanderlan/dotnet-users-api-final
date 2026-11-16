using UsersApi.Domain.Entities;

namespace UsersApi.Dtos;

// DTOs = contrato público da API. A entidade User nunca sai (nem entra) direto pelo controller.

// EX04 - Novo campo no usuário - Aqui o DTO de criação ganha o Phone (opcional: o "= null" permite o cliente não enviar)
public record CreateUserRequest(
    string Name, string Email, string Password, UserProfile Profile, string? Phone = null);

// EX04 - Novo campo no usuário - Aqui o DTO de atualização ganha o Phone (PUT substitui: sem phone no JSON, o valor vira null)
public record UpdateUserRequest(
    string Name, string Email, UserProfile Profile, string? Phone = null);

public record UserResponse(
    string Id,
    string Name,
    string Email,
    // EX04 - Novo campo no usuário - Aqui o DTO de resposta devolve o Phone
    string? Phone,
    UserProfile Profile,
    DateTime CreatedAt,
    DateTime LastUpdated)
{
    // EX04 - Novo campo no usuário - Aqui copio o Phone da entidade para o DTO (a ordem segue a dos parâmetros do record)
    public static UserResponse From(User user) =>
        new(user.PublicId, user.Name, user.Email, user.Phone,
            user.Profile, user.CreatedAt, user.LastUpdated);
}

/// <summary>Filtro de listagem: vem da query string (?page=1&amp;quantity=10&amp;query=ana).</summary>
public class UserFilter
{
    public int Page { get; init; } = 1;
    public int Quantity { get; init; } = 10;
    public string? Query { get; init; }

    // EX02 - Filtro por perfil - Aqui é o novo parâmetro do filtro (?profile=Admin): o model binding converte o texto para o enum
    public UserProfile? Profile { get; init; }
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Count);
