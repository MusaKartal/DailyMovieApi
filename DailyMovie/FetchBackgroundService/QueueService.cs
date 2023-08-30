using DailyMovie.Data;
using DailyMovie.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DailyMovie.FetchBackgroundService
{
    public class QueueService : BackgroundService
    {
        private readonly ILogger<MediaDataFetchingService> _logger;
        private readonly IServiceProvider _services;
        private readonly IMemoryCache _cache;
        public QueueService(IServiceProvider services, IMemoryCache cache, ILogger<MediaDataFetchingService> logger)
        {
            _services = services;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cache process started");
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DailyMovieDbContext>();
                    var moviesToRecommend = dbContext.Movies
                        .Where(movie => !movie.IsViewed)
                        .Take(50)
                        .ToList();

                    if (!_cache.TryGetValue("movieQueue", out List<Movie> movieQueue))
                    {
                        movieQueue = new List<Movie>();
                    }

                    // Yeni önerilen filmleri kuyruğa ekle (eğer zaten ekli değilse) ve görüntülenenleri kaldır
                    foreach (var newMovie in moviesToRecommend)
                    {
                        if (!newMovie.IsViewed && !movieQueue.Any(movie => movie.Id == newMovie.Id))
                        {
                            movieQueue.Add(newMovie);
                        }
                    }

                    // Kuyruktaki filmlerden geçmişte görüntülenenleri kaldır
                    movieQueue.RemoveAll(movie => dbContext.Movies.Any(dbMovie => dbMovie.Id == movie.Id && dbMovie.IsViewed));

                    _cache.Set("movieQueue", movieQueue);
                    _logger.LogInformation("Movie added to cache");
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            
        }
    }
}

