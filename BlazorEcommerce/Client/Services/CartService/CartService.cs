using BlazorEcommerce.Client.Services.AuthService;
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
        private readonly IAuthService _authService;

		public CartService(ILocalStorageService localStorageService, HttpClient httpClient, IAuthService authService)
		{
			_localStorageService = localStorageService;
			_httpClient = httpClient;
			_authService = authService;
		}

		public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            if (await _authService.IsUserAuthenticated())
            {
                var request = await _httpClient.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
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
			}
            
            await GetCartItemsCount();
        }

        public async Task GetCartItemsCount()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorageService.SetItemAsync("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                await _localStorageService.SetItemAsync("cartItemsCount", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
		{
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if(cartItems == null)
                    return new List<CartProductResponse>();
                var res = await _httpClient.PostAsJsonAsync("/api/cart/products", cartItems);
                var cartProducts = await res.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartProducts.Data;
            }
            
		}

		public async Task RemoveCartItem(int productId, int productTypeId)
        {
            if(await _authService.IsUserAuthenticated())
            {
                await _httpClient.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
				var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
				var cartItem = cart.First(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

				if (cartItem is not null)
				{
					cart.Remove(cartItem);
					await _localStorageService.SetItemAsync("cart", cart);
				}
			}

			await GetCartItemsCount();
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
            if(await _authService.IsUserAuthenticated())
            {
                var cartItemToUpdate = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };

                var request = await _httpClient.PutAsJsonAsync("api/cart/update-quantity", cartItemToUpdate);
            }
            else
            {
				var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
				var cartItem = cart.First(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

				if (cartItem is not null)
				{
					cartItem.Quantity = product.Quantity;
					await _localStorageService.SetItemAsync("cart", cart);
				}
			}
			
		}
    }
}
