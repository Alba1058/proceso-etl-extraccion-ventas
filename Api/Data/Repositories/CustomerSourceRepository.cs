using Domain.Entities.Api;
using Api.Data.Interfaces;
using Infrastructure.Extractors.CSV;

namespace Api.Data.Repositories
{
    public class CustomerSourceRepository : ICustomerSourceRepository
    {
        private readonly CsvReaderService _csvReaderService;
        private readonly ISourceFileResolver _sourceFileResolver;

        public CustomerSourceRepository(CsvReaderService csvReaderService, ISourceFileResolver sourceFileResolver)
        {
            _csvReaderService = csvReaderService;
            _sourceFileResolver = sourceFileResolver;
        }

        public async Task<IEnumerable<CustomerApiDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
        {
            var path = _sourceFileResolver.ResolveRequiredPath("CustomerCsv", Path.Combine("Data", "customers.csv"));
            return await _csvReaderService.ReadAsync<CustomerApiDto>(path, cancellationToken);
        }
    }
}
