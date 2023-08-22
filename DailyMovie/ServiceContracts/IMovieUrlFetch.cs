using DailyMovie.Entities;
using System;

namespace DailyMovie.ServiceContracts
{
    public interface IMovieUrlFetch
    {
        public Task MovieUrlFetchAndSaveAsync(int numberOfPages);

        public Task<MovieUrl> GetRandomMovieUrl();

        public Task MovieUrlProcessUpdate(MovieUrl movieUrl);
    }
}
