using static Authorization;

internal static class Application
{
    private static void Main(string[] args) => Login(args).CleanMyComments();
}