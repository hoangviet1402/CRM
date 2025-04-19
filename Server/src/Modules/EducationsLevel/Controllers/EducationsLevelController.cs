using Microsoft.AspNetCore.Mvc;
using EducationsLevelModule.DTOs;
using EducationsLevelModule.Services;
using Shared.Result;

namespace EducationsLevelModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EducationsLevelController : ControllerBase
{
    private readonly IEducationsLevelService _service;

    public EducationsLevelController(IEducationsLevelService service)
    {
        _service = service;
    }
} 