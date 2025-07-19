using Microsoft.Extensions.Caching.Memory;

namespace TickiTackToe.Api.Middlewares
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != HttpMethods.Post)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                await _next(context);
                return;
            }

            var key = $"idem:{idempotencyKey}";

            if (_cache.TryGetValue(key, out var cachedResponse))
            {
                var responseData = (CachedResponse)cachedResponse;
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.Headers["ETag"] = responseData.ETag;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(responseData.Body);
                return;
            }

            var originalBody = context.Response.Body;

            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();

            var etag = context.Response.Headers["ETag"].FirstOrDefault() ?? Guid.NewGuid().ToString();

            _cache.Set(key, new CachedResponse(Body: responseBody, ETag: etag), TimeSpan.FromHours(1));

            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
        }

        private record CachedResponse(string Body, string ETag);
    }

}
