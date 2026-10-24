using Hangfire;
using WebApi.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Configure Hangfire with SQL Server.
var connStrHangFireName = "SqlServer"; 
var connectionString = builder.Configuration
    .GetConnectionString(connStrHangFireName)
    ?? throw new InvalidOperationException($"ConnectionString for HangFire is not configured: {connStrHangFireName}.");

builder.Services.AddHangfire(configuration =>
{
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString);
});

// Start Hangfire background workers.
builder.Services.AddHangfireServer();

//Service Auth Dashboard Hangfire
builder.Services.AddTransient<HangfireDashboardBasicAuthFilter>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Hangfire Dashboard
//app.UseHangfireDashboard("/hangfire");
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        app.Services.GetRequiredService<
            HangfireDashboardBasicAuthFilter>()
    ]
});

app.MapControllers();

app.Run();