namespace Application.Interfaces
{
    public interface IETLService
    {
        Task EjecutarProcesoETLAsync(CancellationToken cancellationToken = default);
    }
}
