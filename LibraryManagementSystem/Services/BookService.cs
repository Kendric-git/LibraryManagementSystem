using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public sealed class BookService(LibrarySystemContext dbContext) : IBookService
{
    public async Task<IReadOnlyList<BookSummaryDto>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Select(book => new BookSummaryDto(
                book.Id,
                book.Name,
                book.Author,
                book.Genre!.Name,
                book.Price,
                book.Stock
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<BookDetailsDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Books
            .Where(book => book.Id == id)
            .Select(book => new BookDetailsDto (
                book.Id,
                book.Name,
                book.Author,
                new GenreDto(book.GenreId, book.Genre!.Name),
                book.Price,
                book.Stock
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateBookResult> CreateBookAsync(CreateBookDto newBook, CancellationToken cancellationToken = default)
    {
        var genre = await dbContext.Genres
            .AsNoTracking()
            .SingleOrDefaultAsync(
                genre => genre.Id == newBook.GenreId,
                cancellationToken);

        if (genre is null)
        {
            return new CreateBookResult(CreateBookStatus.GenreNotFound, null);
        }

        Book book = new()
        {
            Name = newBook.Name,
            Author = newBook.Author,
            GenreId = newBook.GenreId,
            Price = newBook.Price,
            Stock = newBook.Stock
        };

        dbContext.Books.Add(book);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdBook = new BookDetailsDto(
            book.Id,
            book.Name,
            book.Author,
            new GenreDto(genre.Id, genre.Name),
            book.Price,
            book.Stock
        );

        return new CreateBookResult(CreateBookStatus.Created, createdBook);
    }

    public async Task<UpdateBookStatus> UpdateBookByIdAsync(int id,UpdateBookDto updateBook,CancellationToken cancellationToken = default)
    {
        var existingBook = await dbContext.Books.FindAsync([id],cancellationToken);

        if (existingBook is null)
        {
            return UpdateBookStatus.BookNotFound;
        }

        var genreExists = await dbContext.Genres
            .AsNoTracking()
            .AnyAsync(
                genre => genre.Id == updateBook.GenreId,
                cancellationToken);

        if (!genreExists)
        {
            return UpdateBookStatus.GenreNotFound;
        }

        existingBook.Name = updateBook.Name;
        existingBook.Author = updateBook.Author;
        existingBook.GenreId = updateBook.GenreId;
        existingBook.Price = updateBook.Price;
        existingBook.Stock = updateBook.Stock;

        await dbContext.SaveChangesAsync(cancellationToken);

        return UpdateBookStatus.Updated;
    }

    public async Task<bool> DeleteBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var rowsDeleted = await dbContext.Books
            .Where(book => book.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
            
        if (rowsDeleted == 0) {
            return false;
        }

        return rowsDeleted > 0;
    }
}
