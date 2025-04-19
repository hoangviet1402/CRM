using AutoMapper;
using MajorsModule.DTOs;
using MajorsModule.Entities;

namespace MajorsModule.Mapping;

public class MajorsMappingProfile : Profile
{
    public MajorsMappingProfile()
    {
        CreateMap<MajorsEntity, MajorsDTO>().ReverseMap();
    }
} 