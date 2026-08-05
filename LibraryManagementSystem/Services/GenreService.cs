using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public sealed class GenreService(LibrarySystemContext dbContext) : IGenreService
{
    public async Task<IReadOnlyList<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Genres
            .AsNoTracking()
            .Select(genre => new GenreDto (
                genre.Id,
                genre.Name
            ))
            .ToListAsync(cancellationToken);
    }
}
