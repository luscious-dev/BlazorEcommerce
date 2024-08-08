using BlazorEcommerce.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CartService(ILocalStorageService localStorageService, HttpClient httpClient, AuthenticationStateProvider authStateProvider)
		{
			_localStorageService = localStorageService;
			_httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

		public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                Console.WriteLine("User is authenticated");
            }
            else
            {
                Console.WriteLine("User is not authenticated");
            }
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var existingCartItem = cart.FirstOrDefault(x => x.ProductTypeId == cartItem.ProductTypeId && x.ProductId == cartItem.ProductId);

            if (existingCartItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                existingCartItem.Quantity += cartItem.Quantity;
            }

            await _localStorageService.SetItemAsync("cart", cart);
            await GetCartItemsCount();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            await GetCartItemsCount();
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }

        public async Task GetCartItemsCount()
        {
            if(await IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorageService.SetItemAsync("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                await _localStorageService.SetItemAsync("cartItemsCount", cart != null ? cart.Count " 0");
            }

            OnChange.Invoke();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
		{
            var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
			var res = await _httpClient.PostAsJsonAsync("/api/cart/products", cartItems);
            var cartProducts = await res.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

			return cartProducts.Data;
		}

		public async Task RemoveCartItem(int productId, int productTypeId)
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            var cartItem = cart.First(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

            if(cartItem is not null)
            {
				cart.Remove(cartItem);
                await _localStorageService.SetItemAsync("cart", cart);
                await GetCartItemsCount();
			}
		}

        public async Task StoreCartItems(bool emptyLocalCart = false)
        {
            var localCart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if(localCart is null)
            {
                return;
            }

            await _httpClient.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorageService.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
		{
			var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
			var cartItem = cart.First(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

			if (cartItem is not null)
			{
				cartItem.Quantity = product.Quantity;
				await _localStorageService.SetItemAsync("cart", cart);
                OnChange.Invoke();
			}
		}

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}
