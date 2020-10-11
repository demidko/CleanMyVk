using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Model;
using static System.IO.File;
using static System.Console;
using static System.ConsoleColor;
using static VkNet.Enums.Filters.Settings;

/// <summary>
/// Модуль отвечает за авторизацию пользователя
/// </summary>
internal static class Authorization
{
    /// <summary>
    /// Файл содержит две строки (1 логин, 2 пароль)
    /// </summary>
    private const string UserFile = ".authorization";

    /// <summary>
    /// Метод входит под именем и паролем пользователя
    /// </summary>
    /// <param name="args">номер (или email), пароль</param>
    /// <returns>VK Api</returns>
    internal static VkApi Login(IReadOnlyList<string> args)
    {
        var api = new VkApi();
        var (login, password) = LoadAuthorizationData(args);
        login.Println();
        password.Println();
        api.Authorize(new ApiAuthParams
        {
            // Используем app id который откопали где-то в интернете
            ApplicationId = 1980660,
            Login = login,
            Password = password,
            Settings = All
        });
        $"Login as vk.com/id{api.UserId}".Println(DarkBlue);
        return api;
    }

    /// <summary>
    /// Метод загружает пару (логин, пароль) с помощью аргументов
    /// </summary>
    private static ( string Login, string Password) LoadAuthorizationData(IReadOnlyList<string> args) =>
        args.Count switch
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
    private static (string Login, string Password) LoadAuthorizationDataFromInput(IReadOnlyList<string> args)
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
            : throw new Exception($"{UserFile} file broken or wasn't found");

    /// <summary>
    /// Метод извлекает из массива пару (логин, пароль)
    /// </summary>
    private static (string Login, string Password) ExtractUser(IReadOnlyList<string> input) => (input[0], input[1]);
}