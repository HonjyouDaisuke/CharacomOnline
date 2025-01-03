using Blazored.LocalStorage;
using CharacomOnline;
using CharacomOnline.Data;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using CharacomOnline.ViewModel;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
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

// Azure App Configurationを追加
var appConfig = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG")
                ?? builder.Configuration["APPCONFIG"]; // 環境変数が優先
if (string.IsNullOrEmpty(appConfig))
{
  Console.WriteLine("Azure App Configuration connection string is missing.");
}
else
{
  Console.WriteLine("Using Azure App Configuration connection.");
  builder.Configuration.AddAzureAppConfiguration(options =>
  {
    options.Connect(appConfig)
           .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
           .Select(KeyFilter.Any);
  });
}

// Supabaseの接続情報をAzure App Configurationから取得
var supabaseUrl = builder.Configuration["SUPABASE_URL"];
var supabaseAnonKey = builder.Configuration["ANON_KEY"];

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseAnonKey))
{
  throw new InvalidOperationException("Supabase URL or AnonKey is missing from Azure App Configuration.");
}

builder.Services.AddSingleton<Supabase.Client>(_ =>
{
  var client = new Supabase.Client(supabaseUrl, supabaseAnonKey);
  return client;
});

builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<ImagesTableService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CharaDataTableService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<FileHandleService>();
builder.Services.AddScoped<ProjectsTableService>();
builder.Services.AddScoped<UserProjectsTableService>();
builder.Services.AddScoped<CharaDataRepository>();
builder.Services.AddScoped<CharaDataViewModel>();
builder.Services.AddScoped<ImageProcessRepository>();
builder.Services.AddScoped<ImageProcessViewModel>();
builder.Services.AddScoped<ProjectsViewModel>();
builder.Services.AddScoped<GlobalSettingTableService>();
builder.Services.AddScoped<GlobalSettingRepository>();
builder.Services.AddScoped<GlobalSettingViewModel>();
builder.Services.AddScoped<StandardTableService>();
builder.Services.AddScoped<StrokeTableService>();
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<BoxFileService>();

// builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<SessionStorageService>();

//Box用
builder.Services.AddHttpClient<OAuthService>(client =>
{
  client.BaseAddress = new Uri("https://api.box.com/oauth2/");
});
builder.Services.AddSingleton(sp =>
    new OAuthService(
        sp.GetRequiredService<HttpClient>(),
        "xt52jorsw8fzbit06h1rbciwl96cywe1",
        "BQiaeKEhaNY0yn33R4oiEAyyWtswcYCT"
    )
);

var appSettings = builder.Configuration.Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings could not be loaded. Please check your configuration.");

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

/**
app.Use(async (context, next) =>
{
	// Supabaseクライアントを取得
	var supabaseClient = context.RequestServices.GetRequiredService<Supabase.Client>();

	// Supabaseのセッションを確認
	var session = supabaseClient.Auth.CurrentSession;
	if ((session == null || string.IsNullOrEmpty(session.AccessToken)) &&
			!context.Request.Path.StartsWithSegments("/login") && // ログインページは除外
			!context.Request.Path.StartsWithSegments("/callback")) // callbackページは除外
	{
		// 未認証の場合、ログインページへリダイレクト
		context.Response.Redirect("/login");
		return;
	}

	// 次のミドルウェアを呼び出す
	await next();
});
**/


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
