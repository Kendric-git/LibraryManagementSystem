using LibraryManagementSystem.Dtos;

namespace LibraryManagementSystem.Services;

public interface IBookCopyService
{
    Task<IReadOnlyList<BookCopyDto>> GetBookCopiesByBookIdAsync(int bookId, CancellationToken cancellationToken = default);
    Task<BookCopyDto?> GetBookCopyByIdAsync(int copyId, CancellationToken cancellationToken = default);
    Task<CreateBookCopyResult> CreateBookCopyAsync(int bookId, CreateBookCopyDto request, CancellationToken cancellationToken = default);
}
