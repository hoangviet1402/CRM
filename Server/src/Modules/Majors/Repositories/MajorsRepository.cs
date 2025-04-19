using AutoMapper;
using Infrastructure.DbContext;
using MajorsModule.Entities;
using MajorsModule.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MajorsModule.Repositories;

public class MajorsRepository : IMajorsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

   
} 