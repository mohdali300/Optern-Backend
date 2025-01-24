
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room.RoomDTO;

namespace Optern.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            // Mapping Tag entity to TagDTO
            CreateMap<Tags, TagDTO>();

            // Mapping PostTags entity to a DTO 
            CreateMap<PostTags, TagDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name));


            //post
            CreateMap<Post, PostDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.UserName));

            //comment
            CreateMap<Comment, CommentDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            //react
            CreateMap<Reacts, ReactDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
          // Map Room To RoomDTO
            CreateMap<Room, RoomDTO>()
              .ForMember(dest => dest.NumberOfParticipants,
               opt => opt.MapFrom(src => src.UserRooms.Count));

        }
    }
}
