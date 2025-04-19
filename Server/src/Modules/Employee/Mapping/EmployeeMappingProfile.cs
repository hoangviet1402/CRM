using AutoMapper;
using EmployeeModule.DTOs;
using EmployeeModule.Entities;

namespace EmployeeModule.Mapping;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<EmployeeEntity, EmployeeDto>().ReverseMap();
    }
} 