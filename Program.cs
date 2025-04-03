using Blazored.LocalStorage;
using CharacomOnline;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using CharacomOnline.ViewModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Radzen;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
string logTemplate = "| {Timestamp:HH:mm:ss} | {Level:u4} | {Message:j}{NewLine}";

string logFilePathHead = $"logs\\{nameof(CharacomOnline)}";

// Serilog の設定
Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Information()
  .WriteTo.Console()
  .WriteTo.File(
    $"{logFilePathHead}.txt",
    LogEventLevel.Information,
    outputTemplate: logTemplate,
    rollingInterval: RollingInterval.Day
  )
  .WriteTo.File(
    new CompactJsonFormatter(),
    $"{logFilePathHead}_comapct.json",
    LogEventLevel.Information,
    rollingInterval: RollingInterval.Day
  )
  .Enrich.FromLogContext()
  .CreateLogger();

// Serilog を ASP.NET Core のロガーとして使用
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<Radzen.ThemeService>();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // ログレベルを指定
builder.Logging.AddConsole(); // コンソールに出力するための設定
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<CharaDataTableService>();
builder.Services.AddRadzenComponents();
builder.Services.AddBlazoredLocalStorage();

// JSONファイルから設定を読み込む
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(
  $"appsettings.{builder.Environment.EnvironmentName}.json",
  optional: true
);

// Azure App Configurationの接続
var appConfig = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG") ?? builder.Configuration["APPCONFIG"];
if (!string.IsNullOrEmpty(appConfig))
{
    Console.WriteLine("Using Azure App Configuration connection.");
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options
            .Connect(appConfig)
            .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
            .Select(KeyFilter.Any);
    });
}
else
{
    Console.WriteLine("Azure App Configuration connection string is missing.");
}

// Supabaseの接続情報を取得（環境変数が優先）
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? builder.Configuration["SUPABASE_URL"];
var supabaseAnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? builder.Configuration["ANON_KEY"];
var boxClientId = Environment.GetEnvironmentVariable("BOX_CLIENT_ID") ?? builder.Configuration["BOX_CLIENT_ID"];
var boxClientSecret = Environment.GetEnvironmentVariable("BOX_CLIENT_SECRET") ?? builder.Configuration["BOX_CLIENT_SECRET"];

// デバッグ用ログ
Console.WriteLine($"設定読み込みをデバッグ");
Console.WriteLine($"Supabase URL: {supabaseUrl}");
Console.WriteLine($"Supabase AnonKey: {supabaseAnonKey}");
Console.WriteLine($"Box Client ID: {boxClientId}");
Console.WriteLine($"Box Client Secret: {boxClientSecret}");

// // Azure App Configurationを追加
// var appConfig =
// 	Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG") ?? builder.Configuration["APPCONFIG"]; // 環境変数が優先
// if (string.IsNullOrEmpty(appConfig))
// {
// 	Console.WriteLine("Azure App Configuration connection string is missing.");
// }
// else
// {
// 	Console.WriteLine("Using Azure App Configuration connection.");
// 	builder.Configuration.AddAzureAppConfiguration(options =>
// 	{
// 		options
// 			.Connect(appConfig)
// 			.Select(KeyFilter.Any, builder.Environment.EnvironmentName)
// 			.Select(KeyFilter.Any);
// 	});
// }

// // Supabaseの接続情報をAzure App Configurationから取得
// var supabaseUrl = builder.Configuration["SUPABASE_URL"];
// var supabaseAnonKey = builder.Configuration["ANON_KEY"];
// var boxClientId = builder.Configuration["BOX_CLIENT_ID"];
// var boxClientSecret = builder.Configuration["BOX_CLIENT_SECRET"];

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseAnonKey))
{
  throw new InvalidOperationException(
    "Supabase URL or AnonKey is missing from Azure App Configuration."
  );
}

builder.Services.AddSingleton<Supabase.Client>(_ =>
{
  var options = new SupabaseOptions { AutoRefreshToken = true };

  var client = new Supabase.Client(supabaseUrl, supabaseAnonKey, options);
  return client;
});

builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<ImagesTableService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CharaDataTableService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<FileHandleService>();
builder.Services.AddScoped<ProjectsTableService>();
builder.Services.AddScoped<UserProjectsTableService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<CharaDataRepository>();
builder.Services.AddScoped<CharaDataViewModel>();
builder.Services.AddScoped<ImageProcessRepository>();
builder.Services.AddScoped<ImageProcessViewModel>();
builder.Services.AddScoped<ProjectsViewModel>();
builder.Services.AddScoped<ProjectsRepository>();
builder.Services.AddScoped<GlobalSettingTableService>();
builder.Services.AddScoped<GlobalSettingRepository>();
builder.Services.AddScoped<GlobalSettingViewModel>();
builder.Services.AddScoped<StandardTableService>();
builder.Services.AddScoped<StrokeTableService>();
builder.Services.AddScoped<SelectingItemsRepository>();
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<BoxFileService>();
builder.Services.AddScoped<UsersTableService>();
builder.Services.AddScoped<UsersViewModel>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserSettingsTableService>();
builder.Services.AddScoped<UserSettingsViewModel>();
builder.Services.AddScoped<OverlayViewModel>();

// builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<LocalStorageService>();

//Box用
builder.Services.AddHttpClient<OAuthService>(client =>
{
  client.BaseAddress = new Uri("https://api.box.com/oauth2/");
});

if (string.IsNullOrEmpty(boxClientId) || string.IsNullOrEmpty(boxClientSecret))
{
  throw new InvalidOperationException(
    "boxClientId or boxClientSecret is missing from Azure App Configuration."
  );
}

builder.Services.AddSingleton(sp => new OAuthService(
  sp.GetRequiredService<HttpClient>(),
  boxClientId,
  boxClientSecret
));

var appSettings =
  builder.Configuration.Get<AppSettings>()
  ?? throw new InvalidOperationException(
    "AppSettings could not be loaded. Please check your configuration."
  );

builder.Services.AddSingleton(appSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");

  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

// box用

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

try
{
  app.Run();
}
finally
{
  Log.CloseAndFlush();
}
