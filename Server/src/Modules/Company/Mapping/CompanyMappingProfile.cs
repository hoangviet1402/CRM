using AutoMapper;

namespace Company.Mapping;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Entities.Company, CompanyDTO>();
        CreateMap<CompanyCreateDTO, Entities.Company>();
        CreateMap<CompanyDTO, Entities.Company>();
    }
} 