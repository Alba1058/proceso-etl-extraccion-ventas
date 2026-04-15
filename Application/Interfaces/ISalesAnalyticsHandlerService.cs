using Application.Result;

namespace Application.Interfaces
{
    public interface ISalesAnalyticsHandlerService
    {
        Task<ServiceResult> ProcessSalesAnalyticsAsync(CancellationToken cancellationToken = default);
    }
}
