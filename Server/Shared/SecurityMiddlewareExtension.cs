namespace Server.Shared;

public static class SecurityMiddlewareExtension
{
    public static void UseSecurity(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Add("X-UA-Compatible", "IE=Edge,chrome=1");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self'");
            context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
            await next();
        });
    }
}
