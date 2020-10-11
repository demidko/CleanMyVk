using System.Collections.Generic;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

internal class CommentCollector
{
    private readonly VkApi _api;

    public CommentCollector(VkApi api)
    {
        _api = api;
    }

    public IEnumerable<NewsItem> ListMentions()
    {
        return _api.NewsFeed.GetComments(new NewsFeedGetCommentsParams
        {
            Count = 100,
        }).Items;
    }
}