using FluentValidation;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Max length is 100");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Max length is 500");
        }
    }
}
