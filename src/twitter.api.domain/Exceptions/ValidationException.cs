using System.Runtime.CompilerServices;

namespace twitter.api.domain.Exceptions
{
    public class ValidationException : DomainException
    {
        /// <summary>
        /// Initializes a new <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="type">Unique identifier for the exception.</param>
        /// <param name="message">Message detail for the exception.</param>
        /// <param name="values">The values to replace in message template, if any exists.</param>
        public ValidationException(string message, [CallerArgumentExpression("message")] string type = null, params object[] values)
        : base(type.Split('.')[1], message, values)
        {
        }
    }
}
