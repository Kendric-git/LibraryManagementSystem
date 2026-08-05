using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibrarySystemContext>();
        dbContext.Database.Migrate();
    }

    public static void AddLibrarySystemDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("LibrarySystem");
        builder.Services.AddNpgsql<LibrarySystemContext>
        (
            connString,
            optionsAction: options => options.UseSeeding((context, _) => 
            {
                var genres = new[]
                {
                    new Genre { Id = 1, Name = "Science Fiction" },
                    new Genre { Id = 2, Name = "Nonfiction" },
                    new Genre { Id = 3, Name = "Classic" },
                    new Genre { Id = 4, Name = "Mystery" },
                    new Genre { Id = 5, Name = "Fantasy" },
                    new Genre { Id = 6, Name = "Romance" }
                };

                foreach (var genre in genres)
                {
                    var existingGenre = context.Set<Genre>()
                        .FirstOrDefault(g => g.Id == genre.Id);

                    if (existingGenre is null)
                    {
                        context.Set<Genre>().Add(genre);
                    }
                    else
                    {
                        existingGenre.Name = genre.Name;
                    }
                }

                context.SaveChanges();
            })
        );
    }
}
