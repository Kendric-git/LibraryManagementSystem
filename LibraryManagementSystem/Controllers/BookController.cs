using LibraryManagementSystem.Dtos;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSummaryDto>>> GetBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllBooksAsync(cancellationToken);

            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDetailsDto>> GetBookById(int id, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetBookByIdAsync(id, cancellationToken);

            if (book == null) {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDetailsDto>> CreateBookAsync([FromBody] CreateBookDto newBook, CancellationToken cancellationToken)
        {
            var result = await _bookService.CreateBookAsync(newBook, cancellationToken);

            if (result.Status == CreateBookStatus.GenreNotFound)
            {
                ModelState.AddModelError(
                    nameof(newBook.GenreId),
                    $"Genre with ID {newBook.GenreId} does not exist.");

                return ValidationProblem(ModelState);
            }

            var createdBook = result.Book!;

            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBookByIdAsync(int id, [FromBody] UpdateBookDto updatedBook, CancellationToken cancellationToken)
        {
            var result = await _bookService.UpdateBookByIdAsync(id, updatedBook, cancellationToken);

            if (result == UpdateBookStatus.BookNotFound) {
                return NotFound();
            }

            if (result == UpdateBookStatus.GenreNotFound)
            {
                ModelState.AddModelError(
                    nameof(updatedBook.GenreId),
                    $"Genre with ID {updatedBook.GenreId} does not exist.");

                return ValidationProblem(ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBookAsync(int id, CancellationToken cancellationToken)
        {
            var wasDeleted = await _bookService.DeleteBookByIdAsync(id, cancellationToken);
            
            if (!wasDeleted) {
                return NotFound();
            }

            return NoContent();
        }
    }

}
