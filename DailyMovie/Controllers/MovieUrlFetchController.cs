using DailyMovie.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography.X509Certificates;

namespace DailyMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieUrlFetchController : ControllerBase
    {
        private readonly IMovieUrlFetch _movieUrlFetchService;

        public MovieUrlFetchController(IMovieUrlFetch movieUrlFetchService)
        {
            _movieUrlFetchService = movieUrlFetchService;
        }


        [HttpPost]
        [Route("FetchAndSaveDataAsync")]
        public async Task<IActionResult> Post(int numberOfPages)
        {
           await _movieUrlFetchService.MovieUrlFetchAndSaveAsync(numberOfPages);
            return Ok();

        }

        //[HttpPost]
        //[Route("RenderRandomUrl")]
        //public async Task<IActionResult> Post()
        //{
        //    await _movieUrlFetchService.RenderRandomUrl();
        //    return Ok();

        //}

    }
}
