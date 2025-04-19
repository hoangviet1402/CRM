using AutoMapper;
using EducationsLevelModule.DTOs;
using EducationsLevelModule.Repositories;
using Shared.Result;

namespace EducationsLevelModule.Services;

public class EducationsLevelService : IEducationsLevelService
{
    private readonly IEducationsLevelRepository _repository;
    private readonly IMapper _mapper;

    public EducationsLevelService(IEducationsLevelRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
} 