namespace Helpers
{
    public static class ConstantUrls
    {

        //Chrome
        public const string backupFilePath = @"C:\Users\sit363.SIT\Desktop\History\history_backup.txt";
        public const string chromeHistoryPath = $@"C:\Users\sit363.SIT\AppData\Local\Google\Chrome\User Data\Profile 18\History";
        public const string chromeCookiesPath = $@"C:\Users\sit363.SIT\AppData\Local\Google\Chrome\User Data\Profile 18\Network\Cookies";
        public const string backupCookiesFilePath = @"C:\Users\sit363.SIT\Desktop\cookies_backup.txt"; // Backup path for cookies
        public const string backupPasswordsFilePath = @"C:\Users\sit363.SIT\Desktop\Passwords\passwords_backup.txt";
        public const string chromePasswordsPath = $@"C:\Users\sit363.SIT\AppData\Local\Google\Chrome\User Data\Profile 18\Login Data For Account";
        public const string localStatePath = $@"C:\Users\sit363.SIT\AppData\Local\Google\Chrome\User Data\Local State";
        public const string chromeWebDataPath = $@"C:\Users\sit363.SIT\AppData\Local\Google\Chrome\User Data\Profile 18\Web Data";
        public const string backupWebDataFilePath = @"C:\Users\sit363.SIT\Desktop\chrome_webdata_backup.txt";

        public static readonly string tempHistoryPath = Path.Combine(Path.GetTempPath(), "ChromeHistory.db");
        public static readonly string tempCookiesPath = Path.Combine(Path.GetTempPath(), "CookiesCopy.db");
        public static readonly string tempLoginDataPath = Path.Combine(Path.GetTempPath(), "LoginDataTemp.db");

        //Edge
        public const string edgebackupFilePath = @"C:\Users\sit363.SIT\Desktop\History\edgehistory_backup.txt";
        public const string edgeHistoryPath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\Edge\User Data\Default\History";
        public const string edgeCookiesPath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\Edge\User Data\Default\Network\Cookies";
        public const string edgebackupCookiesFilePath = @"C:\Users\sit363.SIT\Desktop\cookies_backup.txt"; // Backup path for cookies
        public const string edgebackupPasswordsFilePath = @"C:\Users\sit363.SIT\Desktop\Passwords\edgepasswords_backup.txt";
        public const string edgePasswordsPath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\Edge\User Data\Default\Login Data";
        public const string edgelocalStatePath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\Edge\User Data\Local State";
        public const string edgeWebDataPath = $@"C:\Users\sit363.SIT\AppData\LocalMicrosoft\Edge\User Data\Default\Web Data";
        public const string edgebackupWebDataFilePath = @"C:\Users\sit363.SIT\Desktop\chrome_webdata_backup.txt";

        public static readonly string tempedgePath = Path.Combine(Path.GetTempPath(), "EdgeHistory.db");
        public static readonly string tempedgeDataPath = Path.Combine(Path.GetTempPath(), "LoginEdgeDataTemp.db");

        //Firefox
        public const string firefoxbackupFilePath = @"C:\Users\sit363.SIT\Desktop\History\firefoxhistory_backup.txt";
        public const string firefoxHistoryPath = $@"C:\Users\sit363.SIT\AppData\Roaming\Mozilla\Firefox\Profiles\zbf6llkj.default-release\places.sqlite";
        public const string firefoxCookiesPath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\firefox\User Data\Default\Network\Cookies";
        public const string firefoxbackupCookiesFilePath = @"C:\Users\sit363.SIT\Desktop\cookies_backup.txt"; // Backup path for cookies
        public const string firefoxbackupPasswordsFilePath = @"C:\Users\sit363.SIT\Desktop\passwords\firefoxpasswords_backup.txt";
        public const string firefoxPasswordslogin = $@"C:\Users\sit363.SIT\AppData\Roaming\Mozilla\Firefox\Profiles\zbf6llkj.default-release\logins.json";
        public const string firefoxlocalStatePath = $@"C:\Users\sit363.SIT\AppData\Roaming\Mozilla\Firefox\User Data\Default\Local State";
        public const string firefoxWebDataPath = $@"C:\Users\sit363.SIT\AppData\Local\Microsoft\firefox\User Data\Default\Web Data";
        public const string firefoxbackupWebDataFilePath = @"C:\Users\sit363.SIT\Desktop\chrome_webdata_backup.txt";

        public static readonly string tempfirefoxPath = Path.Combine(Path.GetTempPath(), "firefox.db");
        public const string firefoxkeydb = $@"C:\Users\sit363.SIT\AppData\Roaming\Mozilla\Firefox\Profiles\zbf6llkj.default-release\key4.db";
    }
}
