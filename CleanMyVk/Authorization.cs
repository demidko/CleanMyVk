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
    /// In this file, the login data is cached line by line in the following order:
    /// login, password
    private const string UserFile = ".authorization";

    private const ulong ApplicationId = 2274003;

    /// <summary>
    /// Метод входит под именем и паролем пользователя
    /// </summary>
    /// <param name="args">логин, пароль</param>
    /// <returns>VK Api</returns>
    internal static VkApi Login(string[] args)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        var api = new VkApi(services);
        var (login, password) = LoadAuthorizationData(args);
        api.Authorize(new ApiAuthParams
        {
            ApplicationId = ApplicationId,
            Login = login,
            Password = password,
            Settings = All
        });
        WriteLine($"Login as vk.com/id{api.UserId}");
        return api;
    }

    private static ( string Login, string Password) LoadAuthorizationData(string[] args) =>
        args.Length switch
        {
            0 => LoadAuthorizationDataFromCache(),
            2 => LoadAuthorizationDataFromInput(args),
            _ => throw new ArgumentException(
                "Usage:\n" +
                "  With authorization data: dotnet CleanMyVk [login] [password]\n" +
                "  With cache: dotnet CleanMyVk")
        };

    private static (string Login, string Password) LoadAuthorizationDataFromInput(string[] input)
    {
        WriteAllLines(UserFile, input);
        return ExtractUser(input);
    }

    private static (string Login, string Password) LoadAuthorizationDataFromCache() =>
        Exists(UserFile) && ReadAllLines(UserFile) is var lines && lines.Length == 2
            ? ExtractUser(lines)
            : throw new FileFormatException($"{UserFile} file broken or wasn't found");

    private static (string Login, string Password) ExtractUser(string[] input) => (input[0], input[1]);
}