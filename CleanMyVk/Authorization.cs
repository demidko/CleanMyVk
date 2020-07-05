using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using static System.IO.File;
using static System.UInt64;
using static System.Console;
using static VkNet.Enums.Filters.Settings;

internal static class Authorization
{
    /// In this file, the login data is cached line by line in the following order: appId, login, password
    private const string CachePath = ".authorization";

    internal static VkApi Login(string[] args)
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
            Settings = All
        });
        WriteLine($"Login as vk.com/{api.UserId}");
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
        return ExtractAuthorizationData(input);
    }

    private static (ulong AppId, string Login, string Password) LoadAuthorizationDataFromCache() =>
        Exists(CachePath) && ReadAllLines(CachePath) is var lines && lines.Length == 3
            ? ExtractAuthorizationData(lines)
            : throw new FileFormatException($"{CachePath} file broken or wasn't found");

    private static (ulong AppId, string Login, string Password) ExtractAuthorizationData(string[] input) =>
        (Parse(input[0]), input[1], input[2]);
}