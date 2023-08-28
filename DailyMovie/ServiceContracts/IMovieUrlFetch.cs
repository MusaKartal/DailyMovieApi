using DailyMovie.Entities;
using System;

namespace DailyMovie.ServiceContracts
{
    public interface IMovieUrlFetch
    {     
        public Task MovieUrlFetchAndProcess();
    }
}
