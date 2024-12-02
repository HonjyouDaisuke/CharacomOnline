using CharacomOnline.Data;
using CharacomOnline.Service;

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
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(
  $"appsettings.{builder.Environment.EnvironmentName}.json",
  optional: true
);

// SupabaseServiceをDIコンテナに登録
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddSingleton<Supabase.Client>(new Supabase.Client("https://imrymolanolzitkwcnhz.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imltcnltb2xhbm9seml0a3djbmh6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mjk3NDc4ODIsImV4cCI6MjA0NTMyMzg4Mn0.JQkLH8CdtHpjIQsU3DKMEscFKdiHmoA0NaQurk4ReM8"));

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
