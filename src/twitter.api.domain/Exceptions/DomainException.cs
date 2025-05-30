using System;
using System.Linq;

namespace twitter.api.domain.Exceptions
{
    /// <summary>
    /// Base domain error definition.
    /// </summary>
    public abstract class DomainException : Exception
    {
        /// <summary>
        /// Initilizes a new <see cref="DomainException"/> with a more specific type and values.
        /// </summary>
        /// <param name="type">Unique identifier for the exception.</param>
        /// <param name="message">Message detail for the exception.</param>
        /// <param name="values">The values to replace in message template, if any exists.</param>
        protected DomainException(string type, string message, params object[] values) : base(string.Format(message, values))
        {
            ErrorType = type;
            Values = values;
        }

        /// <summary>
        /// Domain's error type
        /// </summary>
        public string ErrorType { get; }

        /// <summary>
        /// Internal exception values
        /// </summary>
        public object[] Values { get; }

        /// <inheritdoc/>
        public override string ToString() =>
            $"Failed with {ErrorType}, " +
            TryGetValuesFormatted() +
            TryGetErrorTextFormatted() +
            $"\n{base.ToString()}";

        #region Private Methods

        private string TryGetErrorTextFormatted() =>
            string.IsNullOrEmpty(Message)
                ? string.Empty
                : $"\nwith Message: {Message}";

        private string TryGetValuesFormatted() =>
            Values == null || !Values.Any()
                ? string.Empty
                : $"\nwith values {string.Join(" ", Values)}";
        
        #endregion
    }
}