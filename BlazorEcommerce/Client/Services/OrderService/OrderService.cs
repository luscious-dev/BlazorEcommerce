
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorEcommerce.Client.Services.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly HttpClient _httpClient;
		private readonly AuthenticationStateProvider _authStateProvider;
		private readonly NavigationManager _navigationManger;

		public OrderService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, NavigationManager navigationManger)
		{
			_httpClient = httpClient;
			_authStateProvider = authStateProvider;
			_navigationManger = navigationManger;
		}
		public async Task PlaceOrder()
		{
			if(await IsUserAuthenticated())
			{
				await _httpClient.PostAsync("api/order", null);
			}
			else
			{
				_navigationManger.NavigateTo("login");
			}
		}

		private async Task<bool> IsUserAuthenticated()
		{
			return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
		}
	}
}
