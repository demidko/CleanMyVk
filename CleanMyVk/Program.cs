using System;
using static System.Console;
using System.Threading.Tasks;
using static System.Environment;
using static System.IO.File;

static class Program
{
    private const string CachePath = ".cache";

    private static async Task Main(string[] args)
    {
        var (login, password) = args.LoadLoginAndPassword();
    }

    private static (string Login, string Password) LoadLoginAndPassword(this string[] args) => args.Length switch
    {
        0 => LoadLoginAndPasswordFromCache(),
        2 => LoadLoginAndPasswordFrom(args),
        _ => ExitWithMessage<(string, string)>("First usage:\ndotnet CleanMyVk [login] [password]")
    };

    private static (string Login, string Password) LoadLoginAndPasswordFrom(string[] args)
    {
        WriteAllLines(CachePath, args);
        return (args[0], args[1]);
    }

    private static (string Login, string Password) LoadLoginAndPasswordFromCache() =>
        (Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 2)
            ? (lines[0], lines[1])
            : ExitWithMessage<(string, string)>(".cache file broken or wasn't found");

    private static T ExitWithMessage<T>(string message) where T : new()
    {
        ExitWithMessage(message);
        return new T();
    }

    private static void ExitWithMessage(string message)
    {
        WriteLine(message);
        Exit(1);
    }
}