using VkNet;
using VkNet.Model;
using VkNet.Utils;

internal class CommentCollector
{
    private readonly VkApi _api;

    public CommentCollector(VkApi api)
    {
        _api = api;
    }

    public VkCollection<Mention> ListMentions()
    {
        return _api.NewsFeed.GetMentions();
    }
}