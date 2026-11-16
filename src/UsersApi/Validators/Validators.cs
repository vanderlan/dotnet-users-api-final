using FluentValidation;
using UsersApi.Dtos;

namespace UsersApi.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail é inválido.")
            .MaximumLength(200);

        RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");

        RuleFor(x => x.Profile).IsInEnum().WithMessage("Perfil inválido.");

        // EX04 - Novo campo no usuário - Aqui valido o tamanho do Phone no cadastro (a coluna aceita até 20 caracteres)
        RuleFor(x => x.Phone).MaximumLength(20)
            .WithMessage("O telefone deve ter no máximo 20 caracteres.");
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail é inválido.")
            .MaximumLength(200);

        RuleFor(x => x.Profile).IsInEnum().WithMessage("Perfil inválido.");

        // EX04 - Novo campo no usuário - Aqui valido o tamanho do Phone na atualização
        RuleFor(x => x.Phone).MaximumLength(20)
            .WithMessage("O telefone deve ter no máximo 20 caracteres.");
    }
}

// EX05 - Autenticação com JWT - Aqui é o validator do login (e-mail e senha obrigatórios)
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
