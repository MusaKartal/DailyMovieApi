using DailyMovie.Data;
using DailyMovie.Extensions;
using DailyMovie.FetchBackgroundService;
using DailyMovie.ServiceContracts;
using DailyMovie.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DailyMovieDbContext>(options => 
options.UseSqlite(builder.Configuration.GetConnectionString("WebApiDatabase")));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IMovieUrlFetch, MovieUrlFetchService>();

builder.Services.AddScoped<IMovie, MovieService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

builder.Services.AddHostedService<MediaDataFetchingService>();

builder.Services.AddHostedService<QueueService>();
var app = builder.Build();

app.ConfigureExceptionHandler();

app.UseMiddleware<ErrorHandlerMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
