using LibraryManagementSystem.Controllers;
using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryManagementSystem.UnitTests.Controllers;

public class BookControllerTests
{
    private readonly Mock<IBookService> _bookService = new();
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _controller = new BookController(_bookService.Object);
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

    private static CreateBookDto CreateBookRequest()
    {
        return new CreateBookDto
        (
            "Project Hail Mary",
            "Andy Weir",
            1,
            34.99m,
            5
        );
    }

    private static UpdateBookDto CreateUpdateRequest()
    {
        return new UpdateBookDto
        (
            "Project Hail Mary",
            "Andy Weir",
            1,
            29.99m,
            7
        );
    }

    [Fact]
    public async Task GetAllBooks_WhenCalled_ReturnsOk()
    {
        // Arrange
        IReadOnlyList<BookSummaryDto> expectedBooks = [
        new BookSummaryDto
        (
            1,
            "Project Hail Mary",
            "Andy Weir",
            "Science Fiction",
            34.99m,
            5
        ),
        new BookSummaryDto
        (
            2,
            "The Great Gatsby",
            "F. Scott Fitzgerald",
            "Classic",
            19.99m,
            3
        )];

        _bookService
        .Setup(service => service.GetAllBooksAsync(
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedBooks);


        // Act
        var result = await _controller.GetBooksAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedBooks = Assert.IsAssignableFrom<IReadOnlyList<BookSummaryDto>>(okResult.Value);

        Assert.Equal(expectedBooks, returnedBooks);
    }

    [Fact]
    public async Task GetBookById_WhenBookExists_ReturnsOk()
    {
        // Arrange
        var expectedBook = CreateBookDetails();

        _bookService
        .Setup(service => service.GetBookByIdAsync(
            expectedBook.Id,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedBook);

        // Act
        var result = await _controller.GetBookById(
            expectedBook.Id,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBook = Assert.IsType<BookDetailsDto>(okResult.Value);

        Assert.Equal(expectedBook, returnedBook);
    }

    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _bookService
            .Setup(service => service.GetBookByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookDetailsDto?)null);

        // Act
        var result = await _controller.GetBookById(
            999,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_WhenCreated_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = CreateBookRequest();
        var createdBook = CreateBookDetails();

        var serviceResult = new CreateBookResult(
            CreateBookStatus.Created, createdBook);

        _bookService
        .Setup(service => service.CreateBookAsync(
            request, 
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(serviceResult);  

        // Act
        var result = await _controller.CreateBookAsync(
            request, 
            CancellationToken.None);
    
        // Assert
        var createdResult = 
        Assert.IsType<CreatedAtActionResult>(result.Result);

        Assert.Equal(nameof(BookController.GetBookById), 
        createdResult.ActionName);

        Assert.Equal(createdBook.Id, createdResult.RouteValues!["id"]);
        Assert.Equal(createdBook, createdResult.Value);
    }

    [Fact]
    public async Task CreateBook_WhenGenreMissing_ReturnsValidationProblem()
    {
        // Arrange
        var request = CreateBookRequest();

        var serviceResult = new CreateBookResult(
            CreateBookStatus.GenreNotFound,
            null);

        _bookService
        .Setup(service => service.CreateBookAsync(
            request,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(serviceResult);
        
        // Act
        var result = await _controller.CreateBookAsync(
            request,
            CancellationToken.None);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        
        var details = Assert.IsType<ValidationProblemDetails>(problemResult.Value);

        Assert.Contains(
            nameof(request.GenreId),
            details.Errors.Keys);
        Assert.Contains(
            $"Genre with ID {request.GenreId} does not exist.",
            details.Errors[nameof(request.GenreId)]
        );
    }

    [Fact]
    public async Task UpdateBook_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        var request = CreateUpdateRequest();

        _bookService
        .Setup(service => service.UpdateBookByIdAsync(
            1,
            request,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(UpdateBookStatus.Updated);

        // Act
        var result = await _controller.UpdateBookByIdAsync(
            1, 
            request, 
            CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateBook_WhenBookMissing_ReturnsNotFound()
    {
        // Arrange
        var request = CreateUpdateRequest();

        _bookService
        .Setup(service => service.UpdateBookByIdAsync(
            999,
            request,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(UpdateBookStatus.BookNotFound);

        // Act
        var result = await _controller.UpdateBookByIdAsync(
            999, 
            request, 
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateBook_WhenGenreMissing_ReturnsValidationProblem()
    {
        // Arrange
        var request = CreateUpdateRequest();

        _bookService
        .Setup(service => service.UpdateBookByIdAsync(
            1,
            request,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(UpdateBookStatus.GenreNotFound);

        // Act
        var result = await _controller.UpdateBookByIdAsync(
            1,
            request,
            CancellationToken.None);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);

        var details = Assert.IsType<ValidationProblemDetails>(problemResult.Value);

        Assert.Contains(
            nameof(request.GenreId),
            details.Errors.Keys);
        Assert.Contains(
            $"Genre with ID {request.GenreId} does not exist.",
            details.Errors[nameof(request.GenreId)]
        );
    }

    [Fact]
    public async Task DeleteBook_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _bookService
        .Setup(service => service.DeleteBookByIdAsync(
            1, 
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteBookAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBook_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        _bookService
        .Setup(service => service.DeleteBookByIdAsync(
            1, 
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteBookAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}