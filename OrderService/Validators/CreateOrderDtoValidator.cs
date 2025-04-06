using FluentValidation;
using OrderService.Dtos;

namespace OrderService.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required")
                .MaximumLength(200).WithMessage("Shipping address cannot exceed 200 characters");

            

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item")
                .Must(items => items.Count > 0).WithMessage("Order must contain at least one item");

            RuleForEach(x => x.Items).SetValidator(new CreateOrderItemDtoValidator());
        }
    }
}
