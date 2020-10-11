using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VkNet;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using static System.ConsoleColor;
using static System.Linq.Enumerable;
using static System.String;
using static Terminal;

/// <summary>
/// Класс отвечает за удаление комментариев.
/// Осторожно! Код написан под воздействием тяжелой функциональщины.
/// </summary>
internal static class Comments
{
  /// <summary>
  /// Метод удаляет все оставленные пользователем комментарии
  /// </summary>
  public static void CleanComments(this VkApi api)
  {

    foreach (var post in api.CommentedPosts())
    {
      post.Println();
      foreach (var comment in api.CommentsOf(post).Where(x => x.FromId == api.UserId))
      {
        api.Delete(comment);
      }
    }
  }

  private static void Delete(this VkApi api, Comment comment)
  {
    try
    {
      if (api.Wall.DeleteComment(comment.OwnerId, comment.Id))
      {
        
      }
      else
      {
        throw new InvalidOperationException();
      }
    }
    catch (Exception)
    {
      
    }
  }

  /// <summary>
  ///  Метод перечисляет все когда либо прокомментированные пользователем посты
  /// </summary>
  internal static IEnumerable<NewsItem> CommentedPosts(this VkApi api)
  {
    for (var i = api.PostsEnumerator();; i = i.Next())
    {
      foreach (var post in i.News)
      {
        yield return post;
      }
      if (i.IsLast())
      {
        yield break;
      }
    }
  }

  /// <summary>
  /// Метод возвращает следующий пакет прокомментированных постов
  /// </summary>
  private static (long? Next, IEnumerable<NewsItem> News, VkApi Api) Next(
    this (long? Next, IEnumerable<NewsItem> News, VkApi Api) it) => it.Api.PostsEnumerator(it.Next);

  /// <summary>
  /// Метод проверяет что это последний пакет постов
  /// </summary>
  private static bool IsLast(this (long? Next, IEnumerable<NewsItem> News, VkApi Api) it) => it.Next == null;

  /// <summary>
  /// Метод возвращает следующий по порядку перечислитель прокомментированных постов на основе смещения,
  /// по умолчанию с самого первого прокомментированного поста
  /// </summary>
  private static (long? Next, IEnumerable<NewsItem> News, VkApi Api) PostsEnumerator(
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

  /// <summary>
  /// Все комментарии к конкретному посту включая вложенные
  /// </summary>
  internal static IEnumerable<Comment> CommentsOf(this VkApi api, NewsItem post, long? commentId = null)
  {
    for (var threadBatch = api.CommentsEnumerator(post, commentId);; threadBatch = threadBatch.Next())
    {
      foreach (var comment in threadBatch.Comments)
      {
        if (commentId == null)
        {
          foreach (var nested in api.UnwrapThread(post, comment))
          {
            yield return nested;
          }
        }
        else
        {
          yield return comment;
        }
      }
      if (threadBatch.IsLast)
      {
        yield break;
      }
    }
  }

  /// <summary>
  /// Комментарий + вложенные
  /// </summary>
  private static IEnumerable<Comment> UnwrapThread(this VkApi api, NewsItem post, Comment comment)
  {
    yield return comment;
    if (comment.Thread.Count > 10)
    {
      post.Println();

      foreach (var nested in api.CommentsOf(post, comment.Id))
      {
        yield return nested;
      }

      yield break;
    }
    foreach (var nested in comment.Thread.Items)
    {
      yield return nested;
    }
  }

  /// <summary>
  /// Метод получает следующий пакет комментариев
  /// </summary>
  private static (bool IsLast, WallGetCommentsParams Params, IEnumerable<Comment> Comments, VkApi Api) Next(
    this (bool IsLast, WallGetCommentsParams Params, IEnumerable<Comment> Comments, VkApi Api) it) =>
    it.Api.CommentsEnumerator(it.Params);


  /// <summary>
  /// Метод возвращает пакет комментариев на основе запроса + запрос для получения следующего по порядку пакета
  /// </summary>
  private static (bool IsLast, WallGetCommentsParams Params, IEnumerable<Comment> Comments, VkApi Api)
    CommentsEnumerator(this VkApi api, WallGetCommentsParams @params)
  {
    try
    {
      var answer = api.Wall.GetComments(@params);
      @params.Offset += answer.Items.Count;
      return (@params.Offset >= answer.CurrentLevelCount, @params, answer.Items, api);
    }
    catch (Exception)
    {
      return (true, @params, Empty<Comment>(), api);
    }
  }

  /// <summary>
  /// Метод возвращает пакет комментариев для поста + запрос для получения следующего по порядку пакета
  /// </summary>
  private static (bool IsLast, WallGetCommentsParams Params, IEnumerable<Comment> Comments, VkApi Api)
    CommentsEnumerator(this VkApi api, NewsItem post, long? commentId = null) => api.CommentsEnumerator(
    new WallGetCommentsParams
    {
      OwnerId = post.SourceId,
      PostId = (long) post.PostId!.Value,
      Count = 100,
      Offset = 0,
      ThreadItemsCount = 10,
      CommentId = commentId
    });
}