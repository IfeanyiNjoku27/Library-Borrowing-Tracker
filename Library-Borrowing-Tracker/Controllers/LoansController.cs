using AutoMapper;
using Library_Borrowing_Tracker.DTO.LoansDTO;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace Library_Borrowing_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IRepository<Loans> _repository;
        private readonly IMapper _mapper;

        public LoansController(IRepository<Loans> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // 1. GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanReadDto>>> GetAllLoans()
        {
            var loans = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LoanReadDto>>(loans));
        }

        // 2. GET BY ID
        [HttpGet("{id}", Name = "GetLoanById")]
        public async Task<ActionResult<LoanReadDto>> GetLoanById(int id)
        {
            var loan = await _repository.GetByIdAsync(id);
            if (loan == null) return NotFound();

            return Ok(_mapper.Map<LoanReadDto>(loan));
        }

        // 3. POST (Create)
        [HttpPost]
        public async Task<ActionResult<LoanReadDto>> CreateLoan(LoanCreateDto loanCreateDto)
        {
            var loanModel = _mapper.Map<Loans>(loanCreateDto);
            await _repository.AddAsync(loanModel);
            await _repository.SaveChangesAsync();

            var loanReadDto = _mapper.Map<LoanReadDto>(loanModel);

            return CreatedAtRoute(nameof(GetLoanById), new { Id = loanReadDto.Id }, loanReadDto);
        }

        // 4. PUT (Replace)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLoan(int id, LoanUpdateDto loanUpdateDto)
        {
            var loanModelFromRepo = await _repository.GetByIdAsync(id);
            if (loanModelFromRepo == null) return NotFound();

            _mapper.Map(loanUpdateDto, loanModelFromRepo);
            _repository.Update(loanModelFromRepo);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // 5. PATCH (Partial Update)
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialLoanUpdate(int id, JsonPatchDocument<LoanUpdateDto> patchDoc)
        {
            var loanModelFromRepo = await _repository.GetByIdAsync(id);
            if (loanModelFromRepo == null) return NotFound();

            var loanToPatch = _mapper.Map<LoanUpdateDto>(loanModelFromRepo);
            patchDoc.ApplyTo(loanToPatch, ModelState);

            if (!TryValidateModel(loanToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(loanToPatch, loanModelFromRepo);
            _repository.Update(loanModelFromRepo);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // 6. DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLoan(int id)
        {
            var loanModelFromRepo = await _repository.GetByIdAsync(id);
            if (loanModelFromRepo == null) return NotFound();

            _repository.Delete(loanModelFromRepo);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

