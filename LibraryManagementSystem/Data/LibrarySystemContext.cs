using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class LibrarySystemContext(DbContextOptions<LibrarySystemContext> options) 
: DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(copy => copy.Id);

            entity.Property(copy => copy.InventoryCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(copy => copy.InventoryCode)
                .IsUnique();

            entity.Property(copy => copy.Condition)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(copy => copy.Book)
                .WithMany(book => book.Copies)
                .HasForeignKey(copy => copy.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

