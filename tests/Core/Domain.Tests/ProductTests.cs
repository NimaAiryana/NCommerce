using AutoFixture;
using Domain.Aggregates.ProductAggregate.Entities;
using FluentAssertions;

namespace Domain.Tests;

public class ProductTests
{
    [Fact]
    public void create_a_new_product_correctly()
    {
        Fixture fixture = new();
        string name = fixture.Create<string>();
        string description = fixture.Create<string>();
        decimal price = fixture.Create<decimal>();

        Product product = new(name, description, price);

        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
    }
}