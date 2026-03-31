namespace TaskFlow.Api.DTOs
{
    public class UpdateTaskPatchDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
