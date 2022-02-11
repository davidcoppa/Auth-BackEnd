using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.Middleware
{
    public static class LogHTTPResponseExtensions
    {
        public static IApplicationBuilder UseLogHTTPResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogHTTPResponse>();
        }

    }
    public class LogHTTPResponse
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogHTTPResponse> logger;

        public LogHTTPResponse(RequestDelegate next,ILogger<LogHTTPResponse> logger)
        {
            this.next = next;
            this.logger = logger;

        }
        public async Task InvokeAsync(HttpContext context)
        {
            using var ms = new MemoryStream();
          
            var originResponse = context.Response.Body;
            context.Response.Body = ms;

            await next(context);

            ms.Seek(0, SeekOrigin.Begin);
            string response = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(originResponse);
            context.Response.Body = originResponse;

            logger.LogInformation(response);

        }

    }
}
