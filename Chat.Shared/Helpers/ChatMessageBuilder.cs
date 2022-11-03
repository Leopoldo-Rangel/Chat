using System;

namespace Chat.Shared.Helpers
{
    public class ChatMessageBuilder
    {
        public static string FormatMessage(string message, DateTime date, bool isBot, string userName = null)
        {
            switch (isBot)
            {
                case true when !string.IsNullOrEmpty(userName):
                    throw new ArgumentException("Username cannot be specified when building a bot message",
                        nameof(userName));
                case false when string.IsNullOrEmpty(userName):
                    throw new ArgumentException("Username is mandatory when building a non bot message",
                        nameof(userName));
                case true:
                    return $"({date:MM/dd/yyyy HH:mm:ss}) Quote Bot says: {message}";
                default:
                    return $"({date:MM/dd/yyyy HH:mm:ss}) {userName} says: {message}";
            }
        }
    }
}
