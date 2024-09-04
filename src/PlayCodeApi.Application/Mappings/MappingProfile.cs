using AutoMapper;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.Domain;

namespace PlayCodeApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PlayCode, PlayCodeData>().ReverseMap();
    }
}