namespace LibraryManagementSystem.Dtos;

public sealed record BookDetailsDto
(
    int Id,
    string Name,
    string Author,
    GenreDto Genre,
    decimal Price,
    int ActiveCopyCount
);
