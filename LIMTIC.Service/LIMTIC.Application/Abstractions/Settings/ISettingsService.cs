using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Settings;
using static LIMTIC.Application.Contracts.Commands.Settings.SettingsCommands;

namespace LIMTIC.Application.Abstractions.Settings
{
    public interface ISettingsService
    {
        Task<Result<SettingsResponseDto>> GetSettingsAsync();
        Task<Result<bool>> UpdateSettingsAsync(SettingsCommand command);
        Task<Result<bool>> TestSmtpAsync(string testEmail);
    }
}
