using LibraryManagementSystem.Services;
using LibraryManagementSystem.Data;
using LibraryManagementSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddLibrarySystemDb();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookCopyService, BookCopyService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MigrateDb();

app.Run();
