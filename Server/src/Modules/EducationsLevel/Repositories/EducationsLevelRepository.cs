using AutoMapper;
using Infrastructure.DbContext;
using EducationsLevelModule.DTOs;
using EducationsLevelModule.Entities;

namespace EducationsLevelModule.Repositories;

public class EducationsLevelRepository : IEducationsLevelRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EducationsLevelRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
} 