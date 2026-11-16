using System.Security.Claims;                   // EX07 - Endpoint GET /api/users/me - using do ClaimTypes
using Microsoft.AspNetCore.Authorization;      // EX04 - Autenticação com JWT - using do [Authorize]
using Microsoft.AspNetCore.Mvc;
using UsersApi.Domain.Entities;                // EX04 - Autenticação com JWT - using do UserProfile (usado no Roles)
using UsersApi.Dtos;
using UsersApi.Services;

namespace UsersApi.Controllers;

/// <summary>Gerenciamento de usuários.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // EX04 - Autenticação com JWT - Aqui exijo login (token) em todo o controller
public class UsersController(IUserService userService) : ControllerBase
{
    // EX05 - Filtro por perfil - Aqui atualizo a descrição do endpoint (ela aparece no Scalar)
    /// <summary>Lista usuários com paginação. Use ?query= para filtrar pelo nome e ?profile= (Admin ou Operator) pelo perfil.</summary>
    [HttpGet]
    [Authorize(Roles = nameof(UserProfile.Admin))] // EX04 - Autenticação com JWT - Aqui exijo o perfil Admin
    [ProducesResponseType<PagedResult<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAll(
        // EX05 - Filtro por perfil - Aqui o código não muda: o [FromQuery] UserFilter já recebe o ?profile= (model binding)
        [FromQuery] UserFilter filter, CancellationToken cancellationToken)
    {
        return Ok(await userService.GetPagedAsync(filter, cancellationToken));
    }

    /// <summary>Busca um usuário pelo id público.</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = nameof(UserProfile.Admin))] // EX04 - Autenticação com JWT - Aqui exijo o perfil Admin
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        return Ok(await userService.GetByIdAsync(id, cancellationToken));
    }

    // EX07 - Endpoint GET /api/users/me - Aqui é o endpoint: devolve o usuário do token. Sem Roles: qualquer perfil logado pode chamar
    // (a rota literal "me" tem prioridade sobre a rota com parâmetro "{id}")
    /// <summary>Devolve os dados do usuário autenticado (o id vem da claim do token).</summary>
    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken cancellationToken)
    {
        // EX07 - Endpoint GET /api/users/me - Aqui leio o id na claim "Sid" do token (nunca da URL nem do corpo da requisição)
        var id = User.FindFirstValue(ClaimTypes.Sid)!;

        return Ok(await userService.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>Cria um usuário.</summary>
    [HttpPost]
    [AllowAnonymous] // EX04 - Autenticação com JWT - Aqui libero o cadastro sem token (senão não dá para criar o primeiro usuário)
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var created = await userService.CreateAsync(request, cancellationToken);

        // 201 + header Location apontando para GET /api/users/{id}
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza nome, e-mail e perfil.</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = nameof(UserProfile.Admin))] // EX04 - Autenticação com JWT - Aqui exijo o perfil Admin
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(id, request, cancellationToken);

        return NoContent();
    }

    /// <summary>Remove um usuário.</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserProfile.Admin))] // EX04 - Autenticação com JWT - Aqui exijo o perfil Admin
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
