using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestSecurityMiddlewareExtension
{
    [Fact]
    public async Task SecurityMiddleware_Should_Return_ResponseHeaders()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddHealthChecks();
                        services.AddControllers();
                        services.AddCors();
                        services.AddMemoryCache();
                        services.AddHttpContextAccessor();
                        services.AddRouting();
                        services.AddHsts(configureOptions =>
                        {
                            configureOptions.Preload = true;
                            configureOptions.IncludeSubDomains = true;
                            configureOptions.MaxAge = TimeSpan.FromDays(365);
                        });
                    })
                    .Configure(app =>
                    {
                        app.UseHttpHeaders();
                        app.UseExceptionHandler("/api/Error/api");
                        app.UseHsts();
                        app.UseHttpsRedirection();
                        app.UseStaticFiles();
                        app.UseFileServer(new FileServerOptions
                        {
                            EnableDirectoryBrowsing = false,
                            EnableDefaultFiles = true,
                            DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
                        });
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers().RequireAuthorization();
                            endpoints.MapFallbackToFile("/index.html");
                            endpoints.MapHealthChecks("/health");
                        });
                    });
            })
            .StartAsync();

        var server = host.GetTestServer();
        server.BaseAddress = new Uri("https://lost.com/health");

        var context = await server.SendAsync(c => c.Request.Method = HttpMethods.Post);

        Assert.True(context.RequestAborted.CanBeCanceled);
        Assert.Equal(HttpProtocol.Http11, context.Request.Protocol);
        Assert.Equal("POST", context.Request.Method);
        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("lost.com", context.Request.Host.Value);
        Assert.Equal("/health", context.Request.PathBase.Value);
        Assert.NotNull(context.Request.Body);
        Assert.NotNull(context.Request.Headers);
        Assert.NotNull(context.Response.Headers);
        Assert.NotNull(context.Response.Body);
        Assert.Equal(405, context.Response.StatusCode);

        Assert.Equal("", context.Response.Headers.Server.ToString());
        Assert.Equal("", context.Response.Headers.XPoweredBy.ToString());
        Assert.Equal("no-cache, no-store, must-revalidate", context.Response.Headers.CacheControl);
        Assert.Equal("script-src 'self' 'unsafe-inline'; worker-src 'self' blob:; object-src 'none'; frame-ancestors 'self';", context.Response.Headers.ContentSecurityPolicy);
        Assert.StartsWith("accelerometer", context.Response.Headers["Permissions-Policy"].ToString(), StringComparison.Ordinal);
        Assert.Equal("no-referrer", context.Response.Headers["Referrer-Policy"].ToString());
        Assert.Equal("max-age=31536000; includeSubDomains; preload", context.Response.Headers.StrictTransportSecurity);
        Assert.Equal("nosniff", context.Response.Headers.XContentTypeOptions);
        Assert.Equal("SAMEORIGIN", context.Response.Headers.XFrameOptions);
        Assert.Equal("IE=Edge,chrome=1", context.Response.Headers.XUACompatible);
        Assert.Equal("1; mode=block", context.Response.Headers.XXSSProtection);
    }
}
