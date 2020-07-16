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
    internal static void CleanMyComments(this VkApi api)
    {
        for (var it = api.NewsIterator(); it != null; it = it.Next())
        {
            foreach (var post in it?.News)
            {
                WriteLine($"post vk.com/wall{post.SourceId}_{post.PostId}");
                WriteLine(post.Comments.List == null);
                api.Wall.GetComments(new WallGetCommentsParams()
                {
                    
                })
                foreach (var comment in post.Comments?.List ?? Empty<Comment>())
                {
                    WriteLine($"  vk.com/{comment.OwnerId}: {comment.Text ?? ""}");
                }
            }
        }
    }

    /// <summary>
    /// Метод инкрементирует итератор новостей
    /// </summary>
    /// <param name="self">итератор</param>
    /// <returns>следующий итератор или null</returns>
    private static (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)?
        Next(this (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)? self) => self?.NextFrom switch
    {
        null => null,
        _ => self?.Api.NewsIterator(self?.NextFrom)
    };

    /// <summary>
    /// Метод отдает итератор новостей которые пользователь прокомментировал
    /// </summary>
    /// <param name="api">VK api</param>
    /// <param name="startFrom">смещение</param>
    /// <returns>итератор</returns>
    private static (long? NextFrom, IEnumerable<NewsItem> News, VkApi Api)? NewsIterator(
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