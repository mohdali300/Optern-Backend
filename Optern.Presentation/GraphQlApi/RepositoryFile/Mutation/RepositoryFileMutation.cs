

[ExtendObjectType("Mutation")]
    public class RepositoryFileMutation
    {
            [GraphQLDescription("Upload File To Repository")]
            public async Task<Response<RepositoryFileResponseDTO>> UploadFile([Service] IRepositoryFileService _repositoryFileService,RepositoryFileDTO model,IFile file)=>
                    await _repositoryFileService.UploadFile(model,file);

    [GraphQLDescription("Delete File From Repository")]
    public async Task<Response<bool>> DeleteFile([Service] IRepositoryFileService _repositoryFileService,int repositoryFileID) =>
                 await _repositoryFileService.DeleteRepositoryFile(repositoryFileID);
}

