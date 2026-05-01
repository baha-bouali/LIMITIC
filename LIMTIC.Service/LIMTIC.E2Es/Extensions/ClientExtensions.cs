using LIMTIC.Domain.Entities;
using System.Net.Http.Json;

namespace LIMTIC.E2Es.Extensions
{
    public static class ClientExtensions
    {
        public static async Task<bool> AddUser(this HttpClient client, User user)
        {
            var response = await client.PostAsJsonAsync("users/addUser/", user);
            return response.IsSuccessStatusCode;
        }

        public static async Task<User?> GetUserById(this HttpClient client, Guid id)
        {
            var response = await client.GetAsync($"users/getUser?id={id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }
    }
}
