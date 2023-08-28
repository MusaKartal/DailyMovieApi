using DailyMovie.Data;
using DailyMovie.DTO;
using DailyMovie.Entities;
using DailyMovie.ServiceContracts;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;


namespace DailyMovie.Services
{
    public class MovieUrlFetchService : IMovieUrlFetch
    {
        private readonly DailyMovieDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKeyPrefix = "MovieUrlFetch_";

        public MovieUrlFetchService(DailyMovieDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }


        public async Task MovieUrlFetchAndProcess()
        {
            int initialMinPage = 1;
            int initialMaxPage = 2;

            string baseUrl = "https://www.themoviedb.org";
            string movieUrl = "/movie/top-rated?page=";
            string language = "?language=en-EN";
            string url = baseUrl + movieUrl;


            await MovieUrlFetch(url, baseUrl, initialMinPage, initialMaxPage, language);
        }



        #region Private method
        private async Task MovieUrlFetch(string url, string baseUrl, int initialMinPage, int initialMaxPage, string language)
        {
            int startPage = _memoryCache.TryGetValue<int>(CacheKeyPrefix + "StartPage", out var cachedStartPage)
                ? cachedStartPage
                : initialMinPage;

            int maxPage = _memoryCache.TryGetValue<int>(CacheKeyPrefix + "MaxPage", out var cachedMaxPage)
                ? cachedMaxPage
                : initialMaxPage;

            var existingMovieIds = new HashSet<string>(_context.Movies.Select(m => m.MovieId));

            string html = url + startPage;
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(html);
            HtmlNodeCollection movieNodes = doc.DocumentNode.SelectNodes("//h2//a");
            if (movieNodes == null)
            {
                startPage = initialMinPage;
                maxPage = initialMaxPage;
                _memoryCache.Remove(startPage);
                _memoryCache.Remove(maxPage);
            }
            foreach (HtmlNode movieNode in movieNodes)
            {
                string title = movieNode.GetAttributeValue("title", "");
                string moviId = movieNode.GetAttributeValue("href", "");

                //title uygun url formatına getirmek için
                string input = title;
                string pattern = @"[^a-zA-Z0-9]+";
                string[] words = Regex.Split(input, pattern);
                string resultTitle = "-" + string.Join("-", words).ToLower();

                string fetchUrl = baseUrl + moviId + resultTitle;

                if (!existingMovieIds.Contains(moviId))
                {
                    await ProcessMovieData(fetchUrl, moviId, language);
                }

            }

            startPage += initialMaxPage - initialMinPage + 1;
            maxPage += initialMaxPage - initialMinPage + 1;
            _memoryCache.Set(CacheKeyPrefix + "StartPage", startPage);
            _memoryCache.Set(CacheKeyPrefix + "MaxPage", maxPage);

        }

        private async Task ProcessMovieData(string url, string movieId, string language)
        {

            string movieFetchUrl = url + language;

            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(movieFetchUrl);

            var movieInfo = GetMovieInfo(doc);

            string genres = string.Join(", ", movieInfo.Genres);

            if (movieInfo != null)
            {
                await MovieSaveDatabase(movieInfo, genres, movieFetchUrl, movieId);
            }
        }

        private MovieInfo GetMovieInfo(HtmlDocument document)
        {
            var movieInfo = new MovieInfo();

            var imageNode = document.DocumentNode.SelectSingleNode("//div[@class='image_content backdrop']//img");
            var titleNode = document.DocumentNode.SelectSingleNode("//h2//a");
            var releaseDateNode = document.DocumentNode.SelectSingleNode("//h2//span");
            var fullReleaseDateNode = document.DocumentNode.SelectSingleNode("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[2]/text()");
            var scoreNode = document.DocumentNode.SelectSingleNode("//div[@class='user_score_chart']");
            var genreNodes = document.DocumentNode.SelectNodes("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[3]");
            var historyNode = document.DocumentNode.SelectSingleNode("//*[@id=\"original_header\"]/div[2]/section/div[2]/div/p/text()");
            var trailerNode = document.DocumentNode.SelectSingleNode("//a[@class='no_click play_trailer']");
            var durationNode = document.DocumentNode.SelectSingleNode("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[4]/text()");

            var cerfication = document.DocumentNode.SelectSingleNode("//span[@class='certification']");

            if (cerfication == null)
            {
                fullReleaseDateNode = document.DocumentNode.SelectSingleNode("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[1]");
                durationNode = document.DocumentNode.SelectSingleNode("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[3]/text()");
                genreNodes = document.DocumentNode.SelectNodes("//*[@id=\"original_header\"]/div[2]/section/div[1]/div/span[2]");
            }
            if (imageNode != null && titleNode != null)
            {
                movieInfo.ImageUrl = "https://www.themoviedb.org" + imageNode.GetAttributeValue("data-src", "");
                movieInfo.Title = titleNode.InnerText;
                movieInfo.ReleaseDate = releaseDateNode?.InnerText;
                movieInfo.FullReleaseDate = fullReleaseDateNode?.InnerText.Replace("\n", "").Replace(" ", "").Trim();
                movieInfo.Duration = durationNode?.InnerText.Replace("\n", "").Replace(" ", "").Trim();
                movieInfo.Score = scoreNode?.GetAttributeValue("data-percent", "");
                movieInfo.TrailerId = trailerNode?.GetAttributeValue("data-id", "");
                movieInfo.History = historyNode?.InnerText;

                if (genreNodes != null)
                {
                    foreach (var genreNode in genreNodes)
                    {
                        string genre = genreNode.InnerText.Replace("&nbsp;", "").Trim();
                        if (!string.IsNullOrWhiteSpace(genre))
                        {
                            movieInfo.Genres.Add(genre);

                        }
                    }

                }

            }

            return movieInfo;
        }
        private async Task MovieSaveDatabase(MovieInfo movieInfo, string genres, string movieFetchUrl, string movieId)
        {
            Movie movie = new Movie()
            {
                Title = movieInfo.Title,
                Image = movieInfo.ImageUrl,
                Description = movieInfo.History ?? "Default",
                Genres = genres,
                Score = movieInfo.Score ?? "Default",
                Time = movieInfo.Duration ?? "Default",
                Trailer = movieInfo.TrailerId ?? "Default",
                Date = movieInfo.ReleaseDate,
                FullDate = movieInfo.FullReleaseDate ?? "Default",
                MovieId = movieId,
                Url = movieFetchUrl,
                PullDate = DateTime.UtcNow,
                IsViewed = false
            };


            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }
        #endregion

    }
}





