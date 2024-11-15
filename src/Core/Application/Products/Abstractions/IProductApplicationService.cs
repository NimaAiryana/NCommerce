using Application.Products.Dtos.Requests;
using Application.Products.Dtos.Responses;

namespace Application.Products.Abstractions;

public interface IProductApplicationService
{
    Task<CreateProductResponseDto> Create(CreateProductRequestDto request, CancellationToken cancellationToken);
    Task<GetProductResponseDto> GetById(int id, CancellationToken cancellationToken);
}
