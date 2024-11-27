using CharacomOnline.Data;
using CharacomOnline.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<Radzen.ThemeService>();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // ログレベルを指定
builder.Logging.AddConsole(); // コンソールに出力するための設定
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<FileHandleService>();
builder.Services.AddSingleton<GoogleDriveService>();
builder.Services.AddHttpContextAccessor();

// Add authentication services
builder
  .Services.AddAuthentication(options =>
  {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
  })
  .AddCookie()
  .AddGoogle(options =>
  {
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Google");
    Console.WriteLine($"clientId = {googleAuthNSection["ClientId"]}");
    Console.WriteLine($"clientId = {googleAuthNSection["ClientSecret"]}");
    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
    options.CallbackPath = "/signin-google";
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(
    "Google",
    policy =>
    {
      policy.AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme);
      policy.RequireAuthenticatedUser();
    }
  );
  Console.WriteLine("AddAuthorization");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");

  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line
app.UseAuthorization(); // Add this line

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
