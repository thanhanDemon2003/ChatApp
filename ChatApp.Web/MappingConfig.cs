using AutoMapper;
using Chat.Domain.Entities;
using ChatApp.Web.Models.DTOs;

namespace ChatApp.Web
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<UserGroup, UserDTO>()
                  .ForMember(dest
                  => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                  .ForMember(dest
                  => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                  .ForMember(dest
                  => dest.FullName, opt => opt.MapFrom(src => src.User.Name))
                  .ForMember(dest
                  => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl))
                  .ForMember(dest
                  => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                  .ForMember(dess
                  => dess.Address, opt => opt.MapFrom(src => src.User.Address));
        }
    }
}
