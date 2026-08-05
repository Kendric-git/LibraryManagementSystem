using LibraryManagementSystem.Dtos;

namespace LibraryManagementSystem.Services;

public interface IBookService
{
    Task<IReadOnlyList<BookSummaryDto>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    Task<BookDetailsDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CreateBookResult> CreateBookAsync(CreateBookDto book, CancellationToken cancellationToken = default);
    Task<UpdateBookStatus> UpdateBookByIdAsync(int id, UpdateBookDto book, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookByIdAsync(int id, CancellationToken cancellationToken = default);
}
