using Mangrove.Controllers;
using Mangrove.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<SettingWebsiteController>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MangroveContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("mangrove")));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();
builder.Services.AddSession((options) => {
	options.IdleTimeout = TimeSpan.FromDays(Helper.Variable.timeSession);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.MaxAge = TimeSpan.FromDays(Helper.Variable.timeSession);
});
builder.Services.AddAuthentication(Helper.Variable.cookieName)
.AddCookie(Helper.Variable.cookieName, options => {
	options.Cookie.Name = Helper.Variable.cookieName;
	options.LoginPath = "/SettingWebsite/Page_Error";
	options.ExpireTimeSpan = TimeSpan.FromDays(Helper.Variable.timeLogin);
	options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews()
.AddMvcOptions(options => {
	options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
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
	pattern: "{controller=Home}/{action=Handle_Index}/{id?}");
app.Run();