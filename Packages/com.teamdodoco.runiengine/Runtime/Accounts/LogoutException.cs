using RuniEngine.Resource.Texts;
using System;

namespace RuniEngine.Accounts
{
    /// <summary>
    /// 로그아웃 관련 예외
    /// </summary>
    public class LogoutException : Exception
    {
        /// <returns><see cref="LogoutException"/> 이미 로그아웃되었습니다</returns>
        public static LogoutException AlreadyLoggedException() => new LogoutException(LanguageLoader.TryGetText("exception.already_logged"));
        /// <returns><see cref="LogoutException"/> 로그아웃된 상태에서 유저 데이터에 접근할 수 없습니다</returns>
        public static LogoutException LoggedOutUserDataException() => new LogoutException(LanguageLoader.TryGetText("exception.logged_out_user_data"));

        public LogoutException(string message) : base(message)
        {

        }
    }
}
