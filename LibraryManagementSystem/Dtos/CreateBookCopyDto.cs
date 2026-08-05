using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos;

public record CreateBookCopyDto 
(
    [Required] string InventoryCode,
    [Required] string Condition
);
