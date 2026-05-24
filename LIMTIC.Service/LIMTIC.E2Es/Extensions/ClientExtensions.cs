using System.Net.Http.Headers;
using System.Net.Http.Json;
using LIMTIC.Application.DTOs.UserManagement.GetUser;
using LIMTIC.WebAPI;
using LIMTIC.WebAPI.Models.Auth.ForgetPassword;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.ResetPassword;
using LIMTIC.WebAPI.Models.Auth.VerifyResetCode;
using LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;

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

        public static async Task<HttpResponseMessage> RefreshTokenFullHttpResponse(this HttpClient client)
        {
            return await client.PostAsync("api/auth/refreshToken", null);
        }

    }
}
