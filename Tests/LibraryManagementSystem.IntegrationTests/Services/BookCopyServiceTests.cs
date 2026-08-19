
using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.IntegrationTests.Infrastructure;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.IntegrationTests.Services;

public sealed class BookCopyServiceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public BookCopyServiceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetBookCopiesByBookIdAsync_WhenBookExists_ReturnsBookCopies()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<BookCopyDto> result;

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
                Price = 34.99m
            };
        var seededFirstBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-001",
                Condition = BookCopyCondition.Good
            };
        var seededSecondBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-002",
                Condition = BookCopyCondition.New
            };
    
        firstBook.Copies.Add(seededFirstBookCopy);
        firstBook.Copies.Add(seededSecondBookCopy);

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(firstBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.GetBookCopiesByBookIdAsync(firstBook.Id);
        }

        // Assert
        Assert.Equal(2, result.Count);

        var firstBookCopy = result.Single(copy => copy.Id == seededFirstBookCopy.Id);
        var secondBookCopy = result.Single(copy => copy.Id == seededSecondBookCopy.Id);

        Assert.Equal(seededFirstBookCopy.Id, firstBookCopy.Id);
        Assert.Equal(seededFirstBookCopy.InventoryCode, firstBookCopy.InventoryCode);
        Assert.Equal(seededFirstBookCopy.Condition.ToString(), firstBookCopy.Condition);

        Assert.Equal(seededSecondBookCopy.Id, secondBookCopy.Id);
        Assert.Equal(seededSecondBookCopy.InventoryCode, secondBookCopy.InventoryCode);
        Assert.Equal(seededSecondBookCopy.Condition.ToString(), secondBookCopy.Condition);
    }

    [Fact]
    public async Task GetBookCopiesByBookIdAsync_WhenBookExists_ReturnsOnlyBookCopiesOfThatBook()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<BookCopyDto> result;

        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var seededSecondGenre = new Genre
            {
                Id = 2,
                Name = "Romance"
            };
        var firstBook = new Book
            {
                Name = "Project Hail Mary",
                Author = "Andy Weir",
                GenreId = seededGenre.Id,
                Price = 34.99m
            };
        var secondBook = new Book
            {
                Name = "It Ends With Us",
                Author = "Colleen Hoover",
                GenreId = seededSecondGenre.Id,
                Price = 34.99m
            };
        var seededFirstBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-001",
                Condition = BookCopyCondition.Good
            };
        var seededSecondBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-002",
                Condition = BookCopyCondition.New
            };
        var seededThirdBookCopy = new BookCopy
            {
                InventoryCode = "COPY-2-001",
                Condition = BookCopyCondition.New
            };
        var seededFourthBookCopy = new BookCopy
            {
                InventoryCode = "COPY-2-002",
                Condition = BookCopyCondition.Fair
            };
    
        firstBook.Copies.Add(seededFirstBookCopy);
        firstBook.Copies.Add(seededSecondBookCopy);
        secondBook.Copies.Add(seededThirdBookCopy);
        secondBook.Copies.Add(seededFourthBookCopy);

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededSecondGenre);
            seedContext.Add(firstBook);
            seedContext.Add(secondBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.GetBookCopiesByBookIdAsync(firstBook.Id);
        }

        // Assert
        Assert.Equal(2, result.Count);

        var firstBookCopy = result.Single(copy => copy.Id == seededFirstBookCopy.Id);
        var secondBookCopy = result.Single(copy => copy.Id == seededSecondBookCopy.Id);

        Assert.Equal(seededFirstBookCopy.Id, firstBookCopy.Id);
        Assert.Equal(seededFirstBookCopy.InventoryCode, firstBookCopy.InventoryCode);
        Assert.Equal(seededFirstBookCopy.Condition.ToString(), firstBookCopy.Condition);

        Assert.Equal(seededSecondBookCopy.Id, secondBookCopy.Id);
        Assert.Equal(seededSecondBookCopy.InventoryCode, secondBookCopy.InventoryCode);
        Assert.Equal(seededSecondBookCopy.Condition.ToString(), secondBookCopy.Condition);
    }

    [Fact]
    public async Task GetBookCopiesByBookIdAsync_WhenBookDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        const int missingBookId = 999;

        IReadOnlyList<BookCopyDto> result;
    
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.GetBookCopiesByBookIdAsync(missingBookId);
        }
    
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookCopyByIdAsync_WhenBookCopyExists_ReturnsBookCopy()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        BookCopyDto? result;
    
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
                Price = 34.99m 
            };
        var seededFirstBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-001",
                Condition = BookCopyCondition.Good
            };
        var seededSecondBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-002",
                Condition = BookCopyCondition.Good
            };
    
        firstBook.Copies.Add(seededFirstBookCopy);
        firstBook.Copies.Add(seededSecondBookCopy);

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(firstBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.GetBookCopyByIdAsync(seededFirstBookCopy.Id);
        }
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(seededFirstBookCopy.Id, result.Id);
        Assert.Equal(seededFirstBookCopy.InventoryCode, result.InventoryCode);
        Assert.Equal(seededFirstBookCopy.Condition.ToString(), result.Condition);
        Assert.Equal(seededFirstBookCopy.AcquiredAt, result.AcquiredAt);
        Assert.Equal(seededFirstBookCopy.RetiredAt, result.RetiredAt);
    }

    [Fact]
    public async Task GetBookCopyByIdAsync_WhenBookCopyDoesNotExist_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        const int missingBookCopyId = 999;

        BookCopyDto? result;
    
        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.GetBookCopyByIdAsync(missingBookCopyId);
        }
    
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenBookExistsAndValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        CreateBookCopyResult result;
        
        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var seededBook = new Book
            {
                Name = "Project Hail Mary",
                Author = "Andy Weir",
                GenreId = seededGenre.Id,
                Price = 34.99m
            };
        var request = new CreateBookCopyDto
            (
                "COPY-1-001",
                BookCopyCondition.Good.ToString()
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.CreateBookCopyAsync(seededBook.Id, request);
        }

        // Assert
        await using var verificationContext = _fixture.CreateDbContext();
        
        Assert.Equal(CreateBookCopyStatus.Created, result.Status);
        Assert.NotNull(result.BookCopy);
        
        var createdBookCopy = await verificationContext.BookCopies.SingleAsync(copy => copy.Id == result.BookCopy.Id);

        Assert.Equal(request.InventoryCode, result.BookCopy.InventoryCode);
        Assert.Equal(request.Condition, result.BookCopy.Condition);

        Assert.Equal(seededBook.Id, createdBookCopy.BookId);
        Assert.Equal(request.InventoryCode, createdBookCopy.InventoryCode);
        Assert.Equal(request.Condition, createdBookCopy.Condition.ToString());
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenBookDoesNotExistAndValidRequest_ReturnsBookNotFoundDoesNotSaveBookCopy()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        CreateBookCopyResult result;

        const int missingBookId = 999;
        
        var request = new CreateBookCopyDto
            (
                "COPY-1-001",
                BookCopyCondition.Good.ToString()
            );

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.CreateBookCopyAsync(missingBookId, request);
        }

        // Assert
        await using var verificationContext = _fixture.CreateDbContext();
        
        Assert.Equal(CreateBookCopyStatus.BookNotFound, result.Status);
        Assert.Null(result.BookCopy);
        
        var createdBookCopy = await verificationContext.BookCopies.AnyAsync();

        Assert.False(createdBookCopy);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenBookExistsAndDuplicateInventoryCode_ReturnsDuplicateInventoryCodeDoesNotSaveBookCopy()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        CreateBookCopyResult result;

        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var seededBook = new Book
            {
                Name = "Project Hail Mary",
                Author = "Andy Weir",
                GenreId = seededGenre.Id,
                Price = 34.99m
            };
        var seededBookCopy = new BookCopy
            {
                InventoryCode = "COPY-1-001",
                Condition = BookCopyCondition.Good
            };
        var request = new CreateBookCopyDto
            (
                "COPY-1-001",
                BookCopyCondition.Good.ToString()
            );

        seededBook.Copies.Add(seededBookCopy);

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.CreateBookCopyAsync(seededBook.Id, request);
        }

        await using var verificationContext = _fixture.CreateDbContext();

        var savedCopies = await verificationContext.BookCopies
            .Where(copy => copy.Id == seededBookCopy.Id)
            .ToListAsync();
    
        // Assert
        Assert.Equal(CreateBookCopyStatus.DuplicateInventoryCode, result.Status);
        Assert.Null(result.BookCopy);
        Assert.Single(savedCopies);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenBookExistsAndInvalidCondition_ReturnsInvalidConditionDoesNotSaveBookCopy()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        CreateBookCopyResult result;

        var seededGenre = new Genre
            {
                Id = 1,
                Name = "Science Fiction"
            };
        var seededBook = new Book
            {
                Name = "Project Hail Mary",
                Author = "Andy Weir",
                GenreId = seededGenre.Id,
                Price = 34.99m
            };
        var request = new CreateBookCopyDto
            (
                "COPY-1-001",
                "InvalidCondition"
            );

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(seededGenre);
            seedContext.Add(seededBook);

            await seedContext.SaveChangesAsync();
        }

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new BookCopyService(serviceContext);

            result = await service.CreateBookCopyAsync(seededBook.Id, request);
        }

        // Assert
        await using var verificationContext = _fixture.CreateDbContext();

        var createdBookCopy = await verificationContext.BookCopies.AnyAsync();
           
        Assert.Equal(CreateBookCopyStatus.InvalidCondition, result.Status);
        Assert.Null(result.BookCopy);
        Assert.False(createdBookCopy);
    }

}
