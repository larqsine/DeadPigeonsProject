using System.Net.Http.Headers;

namespace Tests;

public class MyTests_s : ApiTestBase
{
    [Fact]
    public async Task MyTest()
    {
        var client =  CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "token");
            
            var response = await client.GetAsync("/api/my");
        response.Content.ReadAsStringAsync();
    }
}