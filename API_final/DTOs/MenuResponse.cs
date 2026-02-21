namespace API_final.DTOs;
public class RestaurantDto
{
    public int Id { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DiscountPercent { get; set; }
    public bool HappyHourEnabled { get; set; }
    public bool IsFavorite { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; // Útil para el front-end
}