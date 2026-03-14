namespace TaskFlow.Api.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
}