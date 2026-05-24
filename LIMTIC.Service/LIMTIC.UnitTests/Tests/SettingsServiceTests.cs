using LIMTIC.Application.DTOs.Settings;
using LIMTIC.UnitTests.Base;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace LIMTIC.UnitTests.Tests
{
    public class SettingsServiceTests : BaseTests
    {
        [Fact]
        public void DataProtection_ProtectsAndUnprotectsBytes()
        {
            using var scope = ServiceProvider.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
            var protector = provider.CreateProtector("smtp-password");

            var plain = "MySecret123!";
            var plainBytes = System.Text.Encoding.UTF8.GetBytes(plain);
            var protectedBytes = protector.Protect(plainBytes);
            var base64 = Convert.ToBase64String(protectedBytes);

            var decoded = Convert.FromBase64String(base64);
            var unprotected = protector.Unprotect(decoded);
            var result = System.Text.Encoding.UTF8.GetString(unprotected);

            Assert.Equal(plain, result);
        }

        [Fact]
        public void SettingsDto_DoesNotContainPasswordProperty()
        {
            var dto = new SettingsResponseDto
            {
                Identity = new IdentityDto { LabName = "x" },
                Smtp = new SmtpResponseDto { Host = "h", Port = 25, Username = "u", UseTls = false }
            };

            var smtpProps = dto.Smtp.GetType().GetProperties().Select(p => p.Name);
            Assert.DoesNotContain("Password", smtpProps);
        }
    }
}
