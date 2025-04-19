using AutoMapper;
using MajorsModule.DTOs;
using MajorsModule.Repositories;
using Shared.Result;

namespace MajorsModule.Services;

public class MajorsService : IMajorsService
{
    private readonly IMajorsRepository _majorsRepository;
    private readonly IMapper _mapper;

    public MajorsService(IMajorsRepository majorsRepository, IMapper mapper)
    {
        _majorsRepository = majorsRepository;
        _mapper = mapper;
    }
    
    // Implement các method khi cần
} 