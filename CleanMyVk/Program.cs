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
        var (login, password) = LoadLoginAndPassword(args);
        
    }

    private static (string Login, string Password) LoadLoginAndPassword(string[] args) => args.Length switch
    {
        0 => LoadLoginAndPasswordFromCache(),
        2 => LoadLoginAndPasswordFromArgs(args),
        _ => ExitWithMessage<(string, string)>("First usage:\ndotnet CleanMyVk [login] [password]")
    };


    private static (string Login, string Password) LoadLoginAndPasswordFromArgs(string[] secretArgs)
    {
        WriteAllLines(CachePath, secretArgs);
        return (secretArgs[0], secretArgs[1]);
    }

    private static (string Login, string Password) LoadLoginAndPasswordFromCache()
    {
        if (!Exists(CachePath))
        {
            ExitWithMessage(".cache file was not found");
        }

        var lines = ReadAllLines(CachePath);
        if (lines.Length != 2)
        {
            ExitWithMessage("Broken .cache file");
        }

        return (lines[0], lines[1]);
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