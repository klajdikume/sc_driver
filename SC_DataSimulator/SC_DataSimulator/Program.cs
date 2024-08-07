using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SC_DataSimulator;
using SC_DataSimulator.DAL;
using SC_DataSimulator.DomainModels;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

const string AuthScheme = "cookie";

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
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddSignalR();

builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme);

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

app.UseAuthentication();

app.UseAuthorization();

app.MapPost("/login", async (HttpContext ctx, IUserRepository userRepository, [FromBody] UserLogIn loginDto) =>
{
    if (string.IsNullOrEmpty(loginDto.Name) || string.IsNullOrEmpty(loginDto.Password))
    {
        return "Name and Password cannot be empty.";
    }

    var user = await userRepository.CheckUserData(loginDto);

    if (user is null)
    {
        return "User does not exist";
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Name, user.Name)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
       principal,
       new AuthenticationProperties { IsPersistent = user.RememberLogin });

    return "Authenticated";

});


app.UseCors("CorsPolicy");

app.MapHub<DriverHub>("/drivers").RequireCors("CorsPolicy");

app.Run();
