namespace LibraryManagementSystem.Models;

public class BookCopy
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public required string InventoryCode { get; set; }
    public BookCopyCondition Condition { get; set; } = BookCopyCondition.Good;
    public DateTime AcquiredAt { get; set ;} = DateTime.UtcNow;
    public DateTime? RetiredAt { get; set; } 

}

public enum BookCopyCondition
{
    New,
    Good,
    Fair,
    Damaged,
    Lost
}