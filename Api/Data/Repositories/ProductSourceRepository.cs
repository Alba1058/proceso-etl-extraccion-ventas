using Domain.Entities.Api;
using Api.Data.Interfaces;
using Infrastructure.Extractors.CSV;

namespace Api.Data.Repositories
{
    public class ProductSourceRepository : IProductSourceRepository
    {
        private readonly CsvReaderService _csvReaderService;
        private readonly ISourceFileResolver _sourceFileResolver;

        public ProductSourceRepository(CsvReaderService csvReaderService, ISourceFileResolver sourceFileResolver)
        {
            _csvReaderService = csvReaderService;
            _sourceFileResolver = sourceFileResolver;
        }

        public async Task<IEnumerable<ProductApiDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            var path = _sourceFileResolver.ResolveRequiredPath("ProductCsv", Path.Combine("Data", "products.csv"));
            return await _csvReaderService.ReadAsync<ProductApiDto>(path, cancellationToken);
        }
    }
}
