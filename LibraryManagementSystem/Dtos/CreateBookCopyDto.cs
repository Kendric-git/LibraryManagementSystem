using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos;

public record CreateBookCopyDto 
(
    [Required][StringLength(50)]  string InventoryCode,
    [Required][StringLength(20)]  string Condition
);
