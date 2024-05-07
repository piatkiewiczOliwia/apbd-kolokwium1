using System.Data.SqlClient;
using kolokwium1.Controllers;
using kolokwium1.Models.DTOs;

namespace kolokwium1.Repositories;

public class BookRepository : IBookRepository
{
    private readonly string _connectionString;

    public BookRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<BookEntryDTO> GetBookWithGenres(int bookId)
    {
        await using SqlConnection con = new SqlConnection(_connectionString);
        await con.OpenAsync();

        await using SqlCommand cmd = new SqlCommand(@"SELECT books.PK AS BookId,
                                                                    books.title AS BookTitle,
                                                                    genres.nam AS BookGenre
                                                                    FROM books
                                                                    JOIN books_genres ON books_genres.FK_book = books.PK
                                                                    JOIN genres ON genres.PK = books_genres.FK_genre
                                                                    WHERE books.PK = @BookId",
            con);
        
        cmd.Parameters.AddWithValue("@BookId", bookId);
        
        var reader = await cmd.ExecuteReaderAsync();

        var bookIdOrdinal = reader.GetOrdinal("BookId");
        var bookTitleOrdinal = reader.GetOrdinal("BookTitle");
        var bookGenreOrdinal = reader.GetOrdinal("BookGenre");

        BookEntryDTO bookEntryDto = null;
        //List<string> Genres = new List<string>();

        while (await reader.ReadAsync())
        {
            bookEntryDto = new BookEntryDTO()
            {
                IdBook = reader.GetInt32(bookIdOrdinal),
                Title = reader.GetString(bookTitleOrdinal),
                Genre = reader.GetString(bookGenreOrdinal)
            };
        }

        return bookEntryDto;
    }

    public async Task<BookDTO> InsertNewBook(BookDTO bookDto)
    {
        await using SqlConnection con = new SqlConnection(_connectionString);
        await con.OpenAsync();
        await using SqlCommand cmd = new SqlCommand(@"INSERT INTO books (PK, title) VALUES (@PK, @Title)", con);
        
        cmd.Parameters.AddWithValue("@PK", bookDto.IdBook);
        cmd.Parameters.AddWithValue("@Title", bookDto.Title);

        BookDTO res = new BookDTO()
        {
            IdBook = bookDto.IdBook,
            Title = bookDto.Title
        };

        return res;
    }

    public async Task InsertNewBookWithGenres(NewBookWithGenresDTO newBookWithGenresDto)
    {
        await using SqlConnection con = new SqlConnection(_connectionString);
        await con.OpenAsync();
        await using SqlCommand cmd = new SqlCommand(@"INSERT INTO books (PK, title) VALUES (@PK, @Title)", con);
        
        cmd.Parameters.AddWithValue("@PK", newBookWithGenresDto.IdBook);
        cmd.Parameters.AddWithValue("@Title", newBookWithGenresDto.Title);
        
        var transaction = await con.BeginTransactionAsync();
        cmd.Transaction = transaction as SqlTransaction;
        
        try
        {
            var id = await cmd.ExecuteScalarAsync();
    
            foreach (var genre in newBookWithGenresDto.Genres)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO book_genres (FK_book, FK_genre) VALUES(@BookId, @GenreId)";
                cmd.Parameters.AddWithValue("@BookId", genre.IdBook);
                cmd.Parameters.AddWithValue("@AnimalId", genre.IdGenre);
                
                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DoesBookExist(int id)
    {
        await using SqlConnection con = new SqlConnection(_connectionString);
        await con.OpenAsync();

        await using SqlCommand cmd = new SqlCommand(@"SELECT 1 FROM books WHERE PK = @PK", con);
        cmd.Parameters.AddWithValue("@PK", id);

        var res = await cmd.ExecuteScalarAsync();

        return res is not null;
    }
}