using Domain.Entities.Api;

namespace Api.Data.Interfaces
{
    public interface ICustomerSourceRepository
    {
        Task<IEnumerable<CustomerApiDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    }
}
