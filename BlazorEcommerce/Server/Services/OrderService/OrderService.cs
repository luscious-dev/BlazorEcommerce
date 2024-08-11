using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly DataContext _dbContext;
		private readonly ICartService _cartService;
		private readonly IAuthService _authService;

		public OrderService(DataContext dbContext, ICartService cartService, IAuthService authService)
		{
			_dbContext = dbContext;
			_cartService = cartService;
			_authService = authService;
		}

		public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders()
		{
			var response = new ServiceResponse<List<OrderOverviewResponse>>();
			var orders = await _dbContext.Orders
				.Include(x => x.OrderItems)
				.ThenInclude(x => x.Product)
				.Where(x => x.UserId == _authService.GetUserId())
				.OrderByDescending(x => x.OrderDate)
				.Select(x => new OrderOverviewResponse
				{
					Id = x.Id,
					OrderDate = x.OrderDate,
					TotalPrice = x.TotalPrice,
					Product = x.OrderItems.Count > 1 ? 
						$"{x.OrderItems.First().Product.Title} and "+$"{x.OrderItems.Count - 1} more..."  
						: x.OrderItems.First().Product.Title,
					ProductImageUrl = x.OrderItems.First().Product.ImageUrl
				})
				.ToListAsync();

			response.Data = orders;
			return response;
		}

		public async Task<ServiceResponse<OrderDetailsResponse>> GetOrdersDetails(int orderId)
		{
			var response = new ServiceResponse<OrderDetailsResponse>();
			var order = await _dbContext.Orders
				.Include(x => x.OrderItems)
				.ThenInclude(x => x.Product)
				.Include(x => x.OrderItems)
				.ThenInclude(x => x.ProductType)
				.Where(x => x.UserId == _authService.GetUserId() && x.Id == orderId)
				.OrderByDescending(x => x.OrderDate)

				.FirstOrDefaultAsync();

			if(order == null)
			{
				response.Success = false;
				response.Message = "Order not found";
				return response;
			}

			var orderDetailsResponse = new OrderDetailsResponse
			{
				OrderDate = order.OrderDate,
				TotalPrice = order.TotalPrice,
				Products = new List<OrderDetailsProductResponse>()
			};

			order.OrderItems.ForEach(item =>
			{
				orderDetailsResponse.Products.Add(new OrderDetailsProductResponse
				{
					ProductId = item.ProductId,
					ImageUrl = item.Product.ImageUrl,
					ProductType = item.ProductType.Name,
					Quantity = item.Quantity,
					Title = item.Product.Title,
					TotalPrice = item.TotalPrice,
				});


			});

			response.Data = orderDetailsResponse;
			return response;
		}

		public async Task<ServiceResponse<bool>> PlaceOrder()
		{
			var products = (await _cartService.GetDbCartProducts()).Data;
			decimal totalPrice = 0;
			products.ForEach(product =>
			{
				totalPrice += product.Price;
			});

			var orderItems = new List<OrderItem>();
			products.ForEach(product =>
			{
				orderItems.Add(new OrderItem
				{
					ProductId = product.ProductId,
					ProductTypeId = product.ProductTypeId,
					Quantity = product.Quantity,
					TotalPrice = product.Price * product.Quantity
				});
			});

			var order = new Order
			{
				UserId = _authService.GetUserId(),
				OrderDate = DateTime.Now,
				TotalPrice = totalPrice,
				OrderItems = orderItems
			};

			_dbContext.Orders.Add(order);

			_dbContext.CartItems.RemoveRange(_dbContext.CartItems.Where(x => x.UserId == _authService.GetUserId()));
			await _dbContext.SaveChangesAsync();
			return new ServiceResponse<bool> { Data = true };
		}
	}
}
