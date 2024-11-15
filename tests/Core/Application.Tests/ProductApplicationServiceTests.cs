using Application.Products;
using Application.Products.Dtos.Requests;
using Application.Products.Dtos.Responses;
using Application.Tests.TestDoubles.Dummies;
using Application.Tests.TestDoubles.Fakes;
using Application.Tests.TestDoubles.Mocks;
using Application.Tests.TestDoubles.Stubs;
using AutoFixture;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;
using FluentAssertions;
using Moq;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Application.Tests;

public class ProductApplicationServiceTests
{
    [Fact]
    public async Task create_a_new_product_correctly()
    {
        Fixture fixture = new();
        string name = fixture.Create<string>();
        string description = fixture.Create<string>();
        decimal price = 100m;
        CreateProductRequestDto request = new() { Name = name, Description = description, Price = price };
        IProductRepository productRepositoryDummy = new ProductRepositoryDummy();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        ProductApplicationService service = new(productRepositoryDummy, loggerDummy);

        CreateProductResponseDto response = await service.Create(request, CancellationToken.None);

        response.Name.Should().Be(name);
        response.Description.Should().Be(description);
        response.Price.Should().Be(price);
    }

    [Fact]
    public async Task add_product_in_repository_once()
    {
        Fixture fixture = new();
        var mockRepository = new Mock<IProductRepository>();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(mockRepository.Object, loggerDummy);
        CreateProductRequestDto request = new() { Name = fixture.Create<string>(), Description = fixture.Create<string>() };

        await productService.Create(request, CancellationToken.None);

        mockRepository.Verify(
            r => r.Add(It.IsAny<Product>(), CancellationToken.None),
            Moq.Times.Once,
            "The repository Add method was not called exactly once.");
    }

    [Fact]
    public async Task get_a_product_by_id()
    {
        // Arrange
        int id = 1;
        string name = "Nima";
        string description = "Xpress";
        decimal price = 100m;
        Product product = new(name, description, price);
        var mockRepository = new Mock<IProductRepository>();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();

        mockRepository
            .Setup(x => x.GetById(id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Product?>(product));

        var productService = new ProductApplicationService(mockRepository.Object, loggerDummy);

        // Act
        GetProductResponseDto response = await productService.GetById(id, CancellationToken.None);

        // Assert
        response.Name.Should().Be(product.Name);
        response.Description.Should().Be(product.Description);
        response.Price.Should().Be(product.Price);
    }

    [Fact]
    public async Task get_a_product_by_id_manually_version()
    {
        // Arrange
        int id = 1;
        string name = "Nima";
        string description = "Xpress";
        decimal price = 100m;
        Product product = new(name, description, price);
        IProductRepository repositoryStub = new ProductRepositoryStub(product);
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();

        var productService = new ProductApplicationService(repositoryStub, loggerDummy);

        // Act
        GetProductResponseDto response = await productService.GetById(id, CancellationToken.None);

        // Assert
        response.Name.Should().Be(product.Name);
        response.Description.Should().Be(product.Description);
        response.Price.Should().Be(product.Price);
    }

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product("Test", "Test", 100);
        decimal price = 100m;
        Mock<IProductRepository> _productRepositoryMock = new();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        
        _productRepositoryMock
            .Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Product?>(product));

        var productService = new ProductApplicationService(_productRepositoryMock.Object, loggerDummy);

        // Act
        var result = await productService.GetById(1, CancellationToken.None);

        // Assert
        result.Price.Should().Be(price);
    }

    [Fact]
    public async Task product_not_found_in_catalog()
    {
        // Arrange
        var productRepository = new FakeProductRepository();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(productRepository, loggerDummy);
        var nonExistentId = 1;

        // Act
        Func<Task> act = async () => await productService.GetById(nonExistentId, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with id {nonExistentId} not found");
    }

    [Fact]
    public async Task product_catalog_saves_new_items()
    {
        // Arrange
        var productRepository = new FakeProductRepository();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(productRepository, loggerDummy);
        var request = new CreateProductRequestDto 
        { 
            Name = "Laptop", 
            Description = "Gaming Laptop", 
            Price = 1500 
        };

        // Act
        var response = await productService.Create(request, CancellationToken.None);

        // Assert
        var savedProduct = await productRepository.GetById(response.Id, CancellationToken.None);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task product_price_cannot_be_negative()
    {
        // Arrange
        var mockRepository = new MockProductRepository();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(mockRepository, loggerDummy);
        var request = new CreateProductRequestDto 
        { 
            Name = "Test Product", 
            Description = "Test Description", 
            Price = -100 
        };

        // Act
        Func<Task> act = async () => await productService.Create(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Price cannot be negative (Parameter 'Price')");
    }

    [Fact]
    public async Task product_name_cannot_be_empty()
    {
        // Arrange
        var mockRepository = new MockProductRepository();
        var loggerDummy = A.Dummy<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(mockRepository, loggerDummy);
        var request = new CreateProductRequestDto 
        { 
            Name = "", 
            Description = "Test Description", 
            Price = 100 
        };

        // Act
        Func<Task> act = async () => await productService.Create(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Name cannot be empty (Parameter 'Name')");

        // Verify repository was never called
        mockRepository.VerifyAdd(0);
    }

    [Fact]
    public async Task logs_product_creation_with_manual_mock()
    {
        // Arrange
        var mockRepository = new MockProductRepository();
        var mockService = new MockProductApplicationService(mockRepository);
        var request = new CreateProductRequestDto 
        { 
            Name = "Test Product", 
            Description = "Test Description", 
            Price = 100 
        };

        // Act
        await mockService.Create(request, CancellationToken.None);

        // Assert
        mockService.VerifyLoggedMessage("Creating new product with name: Test Product");
        mockService.VerifyLoggedMessage("Product created successfully");
        mockService.VerifyLoggedMessageCount(2);
        mockService.VerifyLogLevel("Creating new product", LogLevel.Information);
    }

    [Fact]
    public async Task logs_product_creation_with_moq()
    {
        // Arrange
        var mockRepository = new Mock<IProductRepository>();
        var mockLogger = new Mock<ILogger<ProductApplicationService>>();
        var productService = new ProductApplicationService(mockRepository.Object, mockLogger.Object);
        var request = new CreateProductRequestDto 
        { 
            Name = "Test Product", 
            Description = "Test Description", 
            Price = 100 
        };

        // Act
        await productService.Create(request, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Creating new product")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Moq.Times.Once);

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Product created successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Moq.Times.Once);
    }
}