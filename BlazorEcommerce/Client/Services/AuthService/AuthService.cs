using BlazorEcommerce.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

		public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
		{
			_httpClient = httpClient;
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
		{
            var result = await _httpClient.PostAsJsonAsync("api/auth/change-password", request.Password);
            var res = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return res;
		}

		public async Task<bool> IsUserAuthenticated()
		{
			var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User.Identity.IsAuthenticated;
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
