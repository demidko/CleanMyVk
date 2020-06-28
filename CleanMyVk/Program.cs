using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
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

    private static void Main(string[] args) => Login(args).CleanMyComments().CleanMyLikes();

    private static VkApi CleanMyComments(this VkApi api)
    {
        return api;
    }

    private static VkApi CleanMyLikes(this VkApi api)
    {
        return api;
    }

    private static VkApi Login(string[] args)
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
        WriteLine($"Login as vk.com/{api.Account.GetProfileInfo().ScreenName}");
        return api;
    }
    
    private static ulong LoadAppId() =>
        Exists(AppIdPath) &&
        ReadAllLines(AppIdPath) is var lines &&
        lines.Length == 1 &&
        TryParse(lines.First(), out var appId)
            ? appId
            : ExitWithMessage<ulong>($"Fatal error: {AppIdPath} file (contains app id only) broken or wasn't found");


    private static (string Login, string Password) LoadLoginAndPassword(string[] args) => args.Length switch
    {
        0 => LoadLoginAndPasswordFromCache(),
        2 => LoadLoginAndPasswordFromInput(args),
        _ => ExitWithMessage<(string, string)>(
            "Usage:\n  With login: dotnet CleanMyVk [login] [password]\n  With cache: dotnet CleanMyVk")
    };


    private static (string Login, string Password) LoadLoginAndPasswordFromInput(string[] input)
    {
        WriteAllLines(CachePath, input);
        return (input[0], input[1]);
    }

    private static (string Login, string Password) LoadLoginAndPasswordFromCache() =>
        Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 2
            ? (lines[0], lines[1])
            : ExitWithMessage<(string, string)>($"{CachePath} file broken or wasn't found");


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