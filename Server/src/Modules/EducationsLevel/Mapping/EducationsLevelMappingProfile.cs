using AutoMapper;
using EducationsLevelModule.DTOs;
using EducationsLevelModule.Entities;

namespace EducationsLevelModule.Mapping;

public class EducationsLevelMappingProfile : Profile
{
    public EducationsLevelMappingProfile()
    {
        CreateMap<EducationsLevelEntity, EducationsLevelDTO>().ReverseMap();
    }
} 