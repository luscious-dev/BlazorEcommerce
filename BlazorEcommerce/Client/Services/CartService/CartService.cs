using BlazorEcommerce.Shared;
using Blazored.LocalStorage;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
		public CartService(ILocalStorageService localStorageService, HttpClient httpClient)
		{
			_localStorageService = localStorageService;
			_httpClient = httpClient;
		}

		public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

            if(cart == null )
            {
                cart = new List<CartItem>();
            }

            var existingCartItem = cart.FirstOrDefault(x => x.ProductTypeId == cartItem.ProductTypeId && x.ProductId == cartItem.ProductId);

            if(existingCartItem == null)
            {
				cart.Add(cartItem);
            }
            else
            {
                existingCartItem.Quantity += cartItem.Quantity;
            }
            
            await _localStorageService.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
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
                OnChange.Invoke();
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
	}
}
