using Optern.Application.DTOs.Sprint;
using Optern.Application.Interfaces.ISprintService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Sprint.Mutation
{
    [ExtendObjectType("Mutation")]
    public class SprintMutation
    {

        [GraphQLDescription("Add New Sprint")]
        public async Task<Response<SprintResponseDTO>> AddSprint([Service] ISprintService _sprintService, AddSprintDTO model) =>
            await _sprintService.AddSprint(model);

        [GraphQLDescription("Edit Sprint")]
        public async Task<Response<SprintResponseDTO>> EditSprint([Service] ISprintService _sprintService,int id, EditSprintDTO model) =>
            await _sprintService.EditSprint(id,model);

        [GraphQLDescription("Delete Sprint")]
        public async Task<Response<bool>> DeleteSprint([Service] ISprintService _sprintService, int id) =>
          await _sprintService.DeleteSprint(id);
    }
}
