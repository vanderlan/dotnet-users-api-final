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
    /// <summary>Lista usuários com paginação. Use ?query= para filtrar pelo nome.</summary>
    [HttpGet]
    [Authorize(Roles = nameof(UserProfile.Admin))] // EX04 - Autenticação com JWT - Aqui exijo o perfil Admin
    [ProducesResponseType<PagedResult<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAll(
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
