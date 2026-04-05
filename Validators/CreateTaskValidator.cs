using FluentValidation;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must not be empty")
                .MaximumLength(100).WithMessage("Title must be at most 100 characters");

            RuleFor(x => x.Description)
                   .MaximumLength(500)
                   .When(x => x.Description != null)
                   .WithMessage("Max length is 500");
        }
    }
}
