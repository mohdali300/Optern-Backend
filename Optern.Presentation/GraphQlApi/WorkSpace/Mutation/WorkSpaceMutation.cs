
    [ExtendObjectType("Mutation")]
    public class WorkSpaceMutation
    {

        [GraphQLDescription ("Create New WorkSpace")]
        public async Task<Response<WorkSpaceDTO>> CreateWorkSpace([Service] IWorkSpaceService _workSpaceService, WorkSpaceDTO model)=>
          await  _workSpaceService.CreateWorkSpace(model); 
        
        [GraphQLDescription ("Update WorkSpace")]
        public async Task<Response<WorkSpaceDTO>> UpdateWorkSpace([Service] IWorkSpaceService _workSpaceService, int id,string title)=>
          await  _workSpaceService.UpdateWorkSpace(id,title);
        [GraphQLDescription("Update WorkSpace")]
        public async Task<Response<bool>> DeleteWorkSpace([Service] IWorkSpaceService _workSpaceService, int id) =>
                    await _workSpaceService.DeleteWorkSpace(id);

    }
