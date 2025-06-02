using Microsoft.AspNetCore.Mvc;
using TaskModule.Services;
using TaskModule.DTOs;

namespace TaskModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // Example endpoint
    [HttpGet]
    public IActionResult GetTasks()
    {
        // TODO: Implement logic
        return Ok();
    }
}
