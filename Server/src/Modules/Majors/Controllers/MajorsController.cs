using MajorsModule.DTOs;
using MajorsModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace MajorsModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MajorsController : ControllerBase
{
    private readonly IMajorsService _majorsService;

    public MajorsController(IMajorsService majorsService)
    {
        _majorsService = majorsService;
    }
    
    // Implement các endpoint khi cần
} 