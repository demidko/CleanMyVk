using System;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using static System.Environment;
using static System.UInt32;
using static System.IO.File;

static class Program
{
    private const string CachePath = ".cache";
    private const string AppIdPath = ".app";

    private static async Task Main(string[] args)
    {
        var api = new VkApi();
        var (login, password) = args.LoadLoginAndPassword();
        api.Authorize(new ApiAuthParams
        {
            ApplicationId = LoadAppId(),
            Login = login,
            Password = password
        });
        Console.WriteLine(api.Token);
    }

    private static ulong LoadAppId() =>
        Exists(AppIdPath) &&
        ReadAllLines(AppIdPath) is var lines &&
        lines.Length == 1 &&
        TryParse(lines.First(), out var appId)
            ? appId
            : ExitWithMessage<ulong>("Fatal error: .app file (contains app id only) broken or wasn't found");

    private static (string Login, string Password) LoadLoginAndPassword(this string[] args) => args.Length switch
    {
        0 => LoadLoginAndPasswordFromCache(),
        2 => LoadLoginAndPasswordFrom(args),
        _ => ExitWithMessage<(string, string)>(
            "usage:\n  With login: dotnet CleanMyVk [login] [password]\n  With cache: dotnet CleanMyVk")
    };

    private static (string Login, string Password) LoadLoginAndPasswordFrom(string[] args)
    {
        WriteAllLines(CachePath, args);
        return (args[0], args[1]);
    }

    private static (string Login, string Password) LoadLoginAndPasswordFromCache() =>
        Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 2
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