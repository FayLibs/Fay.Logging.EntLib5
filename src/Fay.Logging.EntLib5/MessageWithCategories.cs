using System.Diagnostics.Contracts;

namespace Fay.Logging.EntLib5
{
    /// <summary>
    /// Data object to store a message and its categories it should be logged to.
    /// </summary>
    public class MessageWithCategories
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; }

        /// <summary>
        /// Gets the categories to log the <see cref="Message"/> to. If no categories provided this will be an empty array, never null.
        /// </summary>
        /// <value>The array of categories or an empty array.</value>
        public string[] Categories { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWithCategories"/> struct.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="categories">The categories to log to, if none provided then will use the default/first category available.</param>
        public MessageWithCategories(string message, params string[] categories)
        {
            Contract.Ensures(Categories != null);
            
            Message = message;
            Categories = categories ?? new string[0];
        }
    }
}