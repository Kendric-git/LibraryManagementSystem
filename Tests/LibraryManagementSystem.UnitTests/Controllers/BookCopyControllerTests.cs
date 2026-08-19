using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryManagementSystem.UnitTests.Controllers;

public class BookCopyControllerTests
{
    private readonly Mock<IBookCopyService> _bookCopyService = new();
    private readonly BookCopyController _controller;

    public BookCopyControllerTests()
    {
        _controller = new BookCopyController(_bookCopyService.Object);
    }

    private static BookDetailsDto CreateBookDetails()
    {
        return new BookDetailsDto
        (
            1,
            "Project Hail Mary",
            "Andy Weir",
            new GenreDto(1, "Science Fiction"),
            34.99m,
            5
        );
    }

    [Fact]
    public async Task GetBookCopiesByBookIdAsync_WhenBookExists_ReturnsOk()
    {
        // Arrange
        var book = CreateBookDetails();

        IReadOnlyList<BookCopyDto> expectedBookCopies = 
        [
            new BookCopyDto
            (
                1,
                "COPY-1-001",
                BookCopyCondition.Good.ToString(),
                DateTime.UtcNow,
                null
            ),
            new BookCopyDto
            (
                2,
                "COPY-1-002",
                BookCopyCondition.Good.ToString(),
                DateTime.UtcNow,
                null
            )
        ];


        _bookCopyService
            .Setup(service => service.GetBookCopiesByBookIdAsync(
                book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBookCopies);

        // Act
        var result = await _controller.GetBookCopiesByBookIdAsync(book.Id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedBookCopies = Assert.IsAssignableFrom<IReadOnlyList<BookCopyDto>>(okResult.Value);

        Assert.Equal(expectedBookCopies, returnedBookCopies);
    }

    [Fact]
    public async Task GetBookCopyByIdAsync_WhenBookCopyExists_ReturnsBookCopy()
    {
        // Arrange
        var expectedbookCopy = new BookCopyDto
            (
                1,
                "COPY-1-001",
                BookCopyCondition.Good.ToString(),
                DateTime.UtcNow,
                null
            );
        
        _bookCopyService
            .Setup(service => service.GetBookCopyByIdAsync(expectedbookCopy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedbookCopy);

        // Act
        var result = await _controller.GetBookCopyById(expectedbookCopy.Id, CancellationToken.None);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookCopy = Assert.IsType<BookCopyDto>(okResult.Value);

        Assert.Equal(expectedbookCopy, returnedBookCopy);
    }

    [Fact]
    public async Task GetBookCopyByIdAsync_WhenBookCopyDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _bookCopyService
            .Setup(service => service.GetBookCopyByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookCopyDto?) null);
    
        // Act
        var result = await _controller.GetBookCopyById(999, CancellationToken.None);
    
        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenBookExists_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateBookCopyDto 
        (
            "COPY-1-001",
            BookCopyCondition.Good.ToString()
        );

        var createdBook = new BookDetailsDto
        (
            1,
            "Project Hail Mary",
            "Andy Weir",
            new GenreDto(1, "Science Fiction"),
            34.99m,
            5
        );

        var createdBookCopy = new BookCopyDto 
        (
            1,
            "COPY-1-001",
            BookCopyCondition.Good.ToString(),
            DateTime.UtcNow,
            null
        );

        var serviceResult = new CreateBookCopyResult(
            CreateBookCopyStatus.Created, createdBookCopy);

        _bookCopyService
            .Setup(service => service.CreateBookCopyAsync(
                createdBook.Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);
    
        // Act
        var result = await _controller.CreateBookCopyAsync(createdBook.Id, request, CancellationToken.None);
        

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(nameof(BookCopyController.GetBookCopyById), createdResult.ActionName);
        Assert.Equal(createdBookCopy.Id, createdResult.RouteValues!["copyId"]);
        Assert.Equal(createdBookCopy, createdResult.Value);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenDuplicateInventoryCode_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CreateBookCopyDto 
        (
            "COPY-1-001",
            BookCopyCondition.Good.ToString()
        );

        var createdBook = new BookDetailsDto
        (
            1,
            "Project Hail Mary",
            "Andy Weir",
            new GenreDto(1, "Science Fiction"),
            34.99m,
            5
        );

        var serviceResult = new CreateBookCopyResult (
            CreateBookCopyStatus.DuplicateInventoryCode, 
            null);

        _bookCopyService
            .Setup(service => service.CreateBookCopyAsync(
                createdBook.Id, 
                request, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);
    
        // Act
        var result = await _controller.CreateBookCopyAsync(
            createdBook.Id, 
            request, 
            CancellationToken.None);
    
        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(problemResult.Value);

        Assert.True(problemDetails.Errors.TryGetValue("InventoryCode", out var errors));

        Assert.Contains($"InventoryCode: {request.InventoryCode} already exists.", errors);
    }

    [Fact]
    public async Task CreateBookCopyAsync_WhenInvalidCondition_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CreateBookCopyDto 
        (
            "COPY-1-001",
            "RandomCondition"
        );

        var createdBook = new BookDetailsDto
        (
            1,
            "Project Hail Mary",
            "Andy Weir",
            new GenreDto(1, "Science Fiction"),
            34.99m,
            5
        );

        var serviceResult = new CreateBookCopyResult (
            CreateBookCopyStatus.InvalidCondition, 
            null);

        _bookCopyService
            .Setup(service => service.CreateBookCopyAsync(
                createdBook.Id, 
                request, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);
    
        // Act
        var result = await _controller.CreateBookCopyAsync(
            createdBook.Id, 
            request, 
            CancellationToken.None);
    
        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(problemResult.Value);

        Assert.True(problemDetails.Errors.TryGetValue("Condition", out var errors));

        Assert.Contains($"The condition {request.Condition} is invalid.", errors);
    }
}
