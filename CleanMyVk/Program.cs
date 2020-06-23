using System;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Environment;
using static System.UInt32;
using static System.IO.File;

static class Program
{
    /// Здесь закешированы две строки: логин и пароль
    private const string CachePath = ".user";

    /// Здесь хранится одна строка app id
    private const string AppIdPath = ".app";

    private static void Main(string[] args)
    {
        var api = args.LoadVkApi();
        var filter = new NewsFeedGetCommentsParams
        {
            
        };
        api.NewsFeed.GetComments().
        foreach (var i in api.NewsFeed.GetComments(filter).Items)
        {
            WriteLine($"{i.Date}({i.Type}): {i.Text}");
        }
    }
    
    private static async Task<IEquatable<NewsItem>> ListAllNews(VkApi api)
    {
        var pageParams = new NewsFeedGetCommentsParams() { }
        var dataArray = (await api.NewsFeed.GetCommentsAsync(pageParams)).
    }

    private static VkApi LoadVkApi(this string[] args)
    {
        var api = new VkApi();
        var (login, password) = args.LoadLoginAndPassword();
        api.Authorize(new ApiAuthParams
        {
            ApplicationId = LoadAppId(),
            Login = login,
            Password = password,
            Settings = Settings.All
        });
        WriteLine(api.Token);
        return api;
    }

    private static ulong LoadAppId() =>
        Exists(AppIdPath) &&
        ReadAllLines(AppIdPath) is var lines &&
        lines.Length == 1 &&
        TryParse(lines.First(), out var appId)
            ? appId
            : ExitWithMessage<ulong>($"Fatal error: {AppIdPath} file (contains app id only) broken or wasn't found");

    private static (string Login, string Password) LoadLoginAndPassword(this string[] args) => args.Length switch
    {
        0 => LoadLoginAndPasswordFromCache(),
        2 => LoadLoginAndPasswordFrom(args),
        _ => ExitWithMessage<(string, string)>(
            "Usage:\n  With login: dotnet CleanMyVk [login] [password]\n  With cache: dotnet CleanMyVk")
    };

    private static (string Login, string Password) LoadLoginAndPasswordFrom(string[] args)
    {
        WriteAllLines(CachePath, args);
        return (args[0], args[1]);
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