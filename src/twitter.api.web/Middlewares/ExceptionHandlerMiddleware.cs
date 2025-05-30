using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using twitter.api.domain.Exceptions;
using twitter.api.web.Models.Responses;
using Newtonsoft.Json;
using System.Linq;

namespace twitter.api.web.Extensions
{
    public class ExceptionHandlerMiddleware
    {
        #region Constants

        private const int MaxInnerExceptionStack = 10;

        #endregion

        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Constructor

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Public methods

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleError(context, e, loggerFactory.CreateLogger<ExceptionHandlerMiddleware>());
            }
        }

        #endregion

        #region Private methods

        private async Task HandleError(HttpContext context, Exception exception, ILogger logger)
        {
            context.Response.ContentType = "application/problem+json";

            var (statusCode, errorResponse) = exception switch
            {
                ValidationException ex => (
                    StatusCodes.Status422UnprocessableEntity,
                    new ErrorResponse(ex.ErrorType, ex.Message, ex.Values.FirstOrDefault()?.ToString())
                ),
                InvalidParameterException ex => (
                    StatusCodes.Status422UnprocessableEntity,
                    new ErrorResponse(ex.ErrorType, ex.Message)
                ),
                RepeatedElementException ex => (
                    StatusCodes.Status409Conflict,
                    new ErrorResponse(ex.ErrorType, ex.Message)
                ),
                NotFoundException ex => (
                    StatusCodes.Status404NotFound,
                    new ErrorResponse(ex.ErrorType, ex.Message, ex.Values.FirstOrDefault()?.ToString())
                ),
                DomainException ex => (
                    StatusCodes.Status400BadRequest,
                    new ErrorResponse(ex.ErrorType, ex.Message)
                ),
                _ => HandleUnknownException(exception, logger)
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private static (int, ErrorResponse) HandleUnknownException(Exception ex, ILogger logger)
        {
            var inner = ex;
            var stackIndex = 0;

            while (inner != null && stackIndex < MaxInnerExceptionStack)
            {
                logger.LogCritical(inner.Message);
                logger.LogCritical(inner.StackTrace);
                inner = inner.InnerException;
                stackIndex++;
            }

            return (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("InternalError", "Server internal error")
            );
        }

        #endregion
    }
}
