using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;

internal static class Comments
{
    internal static VkApi CleanMyComments(this VkApi api)
    {
        foreach (var comment in api.ReadAllMyComments())
        {
            WriteLine(comment);
            ReadKey(true);
        }
        return api;
    }

    private static IEnumerable<Comment> ReadAllMyComments(this VkApi api)
    {
        var commentsReader = new CommentsReader(api);
        for (var commentsPack = commentsReader.NextCommentsPackOrNull();
            commentsPack != null;
            commentsPack = commentsReader.NextCommentsPackOrNull())
        {
            foreach (var comment in commentsPack)
            {
                if (comment.OwnerId == api.UserId)
                {
                    yield return comment;
                }
            }
        }
    }

    private class CommentsReader
    {
        private string? _nextFrom;
        private readonly VkApi _api;
        private IEnumerator<NewsItem> _postEnumerator;

        public CommentsReader(VkApi api)
        {
            _api = api;
            _postEnumerator = NextPostEnumerator();
        }

        private IEnumerator<NewsItem> NextPostEnumerator()
        {
            var answer = _api.NewsFeed.Get(new NewsFeedGetParams
            {
                Count = 100,
                StartFrom = _nextFrom
            });
            _nextFrom = answer.NextFrom;
            return answer.Items.GetEnumerator();
        }

        private NewsItem? NextPost()
        {
            if (_postEnumerator.MoveNext())
            {
                return _postEnumerator.Current;
            }
            _postEnumerator = NextPostEnumerator();
            return _postEnumerator.MoveNext() ? _postEnumerator.Current : null;
        }

        internal ReadOnlyCollection<Comment>? NextCommentsPackOrNull() => NextPost()?.Comments.List;
    }
}