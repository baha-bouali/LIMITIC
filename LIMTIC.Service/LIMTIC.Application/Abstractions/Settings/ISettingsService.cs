using LIMTIC.Application.Contracts.Commands.Settings;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Settings;

namespace LIMTIC.Application.Abstractions.Settings
{
    public interface ISettingsService
    {
        Task<Result<SettingsResponseDto>> GetSettingsAsync();
        Task<Result<bool>> UpdateSettingsAsync(SettingsCommand command);
        Task<Result<bool>> TestSmtpAsync(string testEmail);
    }
}
