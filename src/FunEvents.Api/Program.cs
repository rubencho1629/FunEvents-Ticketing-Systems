using FunEvents.Application.Bookings.ReserveTickets;
using FunEvents.Infrastructure;
using FunEvents.Infrastructure.Persistence;
using FunEvents.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using FunEvents.Api.Endpoints;
using FunEvents.Api.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ReserveTicketsHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<FunEventsDbContext>();

    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    service = "FunEvents API",
    status = "Running"
}));
app.MapBookingsEndpoints();

app.Run();
public partial class Program;