using BooksLibrary.Model;
using BooksLibrary.Dtos;
using BooksLibrary.Services;
using System;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;


namespace BooksLibrary.Controllers
{

    [Route("api/library")]
    [ApiController]
    public class MyLibraryController: ControllerBase
    {
        private readonly ILibraryService _libraryService;
        public MyLibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetAll()
        {
            var booksDtos = _libraryService.GetAll();
            
            return Ok(booksDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<BookDto> Get([FromRoute] int id)
        {
            var book = _libraryService.GetById(id);

            return Ok(book);
        }
        [HttpGet("book/{title}")]
        public ActionResult<BookDto>Get([FromRoute]string title)
        {
            var book = _libraryService.GetByTitle(title);

            return Ok(book);
        }

        [HttpPost]
        public ActionResult CreateBook([FromBody] CreateBookDto dto)
        {           
            var id = _libraryService.Create(dto);

            return Created($"/api/library/{id}", null);
        }
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _libraryService.Delete(id);

            return NoContent();
        }
        [HttpPut("{id}")]
        public ActionResult Put([FromRoute]int id, [FromBody]UpdateBookDto dto)
        {
            _libraryService.Update(id, dto);

            return Ok();
        }
    }
}

