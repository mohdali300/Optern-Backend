
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.WorkSpace;
using Optern.Application.DTOs.Task;
using Task = Optern.Domain.Entities.Task;
using Optern.Application.DTOs.Sprint;
using Optern.Application.DTOs.RoomUser;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.TaskActivity;

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


			CreateMap<AddToFavoriteDTO, FavoritePosts>();


		   CreateMap<Post, PostWithDetailsDTO>()
		  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
		  .ForMember(dest => dest.ReactCount, opt => opt.MapFrom(src => src.Reacts.Count))
		  .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

			//post
			CreateMap<Post, PostDTO>()
			.ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.Creator.ProfilePicture))
			.ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
			.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name).ToList()));

			CreateMap<Post, PostWithDetailsDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName));

			//comment
			CreateMap<Comment, CommentDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
			.ForMember(dest => dest.Reacts, opt => opt.MapFrom(src => src.CommentReacts));


			//react
			CreateMap<Reacts, ReactDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            #endregion

            #region Room

            // Map Room To RoomDTO
            CreateMap<Room, CreateRoomDTO>()
              .ForMember(dest => dest.NumberOfParticipants,
               opt => opt.MapFrom(src => src.UserRooms.Count));

			//CreateMap<Room, ResponseRoomDTO>();
			CreateMap<Room, ResponseRoomDTO>();

            CreateMap<WorkSpace, WorkSpaceDTO>();

			CreateMap<Sprint, SprintResponseDTO>();

            CreateMap<AddTaskDTO, Task>()
             .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore()) 
             .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.DueDate));

            CreateMap<Task, TaskResponseDTO>()
           .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
           .ForMember(dest => dest.AssignedUsers, opt => opt.MapFrom(src => src.AssignedTasks
               .Where(at => at.User != null)
               .Select(at => new AssignedUserDTO
               {
                   UserId = at.UserId,
                   FullName = $"{at.User.FirstName} {at.User.LastName}".Trim(),
                   ProfilePicture = at.User.ProfilePicture
               }).ToList()))
            .ForMember(dest => dest.WorkspaceId, opt => opt.MapFrom(src => src.Sprint != null && src.Sprint.WorkSpace != null ? src.Sprint.WorkSpace.Id : 0))
            .ForMember(dest => dest.WorkspaceName, opt => opt.MapFrom(src => src.Sprint != null && src.Sprint.WorkSpace != null ? src.Sprint.WorkSpace.Title : string.Empty))
            .ForMember(dest => dest.SprintId, opt => opt.MapFrom(src => src.Sprint != null ? src.Sprint.Id : 0))
            .ForMember(dest => dest.SprintName, opt => opt.MapFrom(src => src.Sprint != null ? src.Sprint.Title : string.Empty));

            CreateMap<EditTaskDTO, Task>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId))
        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
        .ForMember(dest => dest.EndDate, opt => opt.Ignore()) 
        .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore());

            CreateMap<TaskActivity, TaskActivityDTO>()
         .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FirstName + " " + src.Creator.LastName : string.Empty))
         .ForMember(dest => dest.CreatorProfilePicture, opt => opt.MapFrom(src => src.Creator != null && src.Creator.ProfilePicture != null ? src.Creator.ProfilePicture : string.Empty));

            CreateMap<Task, TaskDTO>()
                .ForMember(dest => dest.AssignedUsers, opt => opt.MapFrom(src => src.AssignedTasks.Select(ut => new AssignedUserDTO
                {
                    UserId = ut.User.Id,
                    FullName = $"{ut.User.FirstName} {ut.User.LastName}".Trim(),
                    ProfilePicture = ut.User.ProfilePicture
                }).ToList()))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.AssignedTasks
                    .SelectMany(ut => ut.AttachmentUrlsList.Select(att => new AttachmentDTO
                    {
                        Url = att,
                        Uploader = new AssignedUserDTO
                        {
                            UserId = ut.User.Id,
                            FullName = $"{ut.User.FirstName} {ut.User.LastName}".Trim(),
                            ProfilePicture = ut.User.ProfilePicture
                        },
                         AttachmentDate = ut.Attachmentdate
                    })).ToList()))
                .ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.Activities.Select(a => new TaskActivityDTO
                {
                    Id = a.Id,
                    TaskId = a.TaskId,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    CreatorId = a.CreatorId,
                    CouldDelete = a.CouldDelete,
                    CreatorName = a.Creator != null ? a.Creator.FirstName + " " + a.Creator.LastName : string.Empty,
                    CreatorProfilePicture = a.Creator != null && a.Creator.ProfilePicture != null ? a.Creator.ProfilePicture : string.Empty
                }).ToList()));


            //RoomUser
            CreateMap<UserRoom, RoomUserDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.IsAdmin ? "Leader" : "Collaborator"));

            CreateMap<Skills, SkillDTO>();

            #endregion


        }
    }
}
