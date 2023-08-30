
using DailyMovie.ServiceContracts;

namespace DailyMovie.FetchBackgroundService
{

    public class MediaDataFetchingService : BackgroundService
    {

        private readonly ILogger<MediaDataFetchingService> _logger;
        public readonly IServiceScopeFactory _serviceScopeFactory;
        public MediaDataFetchingService(ILogger<MediaDataFetchingService> logger, IServiceScopeFactory serviceScopeFactory)
        {

            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                _logger.LogInformation("Processing started");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var movieUrlFetchService = scope.ServiceProvider.GetRequiredService<IMovieUrlFetch>();

                    await movieUrlFetchService.MovieUrlFetchAndProcess();

                }
                _logger.LogInformation("Movies data database processed");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}