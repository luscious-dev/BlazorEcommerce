using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(CartItem cartItem);
        Task<List<CartItem>> GetCartItems();
        Task<List<CartProductResponse>> GetCartProducts();
        Task RemoveCartItem(int productId, int productTypeId);
        Task UpdateQuantity(CartProductResponse cartProductResponse);
        Task StoreCartItems(bool emptyLocalCart);
        Task GetCartItemsCount();
    }
}
