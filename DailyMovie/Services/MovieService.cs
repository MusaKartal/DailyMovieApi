using AutoMapper;
using DailyMovie.Data;
using DailyMovie.DTO;
using DailyMovie.Entities;
using DailyMovie.ErrorModel;
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
        private readonly IMapper _mapper;
        public MovieService(DailyMovieDbContext context, IMemoryCache cache, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }


        public async Task<MovieDetailDto> GetRandomMovie()
        {


            if (_cache.TryGetValue("movieQueu", out List<Movie> currentMovieQueue) && currentMovieQueue.Any())
            {
                var index = _random.Next(currentMovieQueue.Count);
                var recommendedMovie = currentMovieQueue[index];
                var movieDetailDto = _mapper.Map<MovieDetailDto>(recommendedMovie);
                return movieDetailDto;
            }
          
            Movie firstUnviewedMovie = _context.Movies.FirstOrDefault(m => m.IsViewed == false);
            var movieDetail = _mapper.Map<MovieDetailDto>(firstUnviewedMovie);
            if (firstUnviewedMovie == null)
            {
                throw new KeyNotFoundException("Movie not found");
            }
            return movieDetail;

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
                    else
                    {
                        throw new KeyNotFoundException("cache is empty so couldn't update");
                    }
                }
                else
                {
                    throw new SomeException("Movie cache value not found");
                }

            }
        }

        public List<MovieDetailDto> LatestMovies()
        {
            List<Movie> lastMovie = _context.Movies
                                             .Where(x => x.IsViewed == true)
                                             .OrderByDescending(x => x.ViewedDate)
                                             .Take(5)
                                             .ToList();
            var movieDetails = _mapper.Map<List<MovieDetailDto>>(lastMovie);
            if (movieDetails == null)
            {
                throw new KeyNotFoundException("No movies viewed");
            }

            return movieDetails;
        }

        #region



        #endregion
    }
}
