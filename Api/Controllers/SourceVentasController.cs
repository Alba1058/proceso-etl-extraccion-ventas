using Api.Data.Entities;
using Domain.Entities.Api;
using Api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceVentasController : ControllerBase
    {
        private readonly IVentasSourceRepository _ventasSourceRepository;

        public SourceVentasController(IVentasSourceRepository ventasSourceRepository)
        {
            _ventasSourceRepository = ventasSourceRepository;
        }

        [HttpGet("extract")]
        public async Task<ActionResult<IEnumerable<VentaExtractDto>>> GetVentasParaEtl(CancellationToken cancellationToken)
        {
            var ventas = await _ventasSourceRepository.GetVentasParaEtlAsync(cancellationToken);
            return Ok(ventas);
        }

        [HttpGet("sources")]
        public async Task<IActionResult> GetAvailableSources(CancellationToken cancellationToken)
        {
            var sources = await _ventasSourceRepository.GetAvailableSourcesAsync(cancellationToken);
            return Ok(sources);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSourceSummary(CancellationToken cancellationToken)
        {
            var summary = await _ventasSourceRepository.GetSourceSummaryAsync(cancellationToken);
            return Ok(summary);
        }
    }
}
