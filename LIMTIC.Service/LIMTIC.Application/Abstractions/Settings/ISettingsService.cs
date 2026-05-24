using LIMTIC.Application.DTOs.Settings;
using LIMTIC.Application.DTOs;

namespace LIMTIC.Application.Abstractions.Settings
{
    public interface ISettingsService
    {
        Task<Result<SettingsResponseDto>> GetSettingsAsync();
        Task<Result<bool>> UpdateSettingsAsync(UpdateSettingsDto dto);
        Task<Result<bool>> TestSmtpAsync(string testEmail);
    }
}
