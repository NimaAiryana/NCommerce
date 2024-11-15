namespace Domain.Aggregates.ProductAggregate.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }

    public Product(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Name = name;
        Description = description;
        Price = price;
    }
}
