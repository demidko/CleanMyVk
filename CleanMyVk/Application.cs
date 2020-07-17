using static Authorization;

/// <summary>
/// Точка входа описывает логику приложения на высоком уровне
/// </summary>
internal static class Application
{
    private static void Main(string[] me) => Login(me).CleanMyComments();
}