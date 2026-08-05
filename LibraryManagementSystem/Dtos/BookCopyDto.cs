namespace LibraryManagementSystem.Dtos;

public sealed record BookCopyDto
(
    int Id,
    string InventoryCode,
    string Condition,
    DateTime AcquiredAt,
    DateTime? RetiredAt
);
