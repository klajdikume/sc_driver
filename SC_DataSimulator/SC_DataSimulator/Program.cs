using Hangfire;
using Hangfire.MemoryStorage;
using SC_DataSimulator;
using SC_DataSimulator.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});

builder.Services.AddTransient<DataGenerationJob>();
builder.Services.AddTransient<AppDbContext>();
builder.Services.AddTransient<IDriverActivity, DriverActivity>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
}); 

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHangfireServer();
app.UseHangfireDashboard();

var initialData = SeedData.GetInitialData();

app.MapGet("/seed", (IDriverActivity drivers) =>
{
    var existdrivers = drivers.ExistDriver();

    if (existdrivers)
    {
        drivers.AddDriver("Driver A");
    }

    return "Drivers seeded";
});

app.MapGet("/startdatageneration", (IServiceProvider serviceProvider) =>
{
    var job = serviceProvider.GetRequiredService<DataGenerationJob>();
    RecurringJob.AddOrUpdate("dataGenerationJob", () => job.Execute(), Cron.Minutely);
    return "Data generation job started.";
});

app.MapGet("/stopdatageneration", () =>
{
    RecurringJob.RemoveIfExists("dataGenerationJob");
    return "Data generation job stopped.";
});

app.MapGet("/activitycount", (IServiceProvider serviceProvider) =>
{
    var driver = serviceProvider.GetRequiredService<IDriverActivity>();
    var activitycount = driver.ActivityCount();

    return activitycount;
});

app.MapGet("/getalldrivers", (IServiceProvider serviceProvider) =>
{
    var driver = serviceProvider.GetRequiredService<IDriverActivity>();
    var totalDrivers = driver.GetAllDrivers();

    return totalDrivers;
});

app.MapGet("/totalhourswithtype", (IServiceProvider serviceProvider) =>
{
    var driver = serviceProvider.GetRequiredService<IDriverActivity>();
    var totalHoursWithType = driver.TotalDriveHoursTypes();

    return totalHoursWithType;
});

app.UseCors("CorsPolicy");

app.MapHub<DriverHub>("/drivers").RequireCors("CorsPolicy");

app.Run();
