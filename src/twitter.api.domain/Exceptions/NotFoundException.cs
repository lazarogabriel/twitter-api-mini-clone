using System.Runtime.CompilerServices;

namespace twitter.api.domain.Exceptions
{
    /// <summary>
    /// Defines a not found error.
    /// Use this when a domain resource (Tweet, User) cannot be located.
    /// </summary>
    public class NotFoundException : DomainException
    {
        /// <summary>
        /// Initializes a new <see cref="NotFoundException"/>.
        /// <param name="type">Unique identifier for the exception.</param>
        /// <param name="message">Message detail for the exception.</param>
        /// <param name="values">The values to replace in message template, if any exists.</param>
        /// </summary>
        public NotFoundException(string message, [CallerArgumentExpression("message")] string type = null, params object[] values) :
            base(type.Split('.')[1], message, values)
        {
        }
    }
}
