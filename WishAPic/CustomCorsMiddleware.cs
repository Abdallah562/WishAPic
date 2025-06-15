namespace WishAPic
{
    public class CustomCorsMiddleware
    {
        private RequestDelegate _next;

        public CustomCorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200"); // Allow specific origin
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            // Handle preflight request
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                return;
            }

            await _next(context);

        }
    }
}
