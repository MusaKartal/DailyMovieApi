using DailyMovie.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DailyMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovie _movieService;

        public MovieController(IMovie movieService)
        {
            _movieService = movieService;
        }


        [HttpGet]
        [Route("GetMovie")]
        public IActionResult GetRandomMovie()
        {
            var movie = _movieService.GetRandomMovie();

            if (movie == null)
            {
                return NotFound("No recommendations available.");
            }

            return Ok(movie);
        }
        
        [HttpGet]
        [Route("LastMovies")]
        public IActionResult GetLatestMovies()
        {
            var movie = _movieService.LatestMovies();

            return Ok(movie);
        }

        [HttpPut]
        [Route("IsViewedUpdate")]
        public IActionResult MovieIsViewedUpdate(int movieId)
        {
            var movie = _movieService.MovieIsViewedUpdate(movieId);

            return Ok(movie);
        }
    }
}
