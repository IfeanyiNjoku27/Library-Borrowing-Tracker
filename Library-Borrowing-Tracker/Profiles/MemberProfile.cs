using AutoMapper;
using Library_Borrowing_Tracker.Models;
using Library_Borrowing_Tracker.DTO.MemberDTO;

namespace Library_Borrowing_Tracker.Profile
{
    public class MemberProfile : AutoMapper.Profile
    {
        public MemberProfile()
        {
            CreateMap<Member, MemberReadDto>();
            CreateMap<MemberCreateDto, Member>();

            CreateMap<MemberUpdateDto, Member>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
