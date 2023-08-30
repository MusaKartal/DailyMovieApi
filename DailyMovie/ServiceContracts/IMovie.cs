using DailyMovie.DTO;
using DailyMovie.Entities;

namespace DailyMovie.ServiceContracts
{
    public interface IMovie
    {

        //public Task RenderUrlAndSaveAsync();
        public Task<MovieDetailDto> GetRandomMovie();

        public Task MovieIsViewedUpdate(int movieId);

        public List<MovieDetailDto> LatestMovies();

        
    }
}
