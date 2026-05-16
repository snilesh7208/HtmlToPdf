using FluentValidation;
using HtmlToPdf.Application.Commands;

namespace HtmlToPdf.Application.Validators
{
    public class GeneratePdfCommandValidator : AbstractValidator<GeneratePdfCommand>
    {
        public GeneratePdfCommandValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .MaximumLength(100).WithMessage("Identifier cannot exceed 100 characters.");
        }
    }
}
