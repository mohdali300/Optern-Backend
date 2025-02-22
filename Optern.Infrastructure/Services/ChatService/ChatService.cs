
using MimeKit;
using Optern.Domain.Entities;
using Optern.Infrastructure.Hubs;

namespace Optern.Infrastructure.Services.ChatService
{
    public class ChatService:IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public ChatService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region Create room chat
        public async Task<Response<ChatDTO>> CreateRoomChatAsync(string creatorId, ChatType type)
        {
            try
            {
                if (!string.IsNullOrEmpty(creatorId))
                {
                    var chat = new Chat
                    {
                        Type = type,
                        CreatedDate = DateTime.UtcNow,
                        CreatorId = creatorId
                    };

                    await _unitOfWork.Chats.AddAsync(chat);
                    await _unitOfWork.SaveAsync();

                    return Response<ChatDTO>.Success(new ChatDTO { Id = chat.Id, Type = type });
                }
                return Response<ChatDTO>.Failure(new ChatDTO(), "Invalid creator Id.", 400);
            }
            catch (Exception ex)
            {
                return Response<ChatDTO>.Failure(new ChatDTO(), $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Join to room chat
        public async Task<Response<bool>> JoinToRoomChatAsync(string roomId, string participantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(roomId) && !string.IsNullOrEmpty(participantId))
                {
                    var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                    var user = await _unitOfWork.Users.GetByIdAsync(participantId);
                    if (room != null && user != null && !_context.ChatParticipants.Any(p => p.UserId == participantId && p.ChatId == room.ChatId))
                    {
                        var chatParticipant = new ChatParticipants
                        {
                            JoinedAt = DateTime.UtcNow,
                            UserId = participantId,
                            ChatId = room.ChatId
                        };

                        await _unitOfWork.ChatParticipants.AddAsync(chatParticipant);
                        await _unitOfWork.SaveAsync();

                        return Response<bool>.Success(true, $"{user.FirstName} {user.LastName} joined to room chat.", 200);
                    }
                    return Response<bool>.Failure(false, "Room or User is not existed!", 400);
                }
                return Response<bool>.Failure(false, "Invalid Room Or User Id!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Join All To Room Chat
        public async Task<Response<bool>> JoinAllToRoomChatAsync(string roomId, List<string> participantsIds)
        {
            try
            {
                if (!string.IsNullOrEmpty(roomId) && participantsIds.Count() > 0)
                {
                    var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                    if (room != null)
                    {
                        var participants = new List<ChatParticipants>();
                        foreach (var id in participantsIds)
                        {
                            var user = await _unitOfWork.Users.GetByIdAsync(id);
                            if (user != null)
                            {
                                var participant = new ChatParticipants
                                {
                                    UserId = id,
                                    ChatId = room.ChatId
                                };
                                participants.Add(participant);
                            }
                        }

                        await _unitOfWork.ChatParticipants.AddRangeAsync(participants);
                        await _unitOfWork.SaveAsync();

                        return Response<bool>.Success(true, "New participants joined to room chat.", 201);
                    }
                    return Response<bool>.Failure(false, "This Room is not existed!", 400);
                }
                return Response<bool>.Failure(false, "Invalid Room or Users IDs!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Remove from room chat
        public async Task<Response<bool>> RemoveFromRoomChatAsync(int chatId, string userId)
        {
            try
            {
                var chatParticipant = await _unitOfWork.ChatParticipants
                    .GetByExpressionAsync(p => p.ChatId == chatId && p.UserId == userId);
                if (chatParticipant != null)
                {
                    await _unitOfWork.ChatParticipants.DeleteAsync(chatParticipant);
                    await _unitOfWork.SaveAsync();

                    return Response<bool>.Success(true, "User left room chat.", 200);
                }
                return Response<bool>.Failure(false, "User already left th chat!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Get chat participants for user or chat
        public async Task<Response<List<ChatParticipants>>> GetChatParticipantsAsync(int? chatId = null, string? userId = null)
        {
            try
            {
                if ((!chatId.HasValue && string.IsNullOrEmpty(userId)) || (chatId.HasValue && !string.IsNullOrEmpty(userId)))
                    return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "Specify either ChatId or UserId, only one from them.", 400);

                List<ChatParticipants> participants = new();
                if (chatId.HasValue)
                {
                    if (await _context.Chats.FindAsync(chatId) == null)
                        return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "This chat not existed.", 400);

                    participants = await _context.ChatParticipants.Include(p => p.User)
                        .Include(p => p.Chat)
                        .Where(p => p.ChatId == chatId)
                        .ToListAsync();
                }
                else
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);
                    if (user == null)
                        return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "This user not existed.", 400);

                    participants = await _context.ChatParticipants.Include(p => p.User)
                        .Include(p => p.Chat)
                        .Where(p => p.UserId == userId)
                        .ToListAsync();
                }

                return (participants.Any()) ? Response<List<ChatParticipants>>.Success(participants)
                    : Response<List<ChatParticipants>>.Success(participants, "There is no Chat Participant yet.", 204);
            }
            catch (Exception ex)
            {
                return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), $"Server error: {ex.Message}", 500);
            }

        } 
        #endregion
    }
}
