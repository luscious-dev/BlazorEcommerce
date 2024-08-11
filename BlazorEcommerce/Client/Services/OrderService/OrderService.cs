
using BlazorEcommerce.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

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

		public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
		{
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>($"api/order/{orderId}");
			return result.Data;
		}

		public async Task<List<OrderOverviewResponse>> GetOrders()
		{
			var res = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order");
			return res.Data;
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
