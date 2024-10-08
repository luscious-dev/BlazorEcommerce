global using BlazorEcommerce.Shared;
using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using BlazorEcommerce.Server.Services.CategoryService;
using BlazorEcommerce.Server.Services.OrderService;
using BlazorEcommerce.Server.Services.ProductService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DataContext>(config =>
{
	config.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), opt => opt.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(option =>
	{
		option.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
				.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

var app = builder.Build();

app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseSwagger();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
