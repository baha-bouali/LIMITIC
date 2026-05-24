using System.Net.Http.Headers;
using System.Net.Http.Json;
using LIMTIC.Application.DTOs.UserManagement.GetUser;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI;
using LIMTIC.WebAPI.Models.Auth.ForgetPassword;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.ResetPassword;
using LIMTIC.WebAPI.Models.Auth.VerifyResetCode;
using LIMTIC.WebAPI.Models.Events.GetEvents;
using LIMTIC.WebAPI.Models.Events.CreateEvent;
using LIMTIC.WebAPI.Models.Events.UpdateEvent;
using LIMTIC.WebAPI.Models.Events.AddSpeaker;
using LIMTIC.WebAPI.Models.Events.UpdateSpeaker;
using LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.Contact;

namespace LIMTIC.E2Es.Extensions
{
    public static class ClientExtensions
    {
        private static HttpRequestMessage CreateRequest(string endpoint, HttpMethod httpMethod, string accessToken)
        {
            var request = new HttpRequestMessage(httpMethod, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }

        public static async Task<CreateUserResponse?> AddUser(this HttpClient client, CreateUserRequest createUserRequest, string accessToken)
        {
            var request = CreateRequest("api/users/addUser/", HttpMethod.Post, accessToken);
            request.Content = JsonContent.Create(createUserRequest);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CreateUserResponse>();
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AddUser failed: {response.StatusCode} - {errorBody}");
        }
        public static async Task<GetUserResponse?> GetUserById(this HttpClient client, Guid id, string accessToken)
        {
            var request = CreateRequest($"api/users/getUser?id={id}", HttpMethod.Get, accessToken);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GetUserResponse>();
            }
            return null;
        }

        public static async Task<HttpResponseMessage> GetUserByIdFullResponse(this HttpClient client, Guid id, string accessToken)
        {
            var request = CreateRequest($"api/users/getUser?id={id}", HttpMethod.Get, accessToken);
            return await client.SendAsync(request);
        }

        public static async Task<BaseResponse?> ActivateUser(this HttpClient client, Guid userId, string accessToken)
        {
            var request = CreateRequest($"api/users/activateUser?userId={userId}", HttpMethod.Post, accessToken);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BaseResponse>();
            }
            return null;
        }

