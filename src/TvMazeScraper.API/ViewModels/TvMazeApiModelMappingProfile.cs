using AutoMapper;
using TvMazeScraper.Domain.Entities;

namespace TvMazeScraper.API.ViewModels;

public class TvMazeApiModelMappingProfile : Profile
{
    public TvMazeApiModelMappingProfile()
    {
        CreateMap<TvShow, TvShowModel>();
        CreateMap<CastMember, CastMemberModel>();
    }
}