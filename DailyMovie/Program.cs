using DailyMovie.Data;
using DailyMovie.ServiceContracts;
using DailyMovie.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DailyMovieDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("WebApiDatabase")));
builder.Services.AddTransient<IMovieUrlFetch, MovieUrlFetchService>();
builder.Services.AddTransient<IMovie, MovieService>();

var app = builder.Build();

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
