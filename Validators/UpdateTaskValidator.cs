using FluentValidation;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Validators
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}
