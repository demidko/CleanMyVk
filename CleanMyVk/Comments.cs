using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var myComments = i.Comments.List.GrepComments(api.UserId.Value);
            foreach (var myComment in myComments)
            {
                WriteLine(myComment.Text);
            }
            ReadKey(true);
        }
        return api;
    }

    private static IEnumerable<Comment> GrepComments(this ReadOnlyCollection<Comment> comments, long vkId) =>
        comments.Where(x => x.OwnerId == vkId);

    private class CommentsReader
    {
        private readonly PostReader _postReader;
        private readonly VkApi _api;

        public CommentsReader(VkApi api, PostReader postReader)
        {
            _api = api;
            _postReader = postReader;
        }


        private IEnumerator<Comment> NextEnumerator()
        {
            
        }
        
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