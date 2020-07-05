using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using static System.IO.File;
using static System.UInt64;
using static System.Console;

internal static class Authorization
{
    /// In this file, the login data is cached line by line in the following order: appId, login, password
    private const string CachePath = ".cache";

    public static VkApi Login(string[] args)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        var api = new VkApi(services);
        var (appId, login, password) = LoadAuthorizationData(args);
        api.Authorize(new ApiAuthParams
        {
            ApplicationId = appId,
            Login = login,
            Password = password,
            Settings = Settings.All
        });
        WriteLine($"Login as vk.com/{api.Account.GetProfileInfo().ScreenName}");
        return api;
    }

    private static (ulong AppId, string Login, string Password) LoadAuthorizationData(string[] args) =>
        args.Length switch
        {
            0 => LoadAuthorizationDataFromCache(),
            3 => LoadAuthorizationDataFromInput(args),
            _ => throw new ArgumentException(
                "Usage:\n" +
                "  With authorization data: dotnet CleanMyVk [appId] [login] [password]\n" +
                "  With cache: dotnet CleanMyVk")
        };

    private static (ulong AppId, string Login, string Password) LoadAuthorizationDataFromInput(string[] input)
    {
        WriteAllLines(CachePath, input);
        return (Parse(input[0]), input[1], input[2]);
    }

    private static (ulong AppId, string Login, string Password) LoadAuthorizationDataFromCache() =>
        Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 3
            ? (lines[0], lines[1], lines[2])
            : throw new FileFormatException($"{CachePath} file broken or wasn't found");
}