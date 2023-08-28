using DailyMovie.Data;
using DailyMovie.DTO;
using DailyMovie.Entities;
using DailyMovie.ServiceContracts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DailyMovie.Services
{
    public class MovieService : IMovie
    {
        private readonly DailyMovieDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly Random _random = new Random();
        public MovieService(DailyMovieDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;

        }


        public async Task<Movie> GetRandomMovie()
        {

            if (_cache.TryGetValue("filmQueue", out List<Movie> filmQueue) && filmQueue.Any())
            {
                var index = _random.Next(filmQueue.Count);
                var recommendedMovie = filmQueue[index];
                return recommendedMovie;
            }

            return null;

        }

        public async Task MovieIsViewedUpdate(int movieId)
        {
            Movie movie = _context.Movies.Where(i => i.IsViewed == false)
                                          .FirstOrDefault(x => x.Id == movieId);

            if (movie != null)
            {
                movie.IsViewed = true;
                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();
            }
         

        }

        public List<Movie> LatestMovies()
        {
            List<Movie> lastMovie = _context.Movies
                                             .Where(x => x.IsViewed == true)
                                             .OrderByDescending(x => x.Score)
                                             .Take(5)
                                             .ToList();
            return lastMovie;
        }

        #region



        #endregion
    }
}
