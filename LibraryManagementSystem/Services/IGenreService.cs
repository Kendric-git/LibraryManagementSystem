
using LibraryManagementSystem.Dtos;

namespace LibraryManagementSystem.Services;

public interface IGenreService
{
    Task<IReadOnlyList<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken = default);
}
