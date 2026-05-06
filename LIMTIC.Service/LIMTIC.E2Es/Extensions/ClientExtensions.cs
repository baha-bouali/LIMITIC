using LIMTIC.Domain.Entities;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.Application.DTOs.UserManagement.CreateUser;
using LIMTIC.Application.DTOs.UserManagement.GetUser;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
            return await response.Content.ReadFromJsonAsync<CreateUserResponse>();
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

        public static async Task<LoginResponse?> AuthenticateUser(this HttpClient client, LoginRequest loginRequest)
        {
            var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }

        public static async Task<LoginResponse?> RefreshToken(this HttpClient client)
        {
            var response = await client.PostAsync("api/auth/refresh", null);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }

        public static async Task<HttpResponseMessage> RefreshTokenFullResponse(this HttpClient client)
        {
            return await client.PostAsync("api/auth/refresh", null);
        }
    }
}
