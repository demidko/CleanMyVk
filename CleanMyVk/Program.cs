using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using static System.Console;
using static System.Environment;
using static System.UInt32;
using static System.IO.File;

internal static class Program
{
    /// Здесь закешированы две строки: логин и пароль
    private const string CachePath = ".user";

    /// Здесь хранится одна строка app id
    private const string AppIdPath = ".app";

    private static void Main(string[] args)
    {
        var api = InitVkApi(args);
        WriteLine($"Login as vk.com/{api.Account.GetProfileInfo().ScreenName}");
        var conversationsList =
            api.Messages.GetConversationsById(new List<long> {186068693});
        WriteLine(conversationsList.Count);
    }

    private static VkApi InitVkApi(string[] args)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        var api = new VkApi(services);
        var (login, password) = LoadLoginAndPassword(args);
        api.Authorize(new ApiAuthParams
        {
            ApplicationId = LoadAppId(),
            Login = login,
            Password = password,
            Settings = Settings.All
        });
        return api;
    }


    private static ulong LoadAppId()
    {
        return Exists(AppIdPath) &&
               ReadAllLines(AppIdPath) is var lines &&
               lines.Length == 1 &&
               TryParse(lines.First(), out var appId)
            ? appId
            : ExitWithMessage<ulong>($"Fatal error: {AppIdPath} file (contains app id only) broken or wasn't found");
    }

    private static (string Login, string Password) LoadLoginAndPassword(string[] args)
    {
        return args.Length switch
        {
            0 => LoadLoginAndPasswordFromCache(),
            2 => LoadLoginAndPasswordFrom(args),
            _ => ExitWithMessage<(string, string)>(
                "Usage:\n  With login: dotnet CleanMyVk [login] [password]\n  With cache: dotnet CleanMyVk")
        };
    }

    private static (string Login, string Password) LoadLoginAndPasswordFrom(string[] args)
    {
        WriteAllLines(CachePath, args);
        return (args[0], args[1]);
    }

    private static (string Login, string Password) LoadLoginAndPasswordFromCache()
    {
        return Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 2
            ? (lines[0], lines[1])
            : ExitWithMessage<(string, string)>($"{CachePath} file broken or wasn't found");
    }

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