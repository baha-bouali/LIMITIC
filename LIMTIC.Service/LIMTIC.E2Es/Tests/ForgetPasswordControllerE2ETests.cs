using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI.Models.Auth.ForgetPassword;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.ResetPassword;
using LIMTIC.WebAPI.Models.Auth.VerifyResetCode;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class ForgetPasswordControllerE2ETests : BaseE2ETests
    {
        private readonly string _mailHogApiUrl;

        public ForgetPasswordControllerE2ETests(
            PostgresFixture postgresFixture,
            MailHogFixture mailHogFixture)
            : base(postgresFixture, mailHogFixture)   
        {
            _mailHogApiUrl = mailHogFixture.ApiUrl;
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_ReturnsOk()
        {
            var userEmail = "forgot.test1@example.com";
            await CreateTestUser(userEmail, "Pass1!");

            var response = await Client.ForgotPassword(new ForgetPasswordRequest { Email = userEmail });

            Assert.True(response.IsSuccessStatusCode,
                $"Expected 200 but got {(int)response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task ForgotPassword_UnknownEmail_ReturnsBadRequest()
        {
            var response = await Client.ForgotPassword(
                new ForgetPasswordRequest { Email = "nobody@nowhere.com" });

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task VerifyOTP_ValidOtp_ReturnsResetToken()
        {
            var userEmail = "forgot.test2@example.com";
            await CreateTestUser(userEmail, "Pass1!");
            await Client.ForgotPassword(new ForgetPasswordRequest { Email = userEmail });

            var otp = await GetOtpFromMailHog(userEmail);
            Assert.NotNull(otp);

            var verifyResponse = await Client.VerifyOTP(
                new VerifyResetCodeRequest { Email = userEmail, OtpToken = otp });

            Assert.True(verifyResponse.IsSuccessStatusCode,
                $"Expected 200 but got {(int)verifyResponse.StatusCode}. Body: {await verifyResponse.Content.ReadAsStringAsync()}");

            var body = await verifyResponse.Content.ReadFromJsonAsync<VerifyResetCodeResponse>();
            Assert.NotNull(body?.ResetToken);
            Assert.NotEmpty(body!.ResetToken);
        }

        [Fact]
        public async Task VerifyOTP_WrongOtp_ReturnsBadRequest()
        {
            var userEmail = "forgot.test3@example.com";
            await CreateTestUser(userEmail, "Pass1!");
            await Client.ForgotPassword(new ForgetPasswordRequest { Email = userEmail });

            var verifyResponse = await Client.VerifyOTP(
                new VerifyResetCodeRequest { Email = userEmail, OtpToken = "000000" });

            Assert.False(verifyResponse.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, verifyResponse.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_FullFlow_ShouldSucceed()
        {
            var userEmail = "forgot.test4@example.com";
            var newPassword = "NewPass2!";

            await CreateTestUser(userEmail, "OldPass1!");

            // Step 1 - request OTP
            var forgotResponse = await Client.ForgotPassword(
                new ForgetPasswordRequest { Email = userEmail });
            Assert.True(forgotResponse.IsSuccessStatusCode,
                $"ForgotPassword failed: {await forgotResponse.Content.ReadAsStringAsync()}");

            // Step 2 - get OTP from MailHog
            var otp = await GetOtpFromMailHog(userEmail);
            Assert.NotNull(otp);

            // Step 3 - verify OTP
            var verifyResponse = await Client.VerifyOTP(
                new VerifyResetCodeRequest { Email = userEmail, OtpToken = otp });
            Assert.True(verifyResponse.IsSuccessStatusCode,
                $"VerifyOTP failed: {await verifyResponse.Content.ReadAsStringAsync()}");

            var verifyBody = await verifyResponse.Content.ReadFromJsonAsync<VerifyResetCodeResponse>();
            Assert.NotNull(verifyBody?.ResetToken);

            // Step 4 - reset password
            var resetResponse = await Client.ResetPassword(new ResetPasswordRequest
            {
                Email = userEmail,
                NewPassword = newPassword,
                ResetToken = verifyBody!.ResetToken
            });
            Assert.True(resetResponse.IsSuccessStatusCode,
                $"ResetPassword failed: {await resetResponse.Content.ReadAsStringAsync()}");

            // Step 5 - confirm new password works
            var loginResponse = await Client.AuthenticateUser(new LoginRequest(userEmail, newPassword));
            Assert.NotNull(loginResponse);
            Assert.NotEmpty(loginResponse!.AccessToken);
        }

        [Fact]
        public async Task ResetPassword_WithInvalidResetToken_ReturnsBadRequest()
        {
            var userEmail = "forgot.test5@example.com";
            await CreateTestUser(userEmail, "Pass1!");
            await Client.ForgotPassword(new ForgetPasswordRequest { Email = userEmail });

            var otp = await GetOtpFromMailHog(userEmail);
            Assert.NotNull(otp);

            await Client.VerifyOTP(new VerifyResetCodeRequest { Email = userEmail, OtpToken = otp });

            var resetResponse = await Client.ResetPassword(new ResetPasswordRequest
            {
                Email = userEmail,
                NewPassword = "NewPass2!",
                ResetToken = "invalid-token-that-will-never-match"
            });

            Assert.False(resetResponse.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, resetResponse.StatusCode);
        }

        /// <summary>
        /// Polls MailHog until the email arrives, then decodes the body and
        /// extracts the 6-digit OTP that was rendered into the HTML template.
        /// </summary>
        private async Task<string?> GetOtpFromMailHog(string toEmail)
        {
            using var http = new HttpClient();

            for (int attempt = 0; attempt < 10; attempt++)
            {
                await Task.Delay(500);

                MailHogMessagesResponse? messages;
                try
                {
                    messages = await http.GetFromJsonAsync<MailHogMessagesResponse>(
                        $"{_mailHogApiUrl}/api/v2/messages");
                }
                catch { continue; }

                if (messages?.Items == null || messages.Items.Count == 0)
                    continue;

                var mail = messages.Items
                    .Where(m => m.To != null && m.To.Any(a =>
                        string.Equals($"{a.Mailbox}@{a.Domain}", toEmail,
                            StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(m => m.Created)
                    .FirstOrDefault();

                if (mail == null)
                    continue;

                var rawBody = mail.Content?.Body ?? string.Empty;

                // Try base64 decode first (System.Net.Mail encodes HTML as base64)
                var body = rawBody;
                try
                {
                    body = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(rawBody.Trim()));
                }
                catch
                {
                    // Not base64 — use raw (quoted-printable path below handles it)
                    body = rawBody;
                }

                // Quoted-printable: remove soft line breaks (=\r\n or =\n)
                body = System.Text.RegularExpressions.Regex.Replace(body, @"=\r?\n", "");

                // The OTP sits alone between HTML tags — strip all tags and find 6 digits
                body = System.Text.RegularExpressions.Regex.Replace(body, @"<[^>]+>", " ");

                var match = System.Text.RegularExpressions.Regex.Match(body, @"(?<!\d)(\d{6})(?!\d)");
                if (match.Success)
                    return match.Groups[1].Value;
            }

            return null;
        }

        private async Task CreateTestUser(string email, string password)
        {
            var token = await LoginAsSuperAdmin();
            await Client.AddUser(new LIMTIC.WebAPI.Models.UserManagement.CreateUser.CreateUserRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = email,
                Password = password,
                Role = UserRole.Admin,
                IsActive = true
            }, token);
        }

        private async Task<string> LoginAsSuperAdmin()
        {
            var auth = await Client.AuthenticateUser(new LoginRequest("admin@test.com", "AdminPassword"));
            return auth!.AccessToken;
        }

        private void SeedAdminUser(AppDbContext db, IPasswordHasher passwordHasher)
        {
            if (db.Users.Any(u => u.Email == "admin@test.com"))
                return;

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Admin",
                IsActive = true,
                Email = "admin@test.com",
                Role = UserRole.Super_Admin,
                PasswordHash = passwordHasher.HashPassword("AdminPassword"),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = Guid.Empty
            });
            db.SaveChanges();
        }

        // -----------------------------------------------------------------------
        // MailHog API models — matches /api/v2/messages JSON shape exactly
        // -----------------------------------------------------------------------

        private sealed class MailHogMessagesResponse
        {
            [JsonPropertyName("items")]
            public List<MailHogMessage> Items { get; set; } = new();
        }

        private sealed class MailHogMessage
        {
            [JsonPropertyName("Content")]
            public MailHogContent? Content { get; set; }

            [JsonPropertyName("To")]
            public List<MailHogAddress>? To { get; set; }

            [JsonPropertyName("Created")]
            public DateTime Created { get; set; }
        }

        private sealed class MailHogContent
        {
            [JsonPropertyName("Body")]
            public string? Body { get; set; }
        }

        private sealed class MailHogAddress
        {
            [JsonPropertyName("Mailbox")]
            public string? Mailbox { get; set; }

            [JsonPropertyName("Domain")]
            public string? Domain { get; set; }
        }
    }
}