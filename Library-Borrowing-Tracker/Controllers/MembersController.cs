using AutoMapper;
using Library_Borrowing_Tracker.DTO.MemberDTO;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IRepository<Member> _repository;
        private readonly IMapper _mapper;

        public MembersController(IRepository<Member> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberReadDto>>> GetAll()
        {
            var members = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<MemberReadDto>>(members);
            return Ok(dtos);
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberReadDto>> GetById(int id)
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null) return NotFound();

            var dto = _mapper.Map<MemberReadDto>(member);
            return Ok(dto);
        }

        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<MemberReadDto>> Create(MemberCreateDto createDto)
        {
            var member = _mapper.Map<Member>(createDto);

            await _repository.AddAsync(member);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<MemberReadDto>(member);
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, readDto);
        }

        // PUT: api/members/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MemberUpdateDto updateDto)
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null) return NotFound();

            _mapper.Map(updateDto, member);
            _repository.Update(member);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/members/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(int id, [FromBody] JsonPatchDocument<Member> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var member = await _repository.GetByIdAsync(id);
            if (member == null) return NotFound();

            patchDoc.ApplyTo(member, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _repository.Update(member);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null) return NotFound();

            _repository.Delete(member);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
