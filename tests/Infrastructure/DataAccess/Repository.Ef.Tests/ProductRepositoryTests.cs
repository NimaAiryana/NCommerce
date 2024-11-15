using Domain.Aggregates.ProductAggregate.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Repository.Ef.Tests.TestDoubles.Fakes;

namespace Repository.Ef.Tests;

public class ProductRepositoryTests
{
    private readonly FakeProductRepository _repository;
    private readonly AppDbContext _context;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _repository = new FakeProductRepository(_context);
    }

    [Fact]
    public async Task get_an_existing_product_by_id()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var price = 100m;
        
        var product = new Product(name, description, price);
        await _repository.Add(product, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetById(product.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(name);
        result.Description.Should().Be(description);
        result.Price.Should().Be(price);
    }

    [Fact]
    public async Task get_a_non_existing_product_by_id()
    {
        // Act
        var result = await _repository.GetById(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
} 