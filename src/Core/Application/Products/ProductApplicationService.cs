using Application.Products.Abstractions;
using Application.Products.Dtos.Requests;
using Application.Products.Dtos.Responses;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Products;

public class ProductApplicationService : IProductApplicationService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductApplicationService> _logger;

    public ProductApplicationService(
        IProductRepository productRepository,
        ILogger<ProductApplicationService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<CreateProductResponseDto> Create(CreateProductRequestDto request, CancellationToken cancellationToken)
    {
        ValidateCreateRequest(request);

        _logger.LogInformation("Creating new product with name: {ProductName}", request.Name);

        Product product = new(request.Name, request.Description, request.Price);

        await _productRepository.Add(product, cancellationToken);

        _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

        return new CreateProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task<GetProductResponseDto> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetById(id, cancellationToken) 
            ?? throw new KeyNotFoundException($"Product with id {id} not found");
        
        return new GetProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    private static void ValidateCreateRequest(CreateProductRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name cannot be empty", nameof(request.Name));

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description cannot be empty", nameof(request.Description));

        if (request.Price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(request.Price));
    }
}
