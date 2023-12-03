namespace Server.Shared;

public static class SecurityMiddlewareExtension
{
    public static void UseHttpHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.Headers.Append("Content-Security-Policy", "script-src 'self' 'unsafe-inline'; worker-src 'self' blob:; object-src 'none'; frame-ancestors 'self';");
            context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Append("X-UA-Compatible", "IE=Edge,chrome=1");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            await next();
        });
    }
}
