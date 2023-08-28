using DailyMovie.Data;
using DailyMovie.DTO;
using DailyMovie.Entities;
using DailyMovie.ServiceContracts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


            if (_cache.TryGetValue("movieQueue", out List<Movie> currentMovieQueue) && currentMovieQueue.Any())
            {

                var index = _random.Next(currentMovieQueue.Count);
                var recommendedMovie = currentMovieQueue[index];
                return recommendedMovie;
            }

            return null;

        }

        public async Task MovieIsViewedUpdate(int movieId)
        {
            Movie movie = _context.Movies.FirstOrDefault(x => x.Id == movieId && !x.IsViewed);

            if (movie != null)
            {
                movie.ViewedDate = DateTime.UtcNow;
                movie.IsViewed = true;
                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                // Cache güncellemesi
                if (_cache.TryGetValue("movieQueue", out List<Movie> movieQueue))
                {
                    var movieToRemove = movieQueue.FirstOrDefault(m => m.Id == movie.Id);
                    if (movieToRemove != null)
                    {
                        movieQueue.Remove(movieToRemove);
                        _cache.Set("movieQueue", movieQueue, TimeSpan.FromDays(1));
                    }
                }
            }
        }

        public List<Movie> LatestMovies()
        {
            List<Movie> lastMovie = _context.Movies
                                             .Where(x => x.IsViewed == true)
                                             .OrderByDescending(x => x.ViewedDate)
                                             .Take(5)
                                             .ToList();
            return lastMovie;
        }

        #region



        #endregion
    }
}
