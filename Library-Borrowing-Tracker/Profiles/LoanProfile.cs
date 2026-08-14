using AutoMapper;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.DTO.LoansDTO;

namespace Library_Borrowing_Tracker.Profiles
{
    public class LoanProfile : AutoMapper.Profile
    {
        public LoanProfile()
        {
            // Source -> Target
            CreateMap<Loans, LoanReadDto>();
            CreateMap<LoanCreateDto, Loans>();
            CreateMap<LoanUpdateDto, Loans>();
            CreateMap<Loans, LoanUpdateDto>();
        }
    }
}
