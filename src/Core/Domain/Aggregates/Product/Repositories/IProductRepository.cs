using Domain.Aggregates.ProductAggregate.Entities;

namespace Domain.Aggregates.ProductAggregate.Repositories
{
    public interface IProductRepository
    {
        Task Add(Product product, CancellationToken cancellationToken);
        Task<Product?> GetById(int id, CancellationToken cancellationToken);
    }
}
