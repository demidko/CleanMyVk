using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using static System.IO.File;
using static System.Console;
using static VkNet.Enums.Filters.Settings;

/// <summary>
/// Класс отвечает за авторизацию пользователя
/// </summary>
internal static class Authorization
{
    /// <summary>
    /// Файл содержит две строки (0 логин, 1 пароль)
    /// </summary>
    private const string UserFile = ".authorization";

    /// <summary>
    /// Используем идентификатор официального приложения
    /// </summary>
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
        $"Login as vk.com/id{api.UserId}".PrintlnWithAttention();
        return api;
    }

    /// <summary>
    /// Метод загружает пару (логин, пароль) с помощью аргументов
    /// </summary>
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


    /// <summary>
    /// Метод извлекает пару (логин, пароль) из аргументов
    /// </summary>
    private static (string Login, string Password) LoadAuthorizationDataFromInput(string[] args)
    {
        WriteAllLines(UserFile, args);
        return ExtractUser(args);
    }

    /// <summary>
    /// Метод извлекает пару (логин, пароль) из кеша
    /// </summary>
    private static (string Login, string Password) LoadAuthorizationDataFromCache() =>
        Exists(UserFile) && ReadAllLines(UserFile) is var lines && lines.Length == 2
            ? ExtractUser(lines)
            : throw new FileFormatException($"{UserFile} file broken or wasn't found");

    /// <summary>
    /// Метод извлекает из массива пару (логин, пароль)
    /// </summary>
    private static (string Login, string Password) ExtractUser(string[] input) => (input[0], input[1]);
}