using FluentValidation;
using ProductService.DTos;

namespace ProductService.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.StockQuantity)
                .NotEmpty().WithMessage("Stock quantity is required")
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required");
        }
    }
}
