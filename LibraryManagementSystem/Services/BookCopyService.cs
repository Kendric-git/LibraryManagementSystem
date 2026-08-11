using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Models;
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

    public async Task<CreateBookCopyResult> CreateBookCopyAsync(int bookId, CreateBookCopyDto request, CancellationToken cancellationToken = default)
    {
        var bookExists = await dbContext.Books
            .AnyAsync(book => book.Id == bookId, cancellationToken);
        
        if (!bookExists)
        {
            return new CreateBookCopyResult(CreateBookCopyStatus.BookNotFound, null);
        }

        var inventoryCode = request.InventoryCode.Trim().ToUpperInvariant();

        var inventoryCodeExists = await dbContext.BookCopies
            .AnyAsync(book => book.InventoryCode == inventoryCode, cancellationToken);

        if (inventoryCodeExists)
        {
            return new CreateBookCopyResult(CreateBookCopyStatus.DuplicateInventoryCode, null);
        }

        var parsedCondition = Enum.TryParse<BookCopyCondition>
        (
            request.Condition,
            ignoreCase: true,
            out var condition
        );

        if (!parsedCondition || !Enum.IsDefined(condition))
        {
            return new CreateBookCopyResult(CreateBookCopyStatus.InvalidCondition, null);
        }

        BookCopy bookCopy = new()
        {
            BookId = bookId,
            InventoryCode = inventoryCode,
            Condition = condition
        };
        
        dbContext.BookCopies.Add(bookCopy);

        await dbContext.SaveChangesAsync(cancellationToken);

        var createdCopy = new BookCopyDto 
        (
            bookCopy.Id,
            bookCopy.InventoryCode,
            bookCopy.Condition.ToString(),
            bookCopy.AcquiredAt,
            bookCopy.RetiredAt
        );

        return new CreateBookCopyResult(CreateBookCopyStatus.Created, createdCopy);
    }
}
