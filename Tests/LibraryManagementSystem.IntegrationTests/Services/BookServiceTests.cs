using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.IntegrationTests.Infrastructure;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.IntegrationTests.Services;

public sealed class BookServiceTests
    : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public BookServiceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllBooksAsync_WhenBooksExist_ReturnsBookSummaries()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<BookSummaryDto> result;

        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var firstBook = new Book
            {
                Name = "Project Hail Mary",
                Author = "Andy Weir",
                GenreId = seededGenre.Id,
                Price = 34.99m,
                Stock = 5 
            };
        var secondBook = new Book
            {
                Name = "Star Trek",
                Author = "Jane Doe",
                GenreId = seededGenre.Id,
                Price = 24.99m,
                Stock = 5
            };

        await using (var seedContext = _fixture.CreateDbContext()){
            seedContext.Genres.Add(seededGenre);
            seedContext.Books.Add(firstBook);
            seedContext.Books.Add(secondBook);

            await seedContext.SaveChangesAsync();

        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            result = await service.GetAllBooksAsync();
        }       

        // Assert
        Assert.Equal(2, result.Count);

        var firstBookSummary = result.Single(summary => summary.Id == firstBook.Id);
        var secondBookSummary = result.Single(summary => summary.Id == secondBook.Id);

        Assert.Equal(firstBook.Name, firstBookSummary.Name);
        Assert.Equal(firstBook.Author, firstBookSummary.Author);
        Assert.Equal(seededGenre.Name, firstBookSummary.GenreName);
        Assert.Equal(firstBook.Price, firstBookSummary.Price);
        Assert.Equal(firstBook.Stock, firstBookSummary.Stock);

        Assert.Equal(secondBook.Name, secondBookSummary.Name);
        Assert.Equal(secondBook.Author, secondBookSummary.Author);
        Assert.Equal(seededGenre.Name, secondBookSummary.GenreName);
        Assert.Equal(secondBook.Price, secondBookSummary.Price);
        Assert.Equal(secondBook.Stock, secondBookSummary.Stock);
    }

    [Fact]
    public async Task GetAllBooksAsync_WhenBooksDoNotExist_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<BookSummaryDto> result;
        
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            result = await service.GetAllBooksAsync();
        }

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateBookAsync_WhenGenreExists_ReturnsCreatedAndSavesBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        CreateBookResult result;

        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var request = new CreateBookDto
            (
                "Project Hail Mary",
                "Andy Weir",
                seededGenre.Id,
                34.99m,
                5
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Genres.Add(seededGenre);

            await seedContext.SaveChangesAsync();

            var service = new BookService(seedContext);

            // Act
            result = await service.CreateBookAsync(request);
        }
        
        await using var verificationContext = _fixture.CreateDbContext();
        
        var savedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync();

        // Assert
        Assert.Equal(CreateBookStatus.Created, result.Status);
        Assert.NotNull(result.Book);

        Assert.Equal(request.Name, result.Book.Name);
        Assert.Equal(request.Author, result.Book.Author);
        Assert.Equal(request.GenreId, result.Book.Genre.Id);
        Assert.Equal(request.Price, result.Book.Price);
        Assert.Equal(request.Stock, result.Book.Stock);

        Assert.Equal(request.Name, savedBook.Name);
        Assert.Equal(request.Author, savedBook.Author);
        Assert.Equal(request.GenreId, savedBook.GenreId);
        Assert.Equal(request.Price, savedBook.Price);
        Assert.Equal(request.Stock, savedBook.Stock);
    }
    
    [Fact]
    public async Task CreateBookAsync_WhenGenreDoesNotExist_ReturnsGenreNotFoundAndDoesNotSaveBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        await using var context = _fixture.CreateDbContext();

        var service = new BookService(context);

        var request = new CreateBookDto
        (
            "Project Hail Mary",
            "Andy Weir",
            999,
            34.99m,
            5
        );
        
        // Act
        var result = await service.CreateBookAsync(request);
    
        // Assert
        Assert.Equal(CreateBookStatus.GenreNotFound, result.Status);
        Assert.Null(result.Book);

        var bookWasSaved = await context.Books.AnyAsync();

        Assert.False(bookWasSaved);
    }
    
    [Fact]
    public async Task GetBookByIdAsync_WhenBookExists_ReturnsBookDetails()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        BookDetailsDto? result;

        var seededGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        var seededSecondBook = new Book
        {
            Name = "Star Trek",
            Author = "Jane Doe",
            GenreId = seededGenre.Id,
            Price = 24.99m,
            Stock = 5
        };

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededFirstBook);
            seedContext.Add(seededSecondBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            result = await service.GetBookByIdAsync(seededFirstBook.Id);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(seededFirstBook.Id, result.Id);
        Assert.Equal(seededFirstBook.Name, result.Name);
        Assert.Equal(seededFirstBook.Author, result.Author);
        Assert.Equal(seededGenre.Id, result.Genre.Id);
        Assert.Equal(seededGenre.Name, result.Genre.Name);
        Assert.Equal(seededFirstBook.Price, result.Price);
        Assert.Equal(seededFirstBook.Stock, result.Stock);
    }

    [Fact]
    public async Task GetBookByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        BookDetailsDto? result;
        var missingBookId = 2;

        var seededGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededFirstBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            result = await service.GetBookByIdAsync(missingBookId);
        }

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateBookByIdAsync_WhenBookAndGenreExist_ReturnsUpdatedAndUpdatesOnlyTargetBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        UpdateBookStatus updateStatus;

        var seededFirstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededSecondGenre = new Genre
        {
            Id = 2,
            Name = "Romance"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededFirstGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        var seededSecondBook = new Book
        {
            Name = "Star Trek",
            Author = "Jane Doe",
            GenreId = seededFirstGenre.Id,
            Price = 24.99m,
            Stock = 5
        };

        var request = new UpdateBookDto
            (
                "It Ends With Us",
                "Colleen Hoover",
                seededSecondGenre.Id,
                24.99m,
                10
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededFirstGenre);
            seedContext.Add(seededSecondGenre);
            seedContext.Add(seededFirstBook);
            seedContext.Add(seededSecondBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            updateStatus = await service.UpdateBookByIdAsync(seededFirstBook.Id, request);

        }

        await using var verificationContext = _fixture.CreateDbContext();
        
        var updatedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync(book => book.Id == seededFirstBook.Id);

        var unchangedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync(book => book.Id == seededSecondBook.Id);

        // Assert
        Assert.Equal(UpdateBookStatus.Updated, updateStatus);

        Assert.Equal(request.Name, updatedBook.Name);
        Assert.Equal(request.Author, updatedBook.Author);
        Assert.Equal(request.GenreId, updatedBook.GenreId);
        Assert.Equal(request.Price, updatedBook.Price);
        Assert.Equal(request.Stock, updatedBook.Stock);

        Assert.Equal(seededSecondBook.Name, unchangedBook.Name);
        Assert.Equal(seededSecondBook.Author, unchangedBook.Author);
        Assert.Equal(seededSecondBook.GenreId, unchangedBook.GenreId);
        Assert.Equal(seededSecondBook.Price, unchangedBook.Price);
        Assert.Equal(seededSecondBook.Stock, unchangedBook.Stock);
    }

    [Fact]
    public async Task UpdateBookByIdAsync_WhenBookExistsAndGenreDoesNotExist_ReturnsGenreNotFoundAndDoesNotUpdateBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        UpdateBookStatus updateStatus;

        var seededFirstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededFirstGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        var request = new UpdateBookDto
            (
                "It Ends With Us",
                "Colleen Hoover",
                999,
                24.99m,
                10
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededFirstGenre);
            seedContext.Add(seededFirstBook);

            await seedContext.SaveChangesAsync();
        }
    
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            updateStatus = await service.UpdateBookByIdAsync(seededFirstBook.Id, request);
        }

        await using var verificationContext = _fixture.CreateDbContext();

        var unchangedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync(book => book.Id == seededFirstBook.Id);

        // Assert
        Assert.Equal(UpdateBookStatus.GenreNotFound, updateStatus);

        Assert.Equal(seededFirstBook.Name, unchangedBook.Name);
        Assert.Equal(seededFirstBook.Author, unchangedBook.Author);
        Assert.Equal(seededFirstBook.GenreId, unchangedBook.GenreId);
        Assert.Equal(seededFirstBook.Price, unchangedBook.Price);
        Assert.Equal(seededFirstBook.Stock, unchangedBook.Stock);
    }

    [Fact]
    public async Task UpdateBookByIdAsync_WhenBookDoesNotExistAndGenreExists_ReturnsBookNotFoundAndDoesNotUpdateBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        UpdateBookStatus updateStatus;

        var seededFirstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededFirstGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        var request = new UpdateBookDto
            (
                "It Ends With Us",
                "Colleen Hoover",
                seededFirstGenre.Id,
                24.99m,
                10
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededFirstGenre);
            seedContext.Add(seededFirstBook);

            await seedContext.SaveChangesAsync();
        }
    
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            updateStatus = await service.UpdateBookByIdAsync(999, request);
        }

        await using var verificationContext = _fixture.CreateDbContext();

        var unchangedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync(book => book.Id == seededFirstBook.Id);

        // Assert
        Assert.Equal(UpdateBookStatus.BookNotFound, updateStatus);

        Assert.Equal(seededFirstBook.Name, unchangedBook.Name);
        Assert.Equal(seededFirstBook.Author, unchangedBook.Author);
        Assert.Equal(seededFirstBook.GenreId, unchangedBook.GenreId);
        Assert.Equal(seededFirstBook.Price, unchangedBook.Price);
        Assert.Equal(seededFirstBook.Stock, unchangedBook.Stock);
    }

    [Fact]
    public async Task DeleteBookByIdAsync_WhenBookExists_ReturnsTrueAndDeletesOnlyTargetBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        bool deleteResult;

        var seededFirstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededFirstGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        var seededSecondBook = new Book
        {
            Name = "Star Trek",
            Author = "Jane Doe",
            GenreId = seededFirstGenre.Id,
            Price = 24.99m,
            Stock = 5
        };

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededFirstGenre);
            seedContext.Add(seededFirstBook);
            seedContext.Add(seededSecondBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            deleteResult = await service.DeleteBookByIdAsync(seededFirstBook.Id);
        }
        
        // Assert
        await using var verificationContext = _fixture.CreateDbContext();

        var targetStillExists = await verificationContext.Books
            .AsNoTracking()
            .AnyAsync(book => book.Id == seededFirstBook.Id);
        
        var otherBookStillExists = await verificationContext.Books
            .AsNoTracking()
            .AnyAsync(book => book.Id == seededSecondBook.Id);
        
        Assert.True(deleteResult);
        Assert.False(targetStillExists);
        Assert.True(otherBookStillExists);
    }

    [Fact]
    public async Task DeleteBookByIdAsync_WhenBookDoesNotExist_ReturnsFalseAndDoesNotDeleteExistingBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        bool result;

        var seededFirstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"  
        };

        var seededFirstBook = new Book
        {
            Name = "Project Hail Mary",
            Author = "Andy Weir",
            GenreId = seededFirstGenre.Id,
            Price = 34.99m,
            Stock = 5 
        };

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededFirstGenre);
            seedContext.Add(seededFirstBook);

            await seedContext.SaveChangesAsync();
        }
    
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookService(serviceContext);

            result = await service.DeleteBookByIdAsync(999);
        }

        // Assert
        await using var verificationContext = _fixture.CreateDbContext();

        var unchangedBook = await verificationContext.Books
            .AsNoTracking()
            .SingleAsync(book => book.Id == seededFirstBook.Id);
        
        Assert.False(result);

        Assert.Equal(seededFirstBook.Name, unchangedBook.Name);
        Assert.Equal(seededFirstBook.Author, unchangedBook.Author);
        Assert.Equal(seededFirstBook.GenreId, unchangedBook.GenreId);
        Assert.Equal(seededFirstBook.Price, unchangedBook.Price);
        Assert.Equal(seededFirstBook.Stock, unchangedBook.Stock);
    }
}