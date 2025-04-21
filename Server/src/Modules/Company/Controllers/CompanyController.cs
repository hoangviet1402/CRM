using System.Numerics;
using Company.DTOs;
using Company.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;

namespace Company.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }
}