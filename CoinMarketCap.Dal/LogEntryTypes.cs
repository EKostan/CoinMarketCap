namespace CoinMarketCap.Dal
{
    public static class LogEntryTypes
    {
        public static string Login => "Login: success";
        public static string LoginWrongPassword => "Login: wrong password";
        public static string LoginWrongEmail => "Login: wrong email";
        public static string LoginNotAllowedUser => "Login: not allowed user";
        public static string LoginBlocked => "Login: user blocked";
        public static string Logout => "Logout";
    }
}