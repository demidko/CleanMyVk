using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using static System.Linq.Enumerable;

/// <summary>
/// Класс отвечает за очистку комментариев пользователя.
/// </summary>
internal static class CommentCleaner
{
    /// <summary>
    /// Метод удаляет все оставленные пользователем комментарии
    /// </summary>
    internal static void CleanMyComments(this VkApi api)
    {
        foreach (var comment in api.GetMyComments())
        {
            api.Wall.DeleteComment(comment.OwnerId, comment.Id);
        }
    }

    /// <summary>
    /// Метод перечисляет все комментарии пользователя
    /// </summary>
    private static IEnumerable<Comment> GetMyComments(this VkApi api)
    {
        foreach (var post in api.GetMyCommentedPosts())
        {
            foreach (var comment in api.GetCommentsOf(post))
            {
                if (comment.FromId == api.UserId)
                {
                    //WriteLine($"found your comment vk.com/wall{post.SourceId}_{post.PostId}?reply={comment.Id}");
                    yield return comment;
                }
            }
        }
    }

    /// <summary>
    /// Метод перечисляет все комментарии конкретного поста
    /// </summary>
    private static IEnumerable<Comment> GetCommentsOf(this VkApi api, NewsItem post)
    {
        for (var i = api.GetCommentsEnumerator(post);; i = i.Next())
        {
            foreach (var comment in i.Comments)
            {
                yield return comment;
            }
            if (i.IsLast())
            {
                break;
            }
        }
    }

    /// <summary>
    /// Метод получает следующий пакет комментариев
    /// </summary>
    private static (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) Next(
        this (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) it)
        => it.Api.GetCommentsEnumerator(it.Next);

    /// <summary>
    /// Метод проверяет что можно получить следующий пакет комментариев поста
    /// </summary>
    private static bool HasNext(this (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) it) =>
        it.Comments.Any();

    /// <summary>
    /// Метод проверяет что это был последний пакет постов
    /// </summary>
    private static bool IsLast(this (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) it) =>
        !it.HasNext();

    /// <summary>
    /// Метод возвращает пакет комментариев на основе запроса + запрос для получения следующего по порядку пакета
    /// </summary>
    private static (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) GetCommentsEnumerator(
        this VkApi api, WallGetCommentsParams @params)
    {
        var answer = api.Wall.GetComments(@params);
        @params.Offset += answer.Count;

        WriteLine(
            $"post vk.com/wall{@params.OwnerId}_{@params.PostId} -> (count: {answer.Count}, offset: {@params.Offset})");

        return (@params, answer.Items, api);
    }

    /// <summary>
    /// Метод возвращает пакет комментариев для поста + запрос для получения следующего по порядку пакета
    /// </summary>
    private static (WallGetCommentsParams Next, IEnumerable<Comment> Comments, VkApi Api) GetCommentsEnumerator(
        this VkApi api, NewsItem post) => api.GetCommentsEnumerator(new WallGetCommentsParams
    {
        OwnerId = post.SourceId,
        PostId = (long) post.PostId!.Value,
        Offset = 0,
        Count = 100
    });

    /// <summary>
    ///  Метод перечисляет все когда либо прокомментированные пользователем посты
    /// </summary>
    private static IEnumerable<NewsItem> GetMyCommentedPosts(this VkApi api)
    {
        for (var i = api.GetPostsEnumerator();; i = i.Next())
        {
            foreach (var post in i.News)
            {
                yield return post;
            }
            if (i.IsLast())
            {
                break;
            }
        }
    }

    /// <summary>
    /// Метод возвращает следующий пакет прокомментированных постов
    /// </summary>
    private static (long? Next, IEnumerable<NewsItem> News, VkApi Api) Next(
        this (long? Next, IEnumerable<NewsItem> News, VkApi Api) it) => it.Api.GetPostsEnumerator(it.Next);

    /// <summary>
    /// Метод проверяет что есть следующий пакет прокомментированных постов
    /// </summary>
    private static bool HasNext(this (long? Next, IEnumerable<NewsItem> News, VkApi Api) it) => !it.IsLast();

    /// <summary>
    /// Метод проверяет что это последний пакет постов
    /// </summary>
    private static bool IsLast(this (long? Next, IEnumerable<NewsItem> News, VkApi Api) it) => it.Next == null;

    /// <summary>
    /// Метод возвращает следующий по порядку перечислитель прокомментированных постов на основе смещения,
    /// по умолчанию с самого первого прокомментированного поста
    /// </summary>
    private static (long? Next, IEnumerable<NewsItem> News, VkApi Api) GetPostsEnumerator(
        this VkApi api, long? start = null)
    {
        var answer = api.NewsFeed.GetComments(new NewsFeedGetCommentsParams
        {
            Count = 100,
            StartFrom = start
        });
        var nextFrom = answer.NextFrom == null ? (long?) null : long.Parse(answer.NextFrom);
        return (nextFrom, answer.Items, api);
    }
}