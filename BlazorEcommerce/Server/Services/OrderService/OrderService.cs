﻿
using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Server.Services.CartService;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly DataContext _dbContext;
		private readonly ICartService _cartService;
		private readonly HttpContextAccessor _httpContextAccessor;

		public OrderService(DataContext dbContext, ICartService cartService, HttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_cartService = cartService;
			_httpContextAccessor = httpContextAccessor;
		}

		private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

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
				UserId = GetUserId(),
				OrderDate = DateTime.Now,
				TotalPrice = totalPrice,
				OrderItems = orderItems
			};

			_dbContext.Orders.Add(order);
			await _dbContext.SaveChangesAsync();
			return new ServiceResponse<bool> { Data = true };
		}
	}
}
