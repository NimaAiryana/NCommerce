using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Repository.Ef.Tests.TestDoubles.Fakes;

public class FakeProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public FakeProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task<Product?> GetById(int id, CancellationToken cancellationToken)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
} 