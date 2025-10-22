using System.ComponentModel.DataAnnotations;
//   DataAnnotations kütüphanesi doğrulama (validation) için kullanılır.
//   [Required], [StringLength], [Range] gibi öznitelikler (attribute) sağlar.
//   Bu sayede gelen verileri controller’a ulaşmadan önce otomatik kontrol edebilir

namespace TaskManagerAPI.DTOs
{
    // TaskReadDto → veriyi kullanıcıya dönerken (Response) kullanılan model.
    //   'record' türü, immutable (değiştirilemez) veri yapısıdır — sadece veri tutar, davranış (method) içermez.
    public record TaskReadDto(
        int Id,
        string Title,
        string? Description,
        bool IsCompleted,
        DateTime CreatedAt
    );

    public class TaskCreateDto //kullanıcı yeni görev eklerken (POST) kullandığı model.
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public class TaskUpdateDto //kullanıcı görevi güncellerken (PUT/PATCH) kullandığı model.
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
