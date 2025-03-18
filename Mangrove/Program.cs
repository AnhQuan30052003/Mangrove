using Mangrove.Controllers;
using Mangrove.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<SettingWebsiteController>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MangroveContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("mangrove")));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();
builder.Services.AddSession((options) => {
	options.IdleTimeout = TimeSpan.FromDays(3650);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.MaxAge = TimeSpan.FromDays(3650);
});

var app = builder.Build();
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
Helper.Configure(httpContextAccessor);

var supportedCultures = new[] { "vi", "en" };
var localizationOptions = new RequestLocalizationOptions()
	.SetDefaultCulture("en")
	.AddSupportedCultures(supportedCultures)
	.AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	//pattern: "{controller=Home}/{action=Page_Index}/{id?}");
pattern: "{controller=Home}/{action=Page_Distribution}/{id?}");
	//pattern: "{controller=Admin}/{action=Page_Statistical}/{id?}");
	//pattern: "{controller=Mangrove}/{action=Page_Index}/{id?}");
app.Run();