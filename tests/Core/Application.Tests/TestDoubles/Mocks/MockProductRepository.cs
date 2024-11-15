using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;

namespace Application.Tests.TestDoubles.Mocks;

public class MockProductRepository : IProductRepository
{
    private readonly List<(Product Product, int Id, CancellationToken Token)> _addCalls = new();
    private readonly Dictionary<int, Product> _getByIdResults = new();

    public void SetupGetById(int id, Product result)
    {
        _getByIdResults[id] = result;
    }

    public Task Add(Product product, CancellationToken cancellationToken)
    {
        _addCalls.Add((product, product.Id, cancellationToken));
        return Task.CompletedTask;
    }

    public Task<Product?> GetById(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_getByIdResults.GetValueOrDefault(id));
    }

    public void VerifyAdd(int times)
    {
        if (_addCalls.Count != times)
            throw new Exception($"Add was called {_addCalls.Count} times, but expected {times} times");
    }
} 