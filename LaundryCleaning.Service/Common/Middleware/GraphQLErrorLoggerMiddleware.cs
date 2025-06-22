namespace LaundryCleaning.Service.Common.Middleware
{
    public class GraphQLErrorLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GraphQLErrorLoggerMiddleware> _logger;

        public GraphQLErrorLoggerMiddleware(
            RequestDelegate next,
            ILogger<GraphQLErrorLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Buffer response ke memory
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context); // Lanjut ke GraphQL

            memStream.Position = 0;
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();

            // Log error kalau ada
            if (responseBody.Contains("\"errors\""))
            {
                _logger.LogError("GraphQL Error Response: {Response}", responseBody);
            }

            // Kembalikan response ke client
            context.Response.Body = originalBody;
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
        }
    }
}
