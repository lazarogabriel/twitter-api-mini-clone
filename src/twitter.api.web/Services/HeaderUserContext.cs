using Microsoft.AspNetCore.Http;
using System;
using twitter.api.application.Services.Abstractions;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;

namespace twitter.api.web.Services
{
    public class HeaderUserContext : IUserContext
    {
        private const string XUserIdHeaderFormat = "X-User-Id";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public Guid GetCurrentUserId()
        {
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;

            if (headers == null || !headers.TryGetValue(XUserIdHeaderFormat, out var userIdHeader))
            {
                throw new AuthenticationException(Errors.MissingUserIdHeader);
            }

            if (!Guid.TryParse(userIdHeader, out Guid userId))
            {
                throw new AuthenticationException(Errors.InvalidUserIdHeader);
            }

            return userId;
        }
    }
}
