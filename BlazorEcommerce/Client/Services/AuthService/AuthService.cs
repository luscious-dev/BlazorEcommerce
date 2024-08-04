using BlazorEcommerce.Shared;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

		public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
		{
            var result = await _httpClient.PostAsJsonAsync("api/auth/change-password", request.Password);
            var res = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return res;
		}

		public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var response = await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
            return response;
        }

        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var response = await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
            return response;
        }
    }
}
