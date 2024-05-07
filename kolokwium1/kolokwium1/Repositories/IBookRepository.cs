using kolokwium1.Models.DTOs;

namespace kolokwium1.Repositories;

public interface IBookRepository
{
    Task<BookEntryDTO> GetBookWithGenres(int bookId);
    Task<BookDTO> InsertNewBook(BookDTO bookDto);
    Task InsertNewBookWithGenres(NewBookWithGenresDTO newBookWithGenresDto);
    Task<bool> DoesBookExist(int id);
}