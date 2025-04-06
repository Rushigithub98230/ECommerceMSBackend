namespace ProductService.DTos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid SellerId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class UpdateStockDto
    {
        public int Quantity { get; set; }
    }
}
