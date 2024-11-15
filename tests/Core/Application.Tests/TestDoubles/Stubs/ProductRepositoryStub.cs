using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Repositories;

namespace Application.Tests.TestDoubles.Stubs
{
    public class ProductRepositoryStub : IProductRepository
    {
        private readonly Product _product;

        public ProductRepositoryStub(Product product)
        {
            _product = product;
        }

        public async Task Add(Product product, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task<Product?> GetById(int id, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_product);
        }
    }
}
