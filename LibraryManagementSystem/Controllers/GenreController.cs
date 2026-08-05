using LibraryManagementSystem.Services;
using LibraryManagementSystem.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetAllGenresAsync(cancellationToken);

            return Ok(genres);
        }
    }
}
