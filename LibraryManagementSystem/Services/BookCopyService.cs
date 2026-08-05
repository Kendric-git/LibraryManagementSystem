using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Services;

public sealed class BookCopyService (LibrarySystemContext dbContext) : IBookCopyService
{
     public async Task<IReadOnlyList<BookCopyDto>> GetBookCopiesByBookIdAsync(int bookId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookCopies
            .AsNoTracking()
            .Where(bookCopy => bookCopy.BookId == bookId)
            .Select(bookCopy => new BookCopyDto (
                bookCopy.Id,
                bookCopy.InventoryCode,
                bookCopy.Condition.ToString(),
                bookCopy.AcquiredAt,
                bookCopy.RetiredAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<BookCopyDto?> GetBookCopyByIdAsync(int copyId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookCopies
            .AsNoTracking()
            .Where(bookCopy => bookCopy.Id == copyId)
            .Select(bookCopy => new BookCopyDto (
                bookCopy.Id,
                bookCopy.InventoryCode,
                bookCopy.Condition.ToString(),
                bookCopy.AcquiredAt,
                bookCopy.RetiredAt
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
