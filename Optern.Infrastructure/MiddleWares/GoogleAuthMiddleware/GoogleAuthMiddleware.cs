using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using Task = System.Threading.Tasks;

public class GoogleAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClient;

    public GoogleAuthMiddleware(RequestDelegate next, IHttpClientFactory httpClient)
    {
        _next = next;
        _httpClient = httpClient;
    }

    public async Task.Task Invoke(HttpContext context)
    {
        var request = context.Request;

        if (request.Path == "/ui/graphql" && request.Query.ContainsKey("code"))
        {
            var code = request.Query["code"].ToString();
            var jsonPayload = JsonConvert.SerializeObject(new
            {
                query = $"query googleCallbackAsync($code: String!) {{ googleCallbackAsync(code: $code) {{ data {{ userId name profilePicture roles isAuthenticated token }} message statusCode }} }}",
                variables = new { code }
            });
            var client = _httpClient.CreateClient();
            var response = await client.PostAsync("http://localhost:5217/ui/graphql",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            var responseData = await response.Content.ReadAsStringAsync();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseData);
            return;
        }

        await _next(context);
    }
}