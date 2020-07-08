using System;
using System.Linq;
using System.Threading;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

internal static class Likes
{
    [Obsolete]
    internal static VkApi CleanLikes(this VkApi api)
    {
        //testgit
        var getPhotos = api.Fave.GetPhotos();
        var getPosts = api.Fave.GetPosts();
        var getVideos = api.Fave.GetVideos();

        int countLikePhotos = getPhotos.Count;
        int countLikePosts = getPosts.WallPosts.Count;
        int countLikeVideos = Convert.ToInt32(getVideos.Count);


        Console.WriteLine("Колличество \"поравившихся\" фото: " + countLikePhotos);
        Console.WriteLine("Колличество \"поравившихся\" постов: " + countLikePosts);
        Console.WriteLine("Колличество \"поравившихся\" видео: " + countLikeVideos);

        Console.WriteLine("Удаляем лайки с фото через 1 секунду");
        Thread.Sleep(1000);
     

        for (int i = 0; i < countLikePhotos; i++)
        {
            Console.WriteLine("Удалили лайк с " + RemoveLikePhotos(api, getPhotos, i));
        }

        Console.WriteLine("Завершенно..");

        Console.WriteLine("Удаляем лайки с постов через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikePosts; i++)
        {
            Console.WriteLine("Удалили лайк с " + RemoveLikePost(api, getPosts, i));
        }

        Console.WriteLine("Завершенно..");

        Console.WriteLine("Удаляем лайки с видео через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikeVideos; i++)
        {
            Console.WriteLine("Удалили лайк с " + RemoveLikeVideo(api, getVideos, i));
        }

        return api;
    }

    private static string RemoveLikeVideo(VkApi api, FaveVideoEx getVideos, int number)
    {
        var delete = api.Likes.Delete(LikeObjectType.Video, (long)getVideos.Videos[number].Id, getVideos.Videos[number].OwnerId);
        return "Удалён лайк с id" + getVideos.Videos[number].Id;
    }

    static string RemoveLikePost(VkApi api, WallGetObject getPosts, int number)
    {
        var delete = api.Likes.Delete(LikeObjectType.Post, (long)getPosts.WallPosts[number].Id, getPosts.WallPosts[number].OwnerId);
        return "Удалён лайк с id" + getPosts.WallPosts[number].Id;
    }

    static string RemoveLikePhotos(VkApi api, VkCollection<Photo> getPhotos, int number)
    {

        var delete = api.Likes.Delete(LikeObjectType.Photo, (long)getPhotos[number].Id, getPhotos[number].OwnerId);
        return "Удалён лайк с id" + getPhotos[number].Id;
    }
}