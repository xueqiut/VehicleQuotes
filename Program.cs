using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add VehicleQuotesContext into ASP.NET Core's built-in IoC (inversion of control) container
// so that it can be injected into other classes via Dependency Injection (DI)
// The services variable contains all the objects (known as “services”) that are available in the app for Dependency Injection
// https://martinfowler.com/articles/injection.html
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-7.0
builder.Services.AddDbContext<VehicleQuotesContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VehicleQuotesContext"))
    .UseSnakeCaseNamingConvention()
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
    .EnableSensitiveDataLogging());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VehicleQuotes v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
