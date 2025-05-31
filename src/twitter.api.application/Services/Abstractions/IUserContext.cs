using System;

namespace twitter.api.application.Services.Abstractions
{
    /// <summary>
    /// Abstraction for obtaining the current user's identifier from the request context.
    /// Allows for easy replacement (ej: extracting the user Id from a future JWT token or other authentication mechanism).
    /// </summary>
    public interface IUserContext
    {
        Guid GetCurrentUserId();
    }
}
