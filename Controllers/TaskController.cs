using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private static List<TaskItem> _tasks = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_tasks);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = _tasks.Count + 1,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false
        };
        _tasks.Add(task);
        return Ok(task);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask (int id, UpdateTaskDto dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound();
        _tasks.Remove(task);

        return NoContent();
    }
}