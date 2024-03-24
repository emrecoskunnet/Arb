namespace ArbTech.SharedKernel.Interfaces;

public interface IAppBusinessSettingStore
{
    Task<T?> GetConfiguration<T>(string settingKey, CancellationToken cancellationToken = default);

    Task SetConfiguration<T>(string settingKey, T? value,
        CancellationToken cancellationToken = default);
}
