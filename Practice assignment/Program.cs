
using Microsoft.EntityFrameworkCore;
using Practice_assignment.Data;
using Practice_assignment.Filters;  
using Practice_assignment.Patterns.Factory;
using Practice_assignment.Patterns.Observer;
using Practice_assignment.Patterns.Repository;
using Practice_assignment.Services;

var builder = WebApplication.CreateBuilder(args);


// API Client Services 

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7217/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();


// Database 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories: Repository Pattern
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

// Factories : Factory Pattern
builder.Services.AddSingleton<IContractFactory, ContractFactory>();

// Observers: Observer Pattern
builder.Services.AddScoped<IContractObserver, AuditLogObserver>();
builder.Services.AddScoped<IContractObserver, ExpiryNotificationObserver>();

// Business Logic Services 
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IFileService, FileService>();

// HTTP Client for Currency API (Original)
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

// MVC with Authorization Filter (NEW)
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthorizeFilter>();  
});

var app = builder.Build();

// Middleware pipeline 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();  // enables session for JWT token storage

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Auto migrate on startup 
/*using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
db.Database.Migrate();*/

app.Run();

// Required for integration test WebApplicationFactory
public partial class Program { }