namespace kolokwium1.Models.DTOs;

public class NewBookWithGenresDTO
{
    public int IdBook { get; set; }
    public string Title { get; set; }
    public IEnumerable<Books_GenreDTO> Genres { get; set; } = new List<Books_GenreDTO>();
}

public class Books_GenreDTO
{
    public int IdBook { get; set; }
    public int IdGenre { get; set; }
}