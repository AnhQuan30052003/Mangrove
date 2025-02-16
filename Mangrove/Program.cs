using Mangrove.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MangroveContext>(options => {
	options.UseSqlServer(builder.Configuration.GetConnectionString("mangrove"));
});
builder.Services.AddSession((options) => {
	options.IdleTimeout = TimeSpan.FromHours(1);
});


// var apiKey = builder.Configuration.GetConnectionString("IPA_Translate") ?? "";
// builder.Services.AddSingleton(new GoogleTranslateService(apiKey));


var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
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
	//pattern: "{controller=Home}/{action=Index}/{id?}");
	pattern: "{controller=Home}/{action=Result}/{id?}");

app.Run();