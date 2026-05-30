using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.Contacts;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.Contracts.Commands.ForgetPassword;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.Contracts.Commands.ResetPassword;
using LIMTIC.Application.Contracts.Commands.Settings;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.Contracts.Commands.VerifyResetCode;
using LIMTIC.Application.Contracts.Queries.AuditLogs;
using LIMTIC.Application.Contracts.Queries.Events;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs.AuditLogs;
using LIMTIC.Application.DTOs.Auth;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.DTOs.ResearchAxis;
using LIMTIC.Application.DTOs.Settings;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.Domain.Enums;
using LIMTIC.WebAPI;
using LIMTIC.WebAPI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LIMTIC.E2Es.Extensions
{
    public static class ClientExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static HttpRequestMessage CreateRequest(string endpoint, HttpMethod httpMethod, string? accessToken = null)
        {
            var request = new HttpRequestMessage(httpMethod, endpoint);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return request;
        }

        private static async Task<BaseResponse<T>> SendAndDeserializeAsync<T>(
            this HttpClient client,
            HttpRequestMessage request,
            CancellationToken cancellationToken = default)
        {
            var response = await client.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                var result = JsonSerializer.Deserialize<BaseResponse<T>>(content, _jsonOptions);
                if (result != null)
                {
                    return result;
                }

                // If deserialization fails, create a fallback response
                return new BaseResponse<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? null : $"Request failed with status {response.StatusCode}",
                    Data = default
                };
            }
            catch (JsonException)
            {
                return new BaseResponse<T>
                {
                    Success = false,
                    Message = $"Failed to deserialize response: {content}",
                    Data = default
                };
            }
        }

        private static async Task<BaseResponse<T>> PostAndDeserializeAsync<T>(
            this HttpClient client,
            string endpoint,
            object? content = null,
            string? accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(endpoint, HttpMethod.Post, accessToken);
            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }
            return await client.SendAndDeserializeAsync<T>(request, cancellationToken);
        }

        private static async Task<BaseResponse<T>> PutAndDeserializeAsync<T>(
            this HttpClient client,
            string endpoint,
            object? content = null,
            string? accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(endpoint, HttpMethod.Put, accessToken);
            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }
            return await client.SendAndDeserializeAsync<T>(request, cancellationToken);
        }

        private static async Task<BaseResponse<T>> DeleteAndDeserializeAsync<T>(
            this HttpClient client,
            string endpoint,
            string? accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(endpoint, HttpMethod.Delete, accessToken);
            return await client.SendAndDeserializeAsync<T>(request, cancellationToken);
        }

        private static async Task<BaseResponse<T>> GetAndDeserializeAsync<T>(
            this HttpClient client,
            string endpoint,
            string? accessToken = null,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(endpoint, HttpMethod.Get, accessToken);
            return await client.SendAndDeserializeAsync<T>(request, cancellationToken);
        }

        // ── Auth ───────────────────────────────────────────────────────────────────

        public static async Task<BaseResponse<LoginDto>> AuthenticateUser(
            this HttpClient client, LoginCommand command)
        {
            return await client.PostAndDeserializeAsync<LoginDto>("api/auth/login", command);
        }

        public static async Task<BaseResponse<LoginDto>> RefreshToken(this HttpClient client)
        {
            return await client.PostAndDeserializeAsync<LoginDto>("api/auth/refreshToken");
        }

        public static async Task<BaseResponse<bool>> Logout(this HttpClient client, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>("api/auth/logout", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> ForgotPassword(
            this HttpClient client, ForgetPasswordCommand command)
        {
            var result = await client.PostAndDeserializeAsync<bool>("api/auth/forgotPassword", command);
            return result;
        }

        public static async Task<BaseResponse<string>> VerifyOTP(
            this HttpClient client, VerifyResetCodeCommand command)
        {
            return await client.PostAndDeserializeAsync<string>("api/auth/verifyOTP", command);
        }

        public static async Task<BaseResponse<bool>> ResetPassword(
            this HttpClient client, ResetPasswordCommand command)
        {
            return await client.PostAndDeserializeAsync<bool>("api/auth/resetPassword", command);
        }

        // ── Users Management ───────────────────────────────────────────────────────

        public static async Task<BaseResponse<GetUsersResult>> GetUsers(
            this HttpClient client, string accessToken,
            UserRole? role = null, string? status = null, string? q = null,
            int page = 1, int limit = 20)
        {
            var qs = $"api/users?page={page}&limit={limit}";
            if (role.HasValue) qs += $"&role={(int)role.Value}";
            if (status != null) qs += $"&status={Uri.EscapeDataString(status)}";
            if (q != null) qs += $"&q={Uri.EscapeDataString(q)}";
            return await client.GetAndDeserializeAsync<GetUsersResult>(qs, accessToken);
        }

        public static async Task<BaseResponse<UserDto>> AddUser(
            this HttpClient client, CreateUserCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<UserDto>("api/users/addUser/", command, accessToken);
        }

        public static async Task<BaseResponse<UserDto>> GetUserById(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.GetAndDeserializeAsync<UserDto>($"api/users/getUser?id={id}", accessToken);
        }

        public static async Task<BaseResponse<bool>> UpdateUserRole(
            this HttpClient client, UpdateUserRoleCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<bool>($"api/users/updateRole", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> ActivateUser(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/users/activateUser?userId={userId}", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> DeactivateUser(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/users/deactivateUser?userId={userId}", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteUser(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/users/{userId}", accessToken);
        }

        public static async Task<BaseResponse<bool>> ChangePassword(
            this HttpClient client, ChangeUserPasswordCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>("api/users/changePassword", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> UploadAvatar(
            this HttpClient client, Guid userId, byte[] fileBytes, string fileName, string accessToken)
        {
            var request = CreateRequest($"api/users/{userId}/avatar", HttpMethod.Post, accessToken);
            var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(fileBytes)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("image/jpeg") }
            }, "avatar", fileName);
            request.Content = form;
            return await client.SendAndDeserializeAsync<bool>(request);
        }

        public static async Task<BaseResponse<FileDownloadDto>> GetAvatar(
            this HttpClient client, Guid userId)
        {
            return await client.GetAndDeserializeAsync<FileDownloadDto>($"api/users/{userId}/avatar");
        }

        // ── Researcher Profile ─────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<ResearcherProfileDto>>> GetAllResearcherProfiles(
            this HttpClient client)
        {
            return await client.GetAndDeserializeAsync<List<ResearcherProfileDto>>("api/profiles/researchers");
        }

        public static async Task<BaseResponse<ResearcherProfileDto>> GetResearcherProfile(
            this HttpClient client, Guid userId)
        {
            return await client.GetAndDeserializeAsync<ResearcherProfileDto>($"api/profiles/researchers/{userId}");
        }

        public static async Task<BaseResponse<ResearcherProfileDto>> UpdateResearcherProfile(
            this HttpClient client, Guid userId, UpdateResearcherProfileCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<ResearcherProfileDto>($"api/profiles/researchers/{userId}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteResearcherProfile(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/profiles/researchers/{userId}", accessToken);
        }

        // ── PhD Student Profile ────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<PhDStudentProfileDto>>> GetAllPhDStudentProfiles(
            this HttpClient client)
        {
            return await client.GetAndDeserializeAsync<List<PhDStudentProfileDto>>("api/profiles/phd-students");
        }

        public static async Task<BaseResponse<PhDStudentProfileDto>> GetPhDStudentProfile(
            this HttpClient client, Guid userId)
        {
            return await client.GetAndDeserializeAsync<PhDStudentProfileDto>($"api/profiles/phd-students/{userId}");
        }

        public static async Task<BaseResponse<PhDStudentProfileDto>> UpdatePhDStudentProfile(
            this HttpClient client, Guid userId, UpdatePhDStudentProfileCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<PhDStudentProfileDto>($"api/profiles/phd-students/{userId}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeletePhDStudentProfile(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/profiles/phd-students/{userId}", accessToken);
        }

        // ── Masterian Profile ──────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<MasterianProfileDto>>> GetAllMasterianProfiles(
            this HttpClient client)
        {
            return await client.GetAndDeserializeAsync<List<MasterianProfileDto>>("api/profiles/masterians");
        }

        public static async Task<BaseResponse<MasterianProfileDto>> GetMasterianProfile(
            this HttpClient client, Guid userId)
        {
            return await client.GetAndDeserializeAsync<MasterianProfileDto>($"api/profiles/masterians/{userId}");
        }

        public static async Task<BaseResponse<MasterianProfileDto>> UpdateMasterianProfile(
            this HttpClient client, Guid userId, UpdateMasterianProfileCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<MasterianProfileDto>($"api/profiles/masterians/{userId}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteMasterianProfile(
            this HttpClient client, Guid userId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/profiles/masterians/{userId}", accessToken);
        }

        // ── Research Axes ──────────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<ResearchAxisDto>>> GetAllResearchAxes(this HttpClient client)
        {
            return await client.GetAndDeserializeAsync<List<ResearchAxisDto>>("api/research-axes");
        }

        public static async Task<BaseResponse<ResearchAxisDto>> GetResearchAxisById(
            this HttpClient client, Guid id)
        {
            return await client.GetAndDeserializeAsync<ResearchAxisDto>($"api/research-axes/{id}");
        }

        public static async Task<BaseResponse<ResearchAxisDto>> CreateResearchAxis(
            this HttpClient client, CreateResearchAxisCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<ResearchAxisDto>("api/research-axes", command, accessToken);
        }

        public static async Task<BaseResponse<ResearchAxisDto>> UpdateResearchAxis(
            this HttpClient client, Guid id, UpdateResearchAxisCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<ResearchAxisDto>($"api/research-axes/{id}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteResearchAxis(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/research-axes/{id}", accessToken);
        }

        public static async Task<BaseResponse<bool>> AddAxisMember(
            this HttpClient client, Guid axisId, Guid userId, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/research-axes/{axisId}/members?userId={userId}", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> RemoveAxisMember(
            this HttpClient client, Guid axisId, Guid userId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/research-axes/{axisId}/members/{userId}", accessToken);
        }

        // ── Events ─────────────────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<EventDto>>> GetEvents(
            this HttpClient client, GetEventsQuery query)
        {
            var queryParams = new List<string>();
            if (query.Q != null) queryParams.Add($"q={Uri.EscapeDataString(query.Q)}");
            if (query.Status != null) queryParams.Add($"status={Uri.EscapeDataString(query.Status)}");
            if (query.Type != null) queryParams.Add($"type={Uri.EscapeDataString(query.Type)}");
            if (query.Page > 0) queryParams.Add($"page={query.Page}");
            if (query.Limit > 0) queryParams.Add($"limit={query.Limit}");
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await client.GetAndDeserializeAsync<List<EventDto>>($"api/events/{queryString}");
        }

        public static async Task<BaseResponse<EventDto>> GetEventById(
            this HttpClient client, Guid eventId)
        {
            return await client.GetAndDeserializeAsync<EventDto>($"api/events/{eventId}");
        }

        public static async Task<BaseResponse<EventDto>> CreateEvent(
            this HttpClient client, CreateEventCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<EventDto>("api/events/", command, accessToken);
        }

        public static async Task<BaseResponse<EventDto>> UpdateEvent(
            this HttpClient client, Guid eventId, UpdateEventCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<EventDto>($"api/events/{eventId}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteEvent(
            this HttpClient client, Guid eventId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/events/{eventId}", accessToken);
        }

        public static async Task<BaseResponse<SpeakerDto>> AddSpeaker(
            this HttpClient client, Guid eventId, CreateSpeakerCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<SpeakerDto>($"api/events/{eventId}/speakers", command, accessToken);
        }

        public static async Task<BaseResponse<SpeakerDto>> UpdateSpeaker(
            this HttpClient client, Guid eventId, Guid speakerId, UpdateSpeakerCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<SpeakerDto>($"api/events/{eventId}/speakers/{speakerId}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeleteSpeaker(
            this HttpClient client, Guid eventId, Guid speakerId, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/events/{eventId}/speakers/{speakerId}", accessToken);
        }

        // ── Publications ───────────────────────────────────────────────────────────

        // ── Publications ───────────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<PublicationDto>>> GetPublications(
            this HttpClient client, GetPublicationsQuery query, string accessToken)
        {
            var queryParams = new List<string> { $"page={query.Page}", $"limit={query.Limit}" };
            if (!string.IsNullOrEmpty(query.Status))
                queryParams.Add($"status={Uri.EscapeDataString(query.Status)}");
            if (query.AxeId.HasValue)
                queryParams.Add($"researchAxisId={query.AxeId}");
            var queryString = "?" + string.Join("&", queryParams);
            return await client.GetAndDeserializeAsync<List<PublicationDto>>($"api/v1/publications{queryString}", accessToken);
        }

        public static async Task<BaseResponse<PublicationDto>> GetPublicationById(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.GetAndDeserializeAsync<PublicationDto>($"api/v1/publications/{id}", accessToken);
        }

        public static async Task<BaseResponse<PublicationDto>> CreatePublication(
            this HttpClient client, CreatePublicationCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<PublicationDto>("api/v1/publications", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> UpdatePublication(
            this HttpClient client, Guid id, UpdatePublicationCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<bool>($"api/v1/publications/{id}", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> DeletePublication(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.DeleteAndDeserializeAsync<bool>($"api/v1/publications/{id}", accessToken);
        }

        public static async Task<BaseResponse<bool>> SubmitPublication(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/v1/publications/{id}/submit", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> ValidatePublication(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/v1/publications/{id}/validate", accessToken: accessToken);
        }

        public static async Task<BaseResponse<bool>> RejectPublication(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/v1/publications/{id}/reject", accessToken: accessToken);
        }

        public static async Task<BaseResponse<List<FileDownloadDto>>> GetPublicationPdfs(
            this HttpClient client, Guid id, string accessToken)
        {
            return await client.GetAndDeserializeAsync<List<FileDownloadDto>>($"api/v1/publications/{id}/pdfs", accessToken);
        }

        public static async Task<BaseResponse<bool>> AddPublicationPdf(
            this HttpClient client, Guid id, AddPublicationPdfsCommand command, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"api/v1/publications/{id}/pdf", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> RemovePublicationPdf(
            this HttpClient client, Guid id, RemovePublicationPdfCommand command, string accessToken)
        {
            var request = CreateRequest($"api/v1/publications/{id}/pdf", HttpMethod.Delete, accessToken);
            request.Content = JsonContent.Create(command);
            return await client.SendAndDeserializeAsync<bool>(request);
        }

        // ── Contact ────────────────────────────────────────────────────────────────

        public static async Task<BaseResponse<string>> SendContactMessage(
            this HttpClient client, SendContactMessageCommand command)
        {
            return await client.PostAndDeserializeAsync<string>("api/contact/send", command);
        }

        // ── Audit Logs ─────────────────────────────────────────────────────────────

        public static async Task<BaseResponse<List<AuditLogDto>>> GetAuditLogs(
            this HttpClient client, string accessToken, GetAuditLogsQuery query)
        {
            var endpoint = $"api/auditLogs/?fromUtc={Uri.EscapeDataString(query.FromUtc.ToString("O"))}";
            if (query.ToUtc.HasValue)
                endpoint += $"&toUtc={Uri.EscapeDataString(query.ToUtc.Value.ToString("O"))}";
            return await client.GetAndDeserializeAsync<List<AuditLogDto>>(endpoint, accessToken);
        }

        // ── Settings ───────────────────────────────────────────────────────────────

        public static async Task<BaseResponse<SettingsResponseDto>> GetSettings(
            this HttpClient client, string accessToken)
        {
            return await client.GetAndDeserializeAsync<SettingsResponseDto>("dashboard/superadmin/settings", accessToken);
        }

        public static async Task<BaseResponse<bool>> UpdateSettings(
            this HttpClient client, SettingsCommand command, string accessToken)
        {
            return await client.PutAndDeserializeAsync<bool>("dashboard/superadmin/settings", command, accessToken);
        }

        public static async Task<BaseResponse<bool>> TestSmtp(
            this HttpClient client, string testEmail, string accessToken)
        {
            return await client.PostAndDeserializeAsync<bool>($"dashboard/superadmin/settings/smtp/test?testEmail={Uri.EscapeDataString(testEmail)}", accessToken: accessToken);
        }
    }
}