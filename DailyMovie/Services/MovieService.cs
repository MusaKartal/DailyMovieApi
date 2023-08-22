using DailyMovie.Data;
using DailyMovie.DTO;
using DailyMovie.Entities;
using DailyMovie.ServiceContracts;
using HtmlAgilityPack;

namespace DailyMovie.Services
{
    public class MovieService : IMovie
    {
        private readonly DailyMovieDbContext _context;
        private readonly IMovieUrlFetch _movieUrlFetchService;

        public MovieService(DailyMovieDbContext context, IMovieUrlFetch movieUrlFetchService)
        {
            _context = context;
            _movieUrlFetchService = movieUrlFetchService;
        }

        public async Task RenderUrlAndSaveAsync()
        {
            MovieUrl movieUrl = await _movieUrlFetchService.GetRandomMovieUrl();

            if (movieUrl != null)
            {
                await _movieUrlFetchService.MovieUrlProcessUpdate(movieUrl);
            }

            string url = movieUrl.Url + "?language=en-EN";

            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            var movieInfo = GetMovieInfo(doc);
            string genres = string.Join(", ", movieInfo.Genres);

            if (movieInfo != null)
            {
                Movie movie = new Movie()
                {
                    Title = movieInfo.Title,
                    Image = movieInfo.ImageUrl,
                    Description = movieInfo.History,
                    Genres = genres,
                    Score = movieInfo.Score,
                    Time = movieInfo.Duration,
                    Trailer = movieInfo.TrailerId,
                    Date = movieInfo.ReleaseDate,
                    FullDate = movieInfo.FullReleaseDate,
                    IsViewed = false
                };

                await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();
            }
           
        }


        public async Task<Movie> GetRandomMovie()
        {
            Movie movie = _context.Movies.Where(i => i.IsViewed == false)
                                         .FirstOrDefault();

            if (movie == null)
            {
                // film bulunamadı 
            }
            return movie;

        }

        public async Task MovieIsViewedUpdate(int movieId)
        {
            Movie movie = _context.Movies.Where(i => i.IsViewed == false)
                                          .FirstOrDefault(x => x.Id == movieId);

            if (movie == null)
            {
                // film bulunamadı 
            }       
            movie.IsViewed = true;
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();

        }

        public List<Movie> LatestMovies()
        {
            var movies = _context.Movies.Where(x => x.IsViewed == true).Take(5).ToList();
       
            return movies;
        }

        #region

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

            if (imageNode != null && titleNode != null)
            {
                movieInfo.ImageUrl = "https://www.themoviedb.org" + imageNode.GetAttributeValue("data-src", "");
                movieInfo.Title = titleNode.InnerText;
                movieInfo.ReleaseDate = releaseDateNode.InnerText;
                movieInfo.FullReleaseDate = fullReleaseDateNode?.InnerText;
                movieInfo.Duration = durationNode?.InnerText;
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


        #endregion
    }
}
