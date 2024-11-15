using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;

namespace Application.Tests.TestDoubles.Fakes
{
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _source = new();

        public Task Add(Product product, CancellationToken cancellationToken)
        {
            _source.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetById(int id, CancellationToken cancellationToken)
        {
            var product = _source.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }
    }
} 