using Microsoft.AspNetCore.Builder;

namespace twitter.api.web.Extensions
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Adds custom error handler into the pipeline.
        /// </summary>
        /// <param name="builder"></param>
        public static void UseErrorHandler(this IApplicationBuilder builder) =>
            builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
