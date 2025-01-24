using Optern.Application.Interfaces.IRoomService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Auth.Query
{
    public class AuthQuery
    {
            [GraphQLDescription("Test")]
            public string Test() => "Test";


    }
}
