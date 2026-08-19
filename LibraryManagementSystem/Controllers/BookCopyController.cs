using LibraryManagementSystem.Dtos;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCopyController : ControllerBase
    {
        private readonly IBookCopyService _bookCopyService;

        public BookCopyController(IBookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }

        [HttpGet("book/{bookId:int}")]
        public async Task<ActionResult<IEnumerable<BookCopyDto>>> GetBookCopiesByBookIdAsync(int bookId, CancellationToken cancellationToken)
        {
            var bookCopies = await _bookCopyService.GetBookCopiesByBookIdAsync(bookId, cancellationToken);

            return Ok(bookCopies);
        }

        [HttpGet("{copyId:int}")]
        public async Task<ActionResult<BookCopyDto>> GetBookCopyById(int copyId, CancellationToken cancellationToken)
        {
            var bookCopy = await _bookCopyService.GetBookCopyByIdAsync(copyId, cancellationToken);

            if (bookCopy is null)
            {
                return NotFound();
            }

            return Ok(bookCopy);
        }

        [HttpPost("book/{bookId:int}")]
        public async Task<ActionResult<BookCopyDto>> CreateBookCopyAsync(int bookId, [FromBody] CreateBookCopyDto newCopy, CancellationToken cancellationToken)
        {
            var result = await _bookCopyService.CreateBookCopyAsync(bookId, newCopy, cancellationToken);

            if (result.Status is CreateBookCopyStatus.BookNotFound)
            {
                ModelState.AddModelError(
                    nameof(bookId),
                    $"Book with ID:{bookId} does not exist."
                );

                return ValidationProblem(ModelState);
            }

            if (result.Status is CreateBookCopyStatus.DuplicateInventoryCode)
            {
                ModelState.AddModelError(
                    nameof(newCopy.InventoryCode),
                    $"InventoryCode: {newCopy.InventoryCode} already exists."
                );

                return ValidationProblem(ModelState);
            }

            if (result.Status is CreateBookCopyStatus.InvalidCondition)
            {
                ModelState.AddModelError(
                    nameof(newCopy.Condition),
                    $"The condition {newCopy.Condition} is invalid."
                );

                return ValidationProblem(ModelState);
            }

            var createdCopy = result.BookCopy;

            return CreatedAtAction(nameof(GetBookCopyById), new { copyId = createdCopy?.Id }, createdCopy);
        }
    }
}
