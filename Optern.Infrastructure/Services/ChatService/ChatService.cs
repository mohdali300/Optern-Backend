
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
                    if (room != null && user != null)
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

        #region Get chat participants
        public async Task<Response<List<ChatParticipantsDTO>>> GetChatParticipantsAsync(int chatId)
        {
            try
            {
                if (await _unitOfWork.Chats.GetByIdAsync(chatId) != null)
                {
                    var participants = await _context.ChatParticipants.Include(p => p.User)
                        .Where(p => p.ChatId == chatId)
                        .Select(p => new ChatParticipantsDTO
                        {
                            Id = p.Id,
                            UserId = p.UserId,
                            Name = $"{p.User.FirstName} {p.User.LastName}"
                        })
                        .ToListAsync();

                    if (participants.Any())
                    {
                        return Response<List<ChatParticipantsDTO>>.Success(participants);
                    }
                    return Response<List<ChatParticipantsDTO>>.Success(new List<ChatParticipantsDTO>(), "There is no participants in this chat.");
                }
                return Response<List<ChatParticipantsDTO>>.Failure(new List<ChatParticipantsDTO>(), "This Chat not found.", 404);
            }
            catch (Exception ex)
            {
                return Response<List<ChatParticipantsDTO>>.Failure(new List<ChatParticipantsDTO>(), $"Server error: {ex.Message}", 500);
            }

        } 
        #endregion
    }
}
