using LibraryManagementSystem.Dtos;

namespace LibraryManagementSystem.Services;

public enum CreateBookCopyStatus
{
    Created,
    BookNotFound,
    DuplicateInventoryCode,
    InvalidCondition
}

public sealed record CreateBookCopyResult
(
    CreateBookCopyStatus Status,
    BookCopyDto? BookCopy
);

