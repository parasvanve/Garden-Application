using System.Security.Claims;

namespace GardenBookingApp.Services
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // MUST match the session key in AccountController
            var username = context.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, username)
                };

                var identity = new ClaimsIdentity(claims, "SessionAuth");
                var principal = new ClaimsPrincipal(identity);

                context.User = principal;
            }

            await _next(context);
        }
    }
}