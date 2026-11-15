# dotnet-users-api-final

Projeto **completo** da oficina **C# e .NET na prática** (Tech Week · Facisa): API de usuários em **.NET 10 + PostgreSQL**
com todos os exercícios implementados. O ponto de partida dos alunos é o
**[dotnet-users-api-initial](https://github.com/vanderlan/dotnet-users-api-initial)**, que traz o passo a passo de cada exercício.

## Como achar o código de cada exercício

Todo trecho de código pertencente a um exercício tem um comentário no formato:

```csharp
// EX02 - Adicionando filters - Aqui é a classe do filter
// EX02 - Adicionando filters - Aqui registro o filter
```

Pesquise pelo código do exercício no VS Code (Ctrl+Shift+F): `EX01` para um exercício, `EX0` para todos os pontos de todos os exercícios.

| Exercício | Tema | Onde está |
|-----------|------|-----------|
| **EX01** | Criando as migrations | `src/UsersApi/Data/Migrations/*` · `Program.cs` (comentário no `AddDbContext`) |
| **EX02** | Adicionando filters | `Filters/TimingFilter.cs` · `Program.cs` (registro do filter) |
| **EX03** | Implementando o Unit of Work | `Repositories/IUnitOfWork.cs`, `UnitOfWork.cs`, `IUserRepository.cs`, `UserRepository.cs` · `Services/UserService.cs` · `Program.cs` |
| **EX04** | Autenticação com JWT | `Options/JwtOptions.cs` · `Security/JwtTokenService.cs` · `Dtos/AuthDtos.cs` · `Services/AuthService.cs` · `Controllers/AuthController.cs` e `UsersController.cs` · `Domain/Exceptions/BusinessException.cs` · `ExceptionHandling/DomainExceptionHandler.cs` · `Validators/Validators.cs` · `Program.cs` · `appsettings.Development.json` · `UsersApi.csproj` · `UsersApi.http` |

## Como rodar

```bash
dotnet tool install --global dotnet-ef --version 10.*   # (ou: dotnet tool update --global dotnet-ef --version 10.*)

docker compose up -d                                    # PostgreSQL (5432) + pgAdmin (5050)
dotnet ef database update --project src/UsersApi        # aplica a migration (EX01)
dotnet run --project src/UsersApi --launch-profile http
```

- API: http://localhost:5042 · Scalar: http://localhost:5042/scalar/v1 · pgAdmin: http://localhost:5050
- Requisições prontas (cadastro, login com token, 401/403): [`src/UsersApi/UsersApi.http`](src/UsersApi/UsersApi.http)

## Endpoints

| Verbo    | Rota                | Quem acessa      | Sucesso        | Erros           |
|----------|---------------------|------------------|----------------|-----------------|
| `POST`   | `/api/auth/login`   | qualquer um      | `200` + token  | 400, 401        |
| `POST`   | `/api/users`        | qualquer um      | `201` Location | 400, 409        |
| `GET`    | `/api/users`        | Admin            | `200`          | 401, 403        |
| `GET`    | `/api/users/{id}`   | Admin            | `200`          | 401, 403, 404   |
| `PUT`    | `/api/users/{id}`   | Admin            | `204`          | 400, 401, 403, 404, 409 |
| `DELETE` | `/api/users/{id}`   | Admin            | `204`          | 401, 403, 404   |

> O cadastro (`POST /api/users`) é aberto para dar para criar o primeiro usuário. Em um sistema real, o primeiro
> administrador viria de um *seed* e o perfil não seria escolhido pelo cliente.

## Fluxo de uma requisição

`Controller → Service → UnitOfWork → Repository → AppDbContext → PostgreSQL`
