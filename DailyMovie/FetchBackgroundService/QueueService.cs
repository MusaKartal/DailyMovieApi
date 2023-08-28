using DailyMovie.Data;
using DailyMovie.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DailyMovie.FetchBackgroundService
{
    public class QueueService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IMemoryCache _cache;
        public QueueService(IServiceProvider services, IMemoryCache cache)
        {
            _services = services;
            _cache = cache;
         
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                

                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DailyMovieDbContext>();
                    List<Movie> films = dbContext.Movies.Where(x => !x.IsViewed).Take(20).ToList();

                    foreach (var film in films)
                    {
                        _cache.TryGetValue("filmQueue", out List<Movie> filmQueue);
                        if (filmQueue == null)
                        {
                            filmQueue = new List<Movie>();
                        }

                        filmQueue.Add(film);
                        _cache.Set("filmQueue", filmQueue, TimeSpan.FromMinutes(10));
                    }

                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
            }
        }
    }
}

