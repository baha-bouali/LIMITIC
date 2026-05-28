using LIMTIC.Domain.Entities.Settings;

namespace LIMTIC.Domain.Abstractions.Settings
{
    public interface ISettingsRepository
    {
        Task<LabSettings?> GetAsync();
        Task<bool> UpdateAsync(LabSettings settings);
    }
}

