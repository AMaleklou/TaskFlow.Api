using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;
using TaskFlow.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.JsonPatch;
using TaskFlow.Api.Common;

namespace TaskFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
          _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber = 1 , int pageSize = 10,
                                            bool? isCompleted = null , string? search = null)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Max(pageSize, 1);
        pageSize = Math.Min(pageSize, 50);

        var userId = GetUserId();
        var query =  _context.Tasks
                          .Where(t => t.UserId == userId)
                          .AsNoTracking();
        
        if(isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.Title.Contains(search) ||
                t.Description.Contains(search));
        }
        var totalCount = await query.CountAsync();
        var tasks = await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();

        return Ok(ApiResponse.Success(new
        {
            pageNumber,
            pageSize,
            totalCount,
            data = tasks
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var task = await _context.Tasks
                                 .Where(t => t.Id == id && t.UserId == userId)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync();
        if (task == null)
            return NotFound(ApiResponse.Fail<string>("Task not found"));

        return Ok(ApiResponse.Success(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false
        };
         _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

       // return Ok(ApiResponse.Success(task,"Created"));
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ApiResponse.Success(task, "Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask (int id, UpdateTaskDto dto)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId ==userId);

        if (task == null)
            return NotFound(ApiResponse.Fail<string>("Task not found"));

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(task,"Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound(ApiResponse.Fail<string>("Task not found"));
        task.IsDeleted = true;
       await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success<string>(null, "Task deleted"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchTask(int id, [FromBody] JsonPatchDocument<UpdateTaskPatchDto> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null)
            return NotFound(ApiResponse.Fail<string>("Task not found"));

        // تبدیل entity به DTO
        var taskDto = new UpdateTaskPatchDto
        {
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        };

        // اعمال patch
        patchDoc.ApplyTo(taskDto, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail<object>("Invalid patch data"));

        // برگردوندن به entity
        task.Title = taskDto.Title ?? task.Title;
        task.Description = taskDto.Description ?? task.Description;
        task.IsCompleted = taskDto.IsCompleted ?? task.IsCompleted;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success(task,"Updated"));
    }

    private int GetUserId ()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}