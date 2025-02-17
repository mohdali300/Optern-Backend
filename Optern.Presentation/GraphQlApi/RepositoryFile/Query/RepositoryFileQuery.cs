

     [ExtendObjectType("Query")]
    public class RepositoryFileQuery
    {

    [GraphQLDescription("Get Uploaded Files from Repository")]
    public async Task<Response<IEnumerable<RepositoryFileResponseDTO>>> GetUploadedFiles([Service] IRepositoryFileService _repositoryFileService, int repositoryId, RepositoryFileSortType? sortType) =>
                   await _repositoryFileService.GetUploadedFiles(repositoryId, sortType);

    [GraphQLDescription("Search For Files from Repository")]
    public async Task<Response<IEnumerable<RepositoryFileResponseDTO>>> SearchRepositoriesFiles([Service] IRepositoryFileService _repositoryFileService,int repositoryId, string Pattern) =>
                   await _repositoryFileService.Search(repositoryId, Pattern);
}

