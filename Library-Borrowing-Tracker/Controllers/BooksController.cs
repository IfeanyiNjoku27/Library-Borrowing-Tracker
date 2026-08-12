using AutoMapper;
using Library_Borrowing_Tracker.DTO;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IRepository<Book> _repository;
        private readonly IMapper _mapper;

        public BooksController(IRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadDto>>> GetAll()
        {
            var books = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<BookReadDto>>(books);
            return Ok(dtos);
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadDto>> GetById(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            var dto = _mapper.Map<BookReadDto>(book);
            return Ok(dto);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookReadDto>> Create(BookCreateDto createDto)
        {
            var book = _mapper.Map<Book>(createDto);

            await _repository.AddAsync(book);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<BookReadDto>(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, readDto);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookUpdateDto updateDto)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            _mapper.Map(updateDto, book);
            _repository.Update(book);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/books/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(int id, [FromBody] JsonPatchDocument<Book> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            patchDoc.ApplyTo(book, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _repository.Update(book);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();

            _repository.Delete(book);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}