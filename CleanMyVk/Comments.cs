using System.Collections.Generic;
using System.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;

internal static class Comments
{
    internal static VkApi CleanComments(this VkApi api)
    {
        var postReader = new PostReader(api.NewsFeed);
        for (var i = postReader.NextPostOrNull(); i != null; i = postReader.NextPostOrNull())
        {
            WriteLine(i.Type);
            ReadKey(true);
        }
        return api;
    }

    private class PostReader
    {
        private string? _nextFrom;
        private readonly INewsFeedCategory _api;
        private IEnumerator<NewsItem> _enumerator;

        public PostReader(INewsFeedCategory api)
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

        internal NewsItem? NextPostOrNull()
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