## CleanMyVk
Утилита удаляет оставленную в ВК информацию из публичного доступа
* Очищает при запуске все комментарии пользователя в сообществах и на страницах
### Как запускать проект напрямую из исходного кода?
1. Установить [.NET Core SDK](https://dot.net)
2. Перейти в директорию проекта (csproj) и запустить командой:  
`dotnet run -- [login] [password]`  
После этого можно запускать используя кеш предыдущей авторизации:  
`dotnet run`
### Todo
* Реализовать очистку лайков пользователя (Игорь)
