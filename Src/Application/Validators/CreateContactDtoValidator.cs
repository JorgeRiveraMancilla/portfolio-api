using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateContactDtoValidator : AbstractValidator<CreateContactDto>
    {
        public CreateContactDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Subject is required")
                .MaximumLength(200)
                .WithMessage("Subject must not exceed 200 characters");

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is required")
                .MaximumLength(2000)
                .WithMessage("Message must not exceed 2000 characters");
        }
    }
}
