using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Settings;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly AppDbContext _dbContext;

        public SettingsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LabSettings?> GetAsync()
        {
            return await _dbContext.LabSettings.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(LabSettings settings)
        {
            var existing = await _dbContext.LabSettings.FirstOrDefaultAsync(e => e.Id == settings.Id);
            if (existing == null)
                return false;

            existing.LabName = settings.LabName;
            existing.LabSlogan = settings.LabSlogan;
            existing.ContactEmail = settings.ContactEmail;
            existing.Address = settings.Address;
            existing.Phone = settings.Phone;
            existing.LogoUrl = settings.LogoUrl;

            existing.SmtpHost = settings.SmtpHost;
            existing.SmtpPort = settings.SmtpPort;
            existing.SmtpUsername = settings.SmtpUsername;
            existing.SmtpPasswordHash = settings.SmtpPasswordHash;
            existing.SmtpUseTls = settings.SmtpUseTls;

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
