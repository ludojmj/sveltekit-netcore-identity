using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Server.UnitTest;

public class TestProgram
{
    private readonly IConfiguration _conf = new ConfigurationBuilder()
        .AddInMemoryCollection([new("Environment", "Production")])
        .Build();

    [Fact]
    public async Task RequestOnServer_ShouldReturn_Ok()
    {
        // Arrange
        await using var app = new WebApplicationFactory<Program>().WithWebHostBuilder(b => b.UseConfiguration(_conf));
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync("/index.html");
        var data = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<body>", data);
    }

    [Fact]
    public void AllDependencies_ShouldBe_Fulfilled()
    {
        // Arrange
        IServiceCollection? serviceList = null;
        var exceptions = new List<Exception>();
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(x =>
        {
            x.UseConfiguration(_conf);
            x.ConfigureServices(services => serviceList = services);
        });

        // Act
        using (factory.Services.CreateScope())
        {
            foreach (var serviceDescriptor in serviceList!)
            {
                var serviceType = serviceDescriptor.ServiceType;
                if (!serviceType.FullName!.Contains("Server", StringComparison.Ordinal))
                {
                    continue;
                }

                var ex = Record.Exception(() => factory.Services.GetRequiredService(serviceType));
                if (ex != null)
                {
                    exceptions.Add(ex);
                }
            }
        }

        // Assert
        Assert.Empty(exceptions);
    }
}
