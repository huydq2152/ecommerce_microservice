using FluentValidation;

namespace Ordering.Application.Features.V1.Order.Common;

public class CreateOrUpdateOrderValidator : AbstractValidator<CreateOrUpdateOrderCommand>
{
    public CreateOrUpdateOrderValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("{FirstName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{FirstName} must not exceed 50 characters.");

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("{LastName} is required.")
            .NotNull()
            .MaximumLength(150).WithMessage("{LastName} must not exceed 150 characters.");

        RuleFor(p => p.EmailAddress)
            .EmailAddress().WithMessage("{EmailAddress} is invalid format.")
            .NotEmpty().WithMessage("{EmailAddress} is required.");

        RuleFor(p => p.ShippingAddress)
            .NotEmpty().WithMessage("{ShippingAddress} is required.");

        RuleFor(p => p.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required.")
            .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");
    }
}