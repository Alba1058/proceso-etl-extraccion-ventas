using Domain.Entities.Api;

namespace Api.Data.Interfaces
{
    public interface IProductSourceRepository
    {
        Task<IEnumerable<ProductApiDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    }
}
