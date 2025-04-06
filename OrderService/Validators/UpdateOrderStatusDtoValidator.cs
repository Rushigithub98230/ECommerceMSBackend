using FluentValidation;
using OrderService.Dtos;

namespace OrderService.Validators
{
    public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => status == "Processing" || status == "Shipped" || status == "Delivered" || status == "Cancelled")
                .WithMessage("Status must be one of: Processing, Shipped, Delivered, Cancelled");
        }
    }
}
