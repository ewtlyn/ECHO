using ECHO.Models;

namespace ECHO;

public static class UserSession
{
    public static User? CurrentUser { get; set; }

    public static bool IsAuthorized => CurrentUser != null;

    public static void Logout()
    {
        CurrentUser = null;
    }
}