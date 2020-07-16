using static Authorization;

internal static class Application
{
    private static void Main(string[] user) => Login(user).CleanMyComments();
}