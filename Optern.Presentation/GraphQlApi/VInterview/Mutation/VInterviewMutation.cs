using Optern.Application.DTOs.VInterview;
using Optern.Application.Interfaces.IVInterviewService;

namespace Optern.Presentation.GraphQlApi.VInterview.Mutation
{
    [ExtendObjectType("Mutation")]
    public class VInterviewMutation
    {
        [GraphQLDescription("Create Virtual Interview")]

        public async Task<Response<VInterviewDTO>> CreateVInterviewAsync([Service] IVInterviewService _vInterviewService, CreateVInterviewDTO dto, int questionCount, string userId)
 => await _vInterviewService.CreateVInterviewAsync(dto, questionCount, userId);

    }
}
