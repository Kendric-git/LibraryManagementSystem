using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryManagementSystem.UnitTests.Controllers;

public class GenreControllerTests
{
    private readonly Mock<IGenreService> _genreService = new ();
    private readonly GenreController _controller;

    public GenreControllerTests()
    {
        _controller = new GenreController(_genreService.Object);
    }

    [Fact]
    public async Task GetAllGenres_WhenCalled_ReturnsOk()
    {
        // Arrange
        IReadOnlyList<GenreDto> expectedGenres = [
            new GenreDto 
            (
                1,
                "Science Fiction"
            ),
            new GenreDto 
            (
                2,
                "Romance"
            )];

            _genreService
            .Setup(service => service.GetAllGenresAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGenres);
    
        // Act
        var result = await _controller.GetAllGenresAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedGenres = Assert.IsAssignableFrom<IReadOnlyList<GenreDto>>(okResult.Value);

        Assert.Equal(expectedGenres, returnedGenres);
    }
}
