using Mangrove.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MangroveContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("mangrove"));
});
builder.Services.AddSession((options) =>
{
	options.IdleTimeout = TimeSpan.FromHours(1);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Page_Index}/{id?}");
// pattern: "{controller=Auth}/{action=Page_Login}/{id?}");

app.Run();