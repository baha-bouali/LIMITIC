using LIMTIC.Domain.Entities.Settings;

namespace LIMTIC.Domain.Abstractions
{
    public interface ISettingsRepository
    {
        Task<LabSettings?> GetAsync();
        Task<bool> UpdateAsync(LabSettings settings);
    }
}

