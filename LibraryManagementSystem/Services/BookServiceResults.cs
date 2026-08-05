using LibraryManagementSystem.Dtos;

namespace LibraryManagementSystem.Services;

public enum CreateBookStatus
{
    Created,
    GenreNotFound
}

public sealed record CreateBookResult
(
    CreateBookStatus Status,
    BookDetailsDto? Book
);

public enum UpdateBookStatus
{
    Updated,
    BookNotFound,
    GenreNotFound
}
