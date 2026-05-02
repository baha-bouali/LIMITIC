using LIMTIC.Domain.Entities;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.GetUser;
using LIMTIC.WebAPI.Models.Auth.Login;
using System.Net.Http.Json;

namespace LIMTIC.E2Es.Extensions
{
    public static class ClientExtensions
    {
        public static async Task<CreateUserResponse?> AddUser(this HttpClient client, CreateUserRequest createUserRequest)
        {
            var response = await client.PostAsJsonAsync("users/addUser/", createUserRequest);
            return await response.Content.ReadFromJsonAsync<CreateUserResponse>();
        }

        public static async Task<GetUserResponse?> GetUserById(this HttpClient client, Guid id)
        {
            var response = await client.GetAsync($"users/getUser?id={id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GetUserResponse>();
            }
            return null;
        }

        public static async Task<LoginResponse?> AuthenticateUser(this HttpClient client, LoginRequest loginRequest)
        {
            var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);
            
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }
    }
}
