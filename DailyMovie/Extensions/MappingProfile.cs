using AutoMapper;
using DailyMovie.DTO;
using DailyMovie.Entities;

namespace DailyMovie.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<MovieDetailDto, Movie>();
            CreateMap<Movie, MovieDetailDto>();

        }
    }
}
