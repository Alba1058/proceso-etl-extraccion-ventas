using Api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportesController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("ventas-por-producto")]
        public async Task<IActionResult> GetVentasPorProducto(CancellationToken cancellationToken)
        {
            return Ok(await _reportRepository.GetVentasPorProductoAsync(cancellationToken));
        }

        [HttpGet("ventas-por-cliente")]
        public async Task<IActionResult> GetVentasPorCliente(CancellationToken cancellationToken)
        {
            return Ok(await _reportRepository.GetVentasPorClienteAsync(cancellationToken));
        }

        [HttpGet("ventas-por-mes")]
        public async Task<IActionResult> GetVentasPorMes(CancellationToken cancellationToken)
        {
            return Ok(await _reportRepository.GetVentasPorMesAsync(cancellationToken));
        }

        [HttpGet("top-productos")]
        public async Task<IActionResult> GetTopProductos(CancellationToken cancellationToken)
        {
            return Ok(await _reportRepository.GetTopProductosAsync(cancellationToken));
        }

        [HttpGet("top-clientes")]
        public async Task<IActionResult> GetTopClientes(CancellationToken cancellationToken)
        {
            return Ok(await _reportRepository.GetTopClientesAsync(cancellationToken));
        }
    }
}
