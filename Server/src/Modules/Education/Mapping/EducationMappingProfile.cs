using AutoMapper;
using Education.DTOs;

namespace Education.Mapping;

public class EducationMappingProfile : Profile
{
    public EducationMappingProfile()
    {
        CreateMap<Entities.Education, EducationDto>().ReverseMap();
    }
} 