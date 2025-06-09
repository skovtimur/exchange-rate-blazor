using ExchangeRate;
using ExchangeRate.Cache;
using ExchangeRate.Components;
using ExchangeRate.Components.Pages;
using ExchangeRate.Extensions;
using ExchangeRate.Options;
using ExchangeRate.Services;
using Quartz;
using Radzen;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddStackExchangeRedisCache(x =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

    if (string.IsNullOrEmpty(redisConnectionString))
        throw new NullReferenceException("Redis connection string not found");

    x.Configuration = redisConnectionString;
});
builder.Services.AddServerSideBlazor();
builder.Services.AddAutoMapper(typeof(MainMapper));
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "VisionSuiteShipManagerServerTheme";
    options.Duration = TimeSpan.FromDays(365);
});

builder.Services.AddQuartz(x => { x.UseMicrosoftDependencyInjectionJobFactory(); });

//Options:
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("API"));

//Dependencies injection:
builder.Services.AddHttpClient<CurrencyService>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddSingleton<IRedisManager, RedisManager>();
builder.Services.AddScoped<IExcelExporterService, ExcelExporterService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("Redis")));

await AddBackgroundJobsJobsExtensions.AddUpdaterJob(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();