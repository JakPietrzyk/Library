using BooksLibrary.Model;
using BooksLibrary.Dtos;
using BooksLibrary.Services;
using System;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BooksLibrary.Loggers;

namespace BooksLibrary.Controllers
{

    [Route("api/library")]
    [ServiceFilter(typeof(LoggerFilterAttribbute))]
    [ApiController]
    public class MyLibraryController: ControllerBase
    {
        private readonly ILibraryService _libraryService;
        public MyLibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var booksDtos = await _libraryService.GetAll(HttpContext);
            
            return Ok(booksDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> Get([FromRoute] int id)
        {
            var book = await _libraryService.GetById(id, HttpContext);

            return Ok(book);
        }
        [HttpGet("book/{title}")]
        public ActionResult<BookDto> Get([FromRoute]string title)
        {
            var book = _libraryService.GetByTitle(title, HttpContext);

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook([FromBody] CreateBookDto dto)
        {           
            var id = await _libraryService.Create(dto, HttpContext);

            return Created($"/api/library/{id}", null);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _libraryService.Delete(id, HttpContext);
            
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromRoute]int id, [FromBody]UpdateBookDto dto)
        {
            await _libraryService.Update(id, dto, HttpContext);

            return Ok();
        }
    }
}

