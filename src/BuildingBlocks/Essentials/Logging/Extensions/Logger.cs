using Essentials.ErrorHandling.Extensions;
using Microsoft.Extensions.Logging;

namespace Essentials.Logging.Extensions
{
    public static class Logger
    {
        public static void LogException(this ILogger logger, EventId eventId, Exception exception, string message, params object?[] args)
        {
            var inner = exception.GetInnerExceptions();
            var exceptionMessage = $" Exception Message: {exception.Message} " +
                $"{Environment.NewLine}" +
                $"Inner Exceptions: {string.Join(Environment.NewLine, inner.Select(x => x.Message))} " +
                $"{Environment.NewLine} " +
                $"StackTrace: {exception.StackTrace}";

            Array.Resize(ref args, args.Length + 1);
            args[args.Length - 1] = exceptionMessage;

            var enrichedMessage = message + "Exception details: {ExceptionDetail}";

            logger.LogError(eventId, enrichedMessage, args);
        }
    }
}
