using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RoomBoard.Web.Data;
using RoomBoard.Web.Services;

var culture = new CultureInfo("el-GR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
var databasePath = Path.Combine(dataDirectory, "roomboard.db");

builder.Services.AddRazorPages();
builder.Services.AddDbContext<RoomBoardDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));
builder.Services.AddScoped<IRoomBoardService, RoomBoardDbService>();

var app = builder.Build();

var disableHttpsRedirection =
    builder.Configuration.GetValue<bool>("RoomBoard:DisableHttpsRedirection") ||
    string.Equals(Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECTION"), "true", StringComparison.OrdinalIgnoreCase);


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomBoardDbContext>();
    db.Database.EnsureCreated();
    RoomBoardDbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("OK"));
app.MapRazorPages();


app.Run();
