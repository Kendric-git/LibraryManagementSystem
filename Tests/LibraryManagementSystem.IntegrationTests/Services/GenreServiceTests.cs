using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.IntegrationTests.Infrastructure;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.IntegrationTests.Services;

public sealed class GenreServiceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public GenreServiceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllGenresAsync_WhenGenresExist_ReturnsGenreDtos()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<GenreDto> result;

        var firstGenre = new Genre
        {
            Id = 1,
            Name = "Science Fiction"
        };

        var secondGenre = new Genre
        {
            Id = 2,
            Name = "Romance"
        };

        await using (var seedContext = _fixture.CreateDbContext())
        {
            seedContext.Add(firstGenre);
            seedContext.Add(secondGenre);

            await seedContext.SaveChangesAsync();
        }

        // Act

        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new GenreService(serviceContext);

            result = await service.GetAllGenresAsync();
        }


        // Assert
        Assert.Equal(2, result.Count);

        var firstGenreDto = result.Single(genre => genre.Id == firstGenre.Id);
        var secondGenreDto = result.Single(genre => genre.Id == secondGenre.Id);

        Assert.Equal(firstGenre.Id, firstGenreDto.Id);
        Assert.Equal(firstGenre.Name, firstGenreDto.Name);

        Assert.Equal(secondGenre.Id, secondGenreDto.Id);
        Assert.Equal(secondGenre.Name, secondGenreDto.Name);
    }

    [Fact]
    public async Task GetAllGenresAsync_WhenGenresDoNotExist_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        IReadOnlyList<GenreDto> result;

        // Act
        await using (var serviceContext = _fixture.CreateDbContext())
        {
            var service = new GenreService(serviceContext);

            result = await service.GetAllGenresAsync();
        }

        // Assert
        Assert.Empty(result);
    }
}
