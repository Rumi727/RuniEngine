#nullable enable
using RuniEngine.Resource;
using System;

namespace RuniEngine.Account
{
    public class LogoutException : Exception
    {
        public static Exception AlreadyLoggedException() => new LogoutException(LanguageLoader.TryGetText("exception.already_logged"));
        public static Exception LoggedOutUserDataException() => new LogoutException(LanguageLoader.TryGetText("exception.logged_out_user_data"));

        public LogoutException(string message) : base(message)
        {

        }
    }
}
