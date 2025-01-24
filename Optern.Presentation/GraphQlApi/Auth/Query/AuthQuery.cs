namespace Optern.Presentation.GraphQlApi.Auth.Query
{
	[ExtendObjectType("Query")]
	public class AuthQuery
	{
			[GraphQLDescription("Test")]
			public string Test() => "Test";
	}
}
