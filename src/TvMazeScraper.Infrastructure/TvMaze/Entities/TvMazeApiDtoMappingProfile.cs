using AutoMapper;
using TvMazeScraper.Domain.Entities;

namespace TvMazeScraper.Infrastructure.TvMaze.Entities;

public class TvMazeApiDtoMappingProfile : Profile
{
    public TvMazeApiDtoMappingProfile()
    {
        CreateMap<TvShowApiDto, TvShow>();
        CreateMap<CastMemberApiDto, CastMember>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Person != null ? src.Person.Id : 0))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person != null ? src.Person.Name : string.Empty))
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Person != null ? src.Person.Birthday : DateTime.MinValue));
    }
}