using FluentValidation;

namespace Ordering.Application.Features.V1.Order.Commands.UpdateOrder;

public class UpdateOrderCommandValidator: AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{Id} is required.");
    }
}