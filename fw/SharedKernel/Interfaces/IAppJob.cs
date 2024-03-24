namespace ArbTech.SharedKernel.Interfaces;

public interface IAppJob
{
    Task Execute(IDictionary<string, object> dataMap, CancellationToken cancellationToken);
}
