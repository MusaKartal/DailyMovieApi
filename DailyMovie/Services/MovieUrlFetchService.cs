using DailyMovie.Data;
using DailyMovie.Entities;
using DailyMovie.ServiceContracts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Xml;

namespace DailyMovie.Services
{
    public class MovieUrlFetchService : IMovieUrlFetch
    {
        private readonly DailyMovieDbContext _context;

        public MovieUrlFetchService(DailyMovieDbContext context)
        {
            _context = context;
        }

        public async Task MovieUrlFetchAndSaveAsync(int numberOfPages)
        {
            var tasks = new List<Task>();

            // Paralel işlem yaparak veri çekme ve kaydetme işlemlerini optimize et
            Parallel.ForEach(Partitioner.Create(1, numberOfPages + 1), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    tasks.Add(ProcessPageAsync(i));
                }
            });

            // Tüm görevleri beklet
            await Task.WhenAll(tasks);
        }


        public async Task<MovieUrl> GetRandomMovieUrl()
        {
            var random = new Random();
            var allprocessedMovies = await _context.MovieUrls.ToListAsync(); // Tüm işlenmemiş filmleri alın

            var randomMovie = allprocessedMovies
                .OrderBy(x => random.Next()) // Rastgele bir sıralama yapın
                .FirstOrDefault(); // İlk öğeyi seçin      
            if (randomMovie == null)
            {
                //hata dön
            }
            return randomMovie;
        }

         
        public async Task MovieUrlProcessUpdate(MovieUrl movieUrl)
        {
            movieUrl.IsProcessed = true;
            movieUrl.ProcessingDate = DateTime.UtcNow;
            _context.MovieUrls.Update(movieUrl);      
        }


        #region private Methods
        private async Task ProcessPageAsync(int pageNumber)
        {
            // Her istek sonrası 1 saniye gecikme ekleyin
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            var html = @"https://www.themoviedb.org/movie?page=" + pageNumber;
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(html);
            HtmlNodeCollection divNodes = doc.DocumentNode.SelectNodes("//h2//a");

            if (divNodes != null)
            {
                var movieURLs = new List<MovieUrl>();

                foreach (HtmlNode divNode in divNodes)
                {
                    string title = divNode.GetAttributeValue("title", "");
                    string href = divNode.GetAttributeValue("href", "");

                    var url = UrlFormat(title, href);

                    // MovieId benzer değer varsa devam et
                    if (await MovieIdExistsAsync(href) == false)
                    {
                        MovieUrl movieURL = new MovieUrl
                        {
                            Title = title,
                            IsProcessed = false,
                            MovieId = href,
                            Url = url,
                            PullDate = DateTime.UtcNow,
                        };

                        movieURLs.Add(movieURL);

                    }

                }
                // Paralel olmayan şekilde kaydetmek yerine, bir seferde tüm veriyi ekleyip kaydedebilirsiniz
                await _context.MovieUrls.AddRangeAsync(movieURLs);
                await _context.SaveChangesAsync();
            }
        }


        private async Task<bool> MovieIdExistsAsync(string movieId)
        {
            // Veritabanında aynı MovieId'ye sahip kayıt olup olmadığını kontrol et
            return await _context.MovieUrls.AnyAsync(x => x.MovieId == movieId);
        }

        private string UrlFormat(string title, string href)
        {
            string url = "https://www.themoviedb.org";
            string input = title;
            string pattern = @"[^a-zA-Z0-9]+";
            string[] words = Regex.Split(input, pattern);
            string result = "-" + string.Join("-", words).ToLower();

            return url + href + result;
        }

       
        #endregion
    }
}
