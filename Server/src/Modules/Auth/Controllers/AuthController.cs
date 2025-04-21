using System.Numerics;
using AuthModule.DTOs;
using AuthModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;

namespace AuthModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
}