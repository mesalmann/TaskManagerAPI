using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public record TaskReadDto(
        int Id,
        string Title,
        string? Description,
        bool IsCompleted,
        DateTime CreatedAt
    );

    public class TaskCreateDto
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public class TaskUpdateDto
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
