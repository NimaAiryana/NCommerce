using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;

namespace Application.Tests.TestDoubles.Dummies
{
    internal class ProductRepositoryDummy : IProductRepository
    {
        public Task Add(Product product, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<Product?> GetById(int id, CancellationToken cancellationToken)
            => Task.FromResult<Product?>(null);
    }
}
