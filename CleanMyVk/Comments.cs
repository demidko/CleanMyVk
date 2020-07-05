using System;
using System.Collections.Generic;
using System.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

internal static class Comments
{
    internal static VkApi CleanComments(this VkApi api)
    {
        var commentReader = new CommentsReader(api.NewsFeed);
        for (var i = commentReader.NextItemOrNull(); i != null; i = commentReader.NextItemOrNull())
        {
            Console.WriteLine(i.Text);
            Console.ReadKey();
        }
        return api;
    }

    private class CommentsReader
    {
        private string? _nextFrom;
        private readonly INewsFeedCategory _api;
        private IEnumerator<NewsItem> _enumerator;

        public CommentsReader(INewsFeedCategory api)
        {
            _api = api;
            _enumerator = NextEnumerator();
        }

        private IEnumerator<NewsItem> NextEnumerator()
        {
            var answer = _api.Get(new NewsFeedGetParams {Count = 100, StartFrom = _nextFrom});
            _nextFrom = answer.NextFrom;
            return answer.Items.GetEnumerator();
        }

        internal NewsItem? NextItemOrNull()
        {
            if (_enumerator.MoveNext())
            {
                return _enumerator.Current;
            }
            _enumerator = NextEnumerator();
            return _enumerator.MoveNext() ? _enumerator.Current : null;
        }
    }
}