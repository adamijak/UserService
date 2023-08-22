using FluentValidation;

namespace Api.Validators;

internal class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Id).NotEmpty();
        RuleFor(u => u.Email).EmailAddress().NotEmpty();
    }
}