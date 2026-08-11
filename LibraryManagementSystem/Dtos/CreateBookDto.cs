using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos;

public record CreateBookDto
(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(50)] string Author,
    [Range(1, int.MaxValue)] int GenreId,
    [Range(typeof(decimal), "0.01", "10000")] decimal Price
);