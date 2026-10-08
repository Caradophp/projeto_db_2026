using Serilog;
using projeto.Controllers;
using projeto.Repository;
using projeto.Security;
using projeto.Service;

DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();

// Configuração Global do Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando a aplicação web...");
    var builder = WebApplication.CreateBuilder(args);

    // Integração do Serilog com o pipeline de log do .NET
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddScoped<projeto.Controllers.ColheitaController>();
    builder.Services.AddScoped<CodeRepository>();
    builder.Services.AddScoped<UtilRepository>();
    builder.Services.AddScoped<EmailService>();
    builder.Services.AddScoped<UserRepository>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<Jwt>();
    builder.Services.AddScoped<EncryptionService>();

    builder.Services.AddExceptionHandler<ExceptionController>();
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<SecurityFilter>();
    });

    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
