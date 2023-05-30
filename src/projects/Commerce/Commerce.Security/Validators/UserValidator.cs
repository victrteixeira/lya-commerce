using Commerce.Security.Models;
using FluentValidation;

namespace Commerce.Security.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName)
            //.NotNull().WithMessage("O primeiro nome não pode ser nulo.")
            .NotEmpty().WithMessage("O primeiro nome não pode ficar vazio.")
            .Length(1, 20).WithMessage("No máximo 20 caracteres são permitidos.")
            .NotEqual(y => y.LastName).WithMessage("O primeiro nome não pode ser igual ao seu último nome.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Apenas letras e números são permitidos.");
        
        RuleFor(x => x.LastName)
            //.NotNull().WithMessage("O último nome não pode ser nulo.")
            .NotEmpty().WithMessage("O último nome não pode ficar vazio.")
            .Length(1, 20).WithMessage("No máximo 20 caracteres são permitidos.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Apenas letras e números são permitidos.");

        RuleFor(x => x.EmailAddress)
            //.NotNull().WithMessage("O e-mail não pode ser nulo.")
            .NotEmpty().WithMessage("O e-mail não pode ficar vazio.")
            .Custom((email, context) =>
            {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    context.AddFailure("O Email não deve terminar com ponto.");
                }

                if (trimmedEmail.StartsWith("."))
                {
                    context.AddFailure("O Email não deve começar com um ponto.");
                }

                if (trimmedEmail.Contains(".."))
                {
                    context.AddFailure("O Email não deve conter pontos consecutivos.");
                }

                var atCount = trimmedEmail.Count(ch => ch == '@');

                if (atCount > 1)
                {
                    context.AddFailure("O Email não deve conter mais de um 'arroba'.");
                }
                
                if (trimmedEmail.Contains(" "))
                {
                    context.AddFailure("O Email não deve conter espaços em branco.");
                }

                if (trimmedEmail.Any(ch => !char.IsLetterOrDigit(ch) && ch != '.' && ch != '@'))
                {
                    context.AddFailure("O Email não deve conter caracteres especiais.");
                }

                var parts = trimmedEmail.Split('@');
                if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                {
                    context.AddFailure("O Email deve conter uma parte local e uma parte de domínio.");
                }
                else if (!parts[1].Contains("."))
                {
                    context.AddFailure("A parte do domínio do Email deve conter um ponto.");
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
    }
}