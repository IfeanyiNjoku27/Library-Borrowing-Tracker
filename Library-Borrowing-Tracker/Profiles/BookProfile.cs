using AutoMapper;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.DTO.BookDTO;

namespace Library_Borrowing_Tracker.Profile
{
    public class BookProfile : AutoMapper.Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookReadDto>();
            CreateMap<BookCreateDto, Book>();

            CreateMap<BookUpdateDto, Book>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}