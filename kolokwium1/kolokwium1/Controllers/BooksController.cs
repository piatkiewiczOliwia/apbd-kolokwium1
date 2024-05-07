using kolokwium1.Models.DTOs;
using kolokwium1.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace kolokwium1.Controllers;

[ApiController]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookWithGenres(int id)
    {
        if (!await _bookRepository.DoesBookExist(id))
            return NotFound($"Book with given ID - {id} doesn't exist");
        
        var bookEntry = await _bookRepository.GetBookWithGenres(id);

        return Ok(bookEntry);
    }

    [HttpPost]
    public async Task<IActionResult> InsertNewBook(BookDTO bookDto)
    {
        var newBook = await _bookRepository.InsertNewBook(bookDto);
        return Ok(newBook);
    }
    
    [HttpPost]
    public async Task<IActionResult> InsertNewBookWithGenres(NewBookWithGenresDTO newBookWithGenresDto)
    {
        //var newBookWithGenres = await _bookRepository.InsertNewBookWithGenres(newBookWithGenresDto);
        return Created(Request.Path.Value ?? "api/books", newBookWithGenresDto);
    }
    
    
}