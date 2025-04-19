using AutoMapper;
using Education.DTOs;
using Education.Repositories;
using Shared.Result;

namespace Education.Services;

public class EducationService : IEducationService
{
    private readonly IEducationRepository _repository;
    private readonly IMapper _mapper;

    public EducationService(IEducationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
} 