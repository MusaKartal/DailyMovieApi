using DailyMovie.Entities;

namespace DailyMovie.ServiceContracts
{
    public interface IMovie
    {

        //public Task RenderUrlAndSaveAsync();
        public Task<Movie> GetRandomMovie();

        public Task MovieIsViewedUpdate(int movieId);

        public List<Movie> LatestMovies();

        
    }
}
