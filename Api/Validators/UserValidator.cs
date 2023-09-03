using FluentValidation;

namespace Api.Validators;

internal class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(u => u.Id).NotEmpty();
        RuleFor(u => u.Email).EmailAddress().NotEmpty();
    }
}

internal class UserNoIdDtoValidator : AbstractValidator<UserNoIdDto>
{
    public UserNoIdDtoValidator()
    {
        RuleFor(u => u.Email).EmailAddress().NotEmpty();
    }
}