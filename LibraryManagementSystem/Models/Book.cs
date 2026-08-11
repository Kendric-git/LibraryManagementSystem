namespace LibraryManagementSystem.Models;

public class Book
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Author { get; set; }
    public Genre? Genre { get; set; }
    public int GenreId { set; get; }
    public decimal Price { get; set; }
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
}
