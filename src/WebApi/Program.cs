using Hangfire;
using Microsoft.OpenApi;
using WebApi.Authorization;
using WebApi.Services;
using WebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HangFire",
        Version = "v1",
        Description = "API for managing Hangfire background jobs"
    });
});

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
//builder.Services.AddHangfireServer();
builder.Services.AddHangfireServer(options =>
{
    options.Queues =
    [
        "main-queue",
        "scheduled-queue",
        "recurring-queue",
        "default"
    ];
});

//Service Auth Dashboard Hangfire
builder.Services.AddTransient<HangfireDashboardBasicAuthFilter>();


//Other Services
builder.Services.AddScoped<IBackgroundJobsService, BackgroundJobsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "WebApi v1");

        options.RoutePrefix = "swagger";
    });
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