        public static async Task<BaseResponse?> DeactivateUser(this HttpClient client, Guid userId, string accessToken)
        {
            var request = CreateRequest($"api/users/deactivateUser?userId={userId}", HttpMethod.Post, accessToken);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BaseResponse>();
            }
            return null;
        }

        public static async Task<HttpResponseMessage> GetUserByIdFullHttpResponse(this HttpClient client, Guid id, string accessToken)
        {
            var request = CreateRequest($"api/users/getUser?id={id}", HttpMethod.Get, accessToken);
            return await client.SendAsync(request);
        }

        public static async Task<LoginResponse?> AuthenticateUser(this HttpClient client, LoginRequest loginRequest)
        {
            var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }

        public static async Task<LoginResponse?> RefreshToken(this HttpClient client)
        {
            var response = await client.PostAsync("api/auth/refreshToken", null);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }
        public static async Task<HttpResponseMessage> ChangePassword(this HttpClient client, ChangeUserPasswordRequest request, string accessToken)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return await client.PostAsJsonAsync("api/users/changePassword", request);
        }
        public static async Task<HttpResponseMessage> ForgotPassword(this HttpClient client, ForgetPasswordRequest request)
        {
            return await client.PostAsJsonAsync("api/auth/forgotPassword", request);
        }

        public static async Task<HttpResponseMessage> VerifyOTP(this HttpClient client, VerifyResetCodeRequest request)
        {
            return await client.PostAsJsonAsync("api/auth/verifyOTP", request);
        }

        public static async Task<HttpResponseMessage> ResetPassword(this HttpClient client, ResetPasswordRequest request)
        {
            return await client.PostAsJsonAsync("api/auth/resetPassword", request);
        }

        public static async Task<HttpResponseMessage> SendContactMessage(this HttpClient client, SendContactMessageRequest request)
        {
            return await client.PostAsJsonAsync("api/contact/send", request);
        }

        public static async Task<HttpResponseMessage> RefreshTokenFullHttpResponse(this HttpClient client)
        {
            return await client.PostAsync("api/auth/refreshToken", null);
        }

        public static async Task<GetEventsResponse?> GetEvents(this HttpClient client, string? status = null, string? type = null, int page = 1, int limit = 20, string? q = null)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={Uri.EscapeDataString(status)}");
            if (!string.IsNullOrWhiteSpace(type))
                queryParams.Add($"type={Uri.EscapeDataString(type)}");
            if (page > 0)
                queryParams.Add($"page={page}");
            if (limit > 0)
                queryParams.Add($"limit={limit}");
            if (!string.IsNullOrWhiteSpace(q))
                queryParams.Add($"q={Uri.EscapeDataString(q)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await client.GetAsync($"api/events/{queryString}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<GetEventsResponse>();
            return null;
        }

        public static async Task<HttpResponseMessage> GetEventsFullHttpResponse(this HttpClient client, string? status = null, string? type = null, int page = 1, int limit = 20, string? q = null)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={Uri.EscapeDataString(status)}");
            if (!string.IsNullOrWhiteSpace(type))
                queryParams.Add($"type={Uri.EscapeDataString(type)}");
            if (page > 0)
                queryParams.Add($"page={page}");
            if (limit > 0)
                queryParams.Add($"limit={limit}");
            if (!string.IsNullOrWhiteSpace(q))
                queryParams.Add($"q={Uri.EscapeDataString(q)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await client.GetAsync($"api/events/{queryString}");
        }

        public static async Task<CreateEventResponse?> CreateEvent(this HttpClient client, CreateEventRequest requestBody, string accessToken)
        {
            var request = CreateRequest("api/events/", HttpMethod.Post, accessToken);
            request.Content = JsonContent.Create(requestBody);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CreateEventResponse>();
            return null;
        }

        public static async Task<UpdateEventResponse?> UpdateEvent(this HttpClient client, Guid eventId, UpdateEventRequest requestBody, string accessToken)
        {
            var request = CreateRequest($"api/events/{eventId}", HttpMethod.Put, accessToken);
            request.Content = JsonContent.Create(requestBody);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UpdateEventResponse>();
            return null;
        }

        public static async Task<BaseResponse?> DeleteEvent(this HttpClient client, Guid eventId, string accessToken)
        {
            var request = CreateRequest($"api/events/{eventId}", HttpMethod.Delete, accessToken);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BaseResponse>();
            return null;
        }

        public static async Task<AddSpeakerResponse?> AddSpeaker(this HttpClient client, Guid eventId, AddSpeakerRequest requestBody, string accessToken)
        {
            var request = CreateRequest($"api/events/{eventId}/speakers", HttpMethod.Post, accessToken);
            request.Content = JsonContent.Create(requestBody);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<AddSpeakerResponse>();
            return null;
        }

        public static async Task<UpdateSpeakerResponse?> UpdateSpeaker(this HttpClient client, Guid eventId, Guid speakerId, UpdateSpeakerRequest requestBody, string accessToken)
        {
            var request = CreateRequest($"api/events/{eventId}/speakers/{speakerId}", HttpMethod.Put, accessToken);
            request.Content = JsonContent.Create(requestBody);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UpdateSpeakerResponse>();
            return null;
        }

        public static async Task<BaseResponse?> DeleteSpeaker(this HttpClient client, Guid eventId, Guid speakerId, string accessToken)
        {
            var request = CreateRequest($"api/events/{eventId}/speakers/{speakerId}", HttpMethod.Delete, accessToken);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BaseResponse>();
            return null;
        }
    }
}
