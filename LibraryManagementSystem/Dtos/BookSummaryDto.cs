namespace LibraryManagementSystem.Dtos;

public sealed record BookSummaryDto
(
    int Id,
    string Name,
    string Author,
    string GenreName,
    decimal Price,
    int Stock
);
