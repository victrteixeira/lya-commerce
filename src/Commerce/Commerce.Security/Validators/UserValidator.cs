using Commerce.Security.Models;
using FluentValidation;

namespace Commerce.Security.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("O primeiro nome não pode ser nulo.")
            .NotEmpty().WithMessage("O primeiro nome não pode ficar vazio.")
            .Length(1, 20).WithMessage("No máximo 20 caracteres são permitidos.")
            .NotEqual(y => y.LastName).WithMessage("O primeiro nome não pode ser igual ao seu último nome.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Apenas letras e números são permitidos.");
        
        RuleFor(x => x.LastName)
            .NotNull().WithMessage("O último nome não pode ser nulo.")
            .NotEmpty().WithMessage("O último nome não pode ficar vazio.")
            .Length(1, 20).WithMessage("No máximo 20 caracteres são permitidos.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Apenas letras e números são permitidos.");

        RuleFor(x => x.EmailAddress)
            .NotNull().WithMessage("O e-mail não pode ser nulo.")
            .NotEmpty().WithMessage("O e-mail não pode ficar vazio.")
            .Custom((email, context) =>
            {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    context.AddFailure("O Email não deve terminar com ponto.");
                }

                try
                {
                    var _ = new System.Net.Mail.MailAddress(email);
                }
                catch
                {
                    context.AddFailure("O email não é válido.");
                }
            });

        RuleFor(x => x.Password)
            .Custom((pwd, context) =>
            {
                bool isValid = pwd.Any(char.IsUpper)
                               && pwd.Any(char.IsLower)
                               && pwd.Any(char.IsDigit)
                               && pwd.Contains(' ')
                               && pwd.Length >= 8;
                
                if (!isValid)
                    context.AddFailure("Senha não é válida. Precisa conter ao menos uma letra maiúscula, uma minúscula, um digito númerico, e precisa ser maior ou igual a 8 digitos.");
            });
    }
}