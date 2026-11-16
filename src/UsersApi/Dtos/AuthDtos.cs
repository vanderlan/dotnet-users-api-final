namespace UsersApi.Dtos;

// EX05 - Autenticação com JWT - Aqui são os DTOs do login: o que entra (e-mail e senha)
public record LoginRequest(string Email, string Password);

// EX05 - Autenticação com JWT - Aqui é o DTO que sai do login: o token e quando ele expira
public record LoginResponse(string Token, DateTime Expiration);
