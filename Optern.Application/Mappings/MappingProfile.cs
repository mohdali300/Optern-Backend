
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.FavoritePosts;

namespace Optern.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Blogs

            CreateMap<Tags, TagDTO>();

            CreateMap<PostTags, TagDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name));

            CreateMap<FavoritePosts, FavouritePostsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Post.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.Post.CreatorId))
                .ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(src => src.Post.Creator.UserName))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Post.PostTags));


            CreateMap<AddToFavoriteDTO, FavoritePosts>();




            //post
            CreateMap<Post, PostDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.UserName));
            CreateMap<Post, PostWithDetailsDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.UserName));

            //comment
            CreateMap<Comment, CommentDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            //react
            CreateMap<Reacts, ReactDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            #endregion

            // Map Room To RoomDTO
            CreateMap<Room, RoomDTO>()
              .ForMember(dest => dest.NumberOfParticipants,
               opt => opt.MapFrom(src => src.UserRooms.Count));

        }
    }
}
