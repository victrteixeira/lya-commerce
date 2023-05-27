using Commerce.Core.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce.Core.Validations
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator() {
            RuleFor(x => x.Id).NotEmpty()
                .WithMessage("O Id do produto não deve ser vazio.")
                .NotNull().WithMessage("O Id do produto não deve ser nulo.");
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("O nome do produto não deve ser vazio.")
                .NotNull().WithMessage("O nome do produto não deve ser nulo.")
                .MaximumLength(50).WithMessage("O nome do produto não deve ultrapassar 50 caracteres.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição do produto não deve ser vazia.")
                .NotNull().WithMessage("A descrição do produto não deve ser nula.")
                .MaximumLength(120).WithMessage("A descrição do produto não deve ultrapassar 120 caracteres.");
            RuleFor(x => x.Price).NotEmpty()
                .WithMessage("O preço do produto não deve ser vazio.")
                .NotNull().WithMessage("O preço do produto não deve ser nulo.")
                .PrecisionScale(5, 2, true).WithMessage("O preço do produto não deve ultrapassar 3 dígitos.");
            RuleFor(x => x.Manufacturer)
                .NotEmpty().WithMessage("A marca do produto não deve ser vazia.");
            //TODO caso nulo, ok, caso tenha algo, validações padrão;
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("A categoria do produto não deve ser vazia.")
                .NotNull().WithMessage("A categoria do produto não deve ser nula.")
                .MaximumLength(20).WithMessage("A categoria do produto não deve ultrapassar 20 caracteres.");
            RuleFor(x => x.SubCategory)
                .NotEmpty().WithMessage("A sub-categoria do produto não deve ser vazia.")
                .NotNull().WithMessage("A sub-categoria do produto não deve ser nula.")
                .MaximumLength(20).WithMessage("A sub-categoria do produto não deve ultrapassar 20 caracteres.");
            //TODO caso nulo, ok, caso tenha algo, validações padrão;
        }
    }
}
