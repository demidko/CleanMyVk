using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using static System.Linq.Enumerable;

internal static class Comments
{
    /// <summary>
    /// Метод удаляет все оставленные пользователем комментарии
    /// </summary>
    /// <param name="api">VK API</param>
    internal static void CleanMyComments(this VkApi api)
    {
        foreach (var post in api.GetMyCommentedNews())
        {
            var postLink = $"vk.com/wall{post.SourceId}_{post.PostId}";
            foreach (var comment in api.Wall.GetComments(new WallGetCommentsParams
            {
                OwnerId = post.SourceId,
                PostId = (long) post.PostId!.Value
                // TODO дело в оффсетах!
            }).Items.Where(comment =>
            {
                WriteLine($"Removing the comment {postLink}?reply={comment.Id}");
                return comment.FromId == api.UserId;
            }))
            {
                WriteLine($"Removing the comment {postLink}?reply={comment.Id}");
                if (comment.Text.Any())
                {
                    WriteLine($"  {comment.Text}");
                }
                api.Wall.DeleteComment(post.SourceId, comment.Id);
            }
        }
    }


    private static (IEnumerable<Comment> Comments, long Offset, VkApi Api) Next(
        (IEnumerable<Comment> Comments, long Offset, VkApi Api) current)
    {
        return current.Api.GetCo
    }

    /// <summary>
    /// Метод проверяет что есть еще комментарии
    /// </summary>
    private static bool HasNext((IEnumerable<Comment> Comments, long Offset, VkApi Api) it) => 
        it.Comments.Any();

    /// <summary>
    /// Метод возвращает итератор по комментариям конкретного поста
    /// </summary>
    private static (IEnumerable<Comment> Comments, long Offset, VkApi Api) GetCommentsIteratorFor(
        this VkApi api, NewsItem post, long offset = 0)
    {
        var answer = api.Wall.GetComments(new WallGetCommentsParams
        {
            OwnerId = post.SourceId,
            PostId = (long) post.PostId!.Value,
            Offset = offset,
            Count = 100
        });
        // TODO здесь мы можем возвращать сам параметр подготовленый для следующего запроса
        return (answer.Items, offset + answer.Count, api);
    }


    /// <summary>
    /// Фильтр отбирающий комментарии текущего пользователя
    /// </summary>
    private static Predicate<Comment> OnlyMyComments(this VkApi api) =>
        comment => comment.FromId == api.UserId;

    /// <returns>Все когда либо прокомментированные новости </returns>
    private static IEnumerable<NewsItem> GetMyCommentedNews(this VkApi api)
    {
        for (var i = api.GetNewsIterator(); i != null; i = i.Next())
        {
            foreach (var post in i.Value.News)
            {
                yield return post;
            }
        }
    }

    // TODO избавиться от null переделать в условие с hasNext
    
    /// <summary>
    /// Метод инкрементирует итератор новостей которые пользователь прокомментировал
    /// </summary>
    /// <param name="self">итератор новостей которые пользователь прокомментировал</param>
    /// <returns>следующий итератор или null</returns>
    private static (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)? Next(
        this (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)? self) => self?.NextFrom switch
    {
        null => null,
        _ => self?.Api.GetNewsIterator(self?.NextFrom)
    };

    /// <param name="api">VK api</param>
    /// <param name="startFrom">смещение</param>
    /// <returns>итератор новостей которые пользователь прокомментировал</returns>
    private static (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)? GetNewsIterator(
        this VkApi api, long? startFrom = null)
    {
        var answer = api.NewsFeed.GetComments(new NewsFeedGetCommentsParams
        {
            Count = 100,
            StartFrom = startFrom
        });
        var nextFrom = answer.NextFrom == null ? (long?) null : long.Parse(answer.NextFrom);
        return (nextFrom, answer.Items, api);
    }
}