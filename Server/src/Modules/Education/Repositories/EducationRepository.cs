using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Education.Repositories;

public class EducationRepository : IEducationRepository
{
    private readonly ApplicationDbContext _context;

    public EducationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
} 