using System;
using System.Threading;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Utils;
using static System.Console;

//TODO Доделать вывод ссылок
internal static class LikesCleaner
{
    [Obsolete]
    internal static VkApi CleanLikes(this VkApi api)
    {
        var getPhotos = api.Fave.GetPhotos();
        var getPosts = api.Fave.GetPosts();
        var getVideos = api.Fave.GetVideos();

        int countLikePhotos = getPhotos.Count;
        int countLikePosts = getPosts.WallPosts.Count;
        int countLikeVideos = Convert.ToInt32(getVideos.Count);


        WriteLine("Колличество \"поравившихся\" фото: " + countLikePhotos);
        WriteLine("Колличество \"поравившихся\" постов: " + countLikePosts);
        WriteLine("Колличество \"поравившихся\" видео: " + countLikeVideos);

        WriteLine("Удаляем лайки с фото через 1 секунду");
        Thread.Sleep(1000);


        for (int i = 0; i < countLikePhotos; i++)
        {
            WriteLine("Удалили лайк с " + RemoveLikePhotos(api, getPhotos, i));
            Thread.Sleep(5000);//Попытка защиты от Капчи
        }

        WriteLine("Завершенно..");

        WriteLine("Удаляем лайки с постов через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikePosts; i++)
        {
            WriteLine("Удалили лайк с " + RemoveLikePost(api, getPosts, i));
            Thread.Sleep(5000);//Попытка защиты от Капчи
        }

        WriteLine("Завершенно..");

        WriteLine("Удаляем лайки с видео через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikeVideos; i++)
        {
            WriteLine("Удалили лайк с " + RemoveLikeVideo(api, getVideos, i));
            Thread.Sleep(5000);//Попытка защиты от Капчи
        }

        return api;
    }

    private static string RemoveLikeVideo(VkApi api, FaveVideoEx getVideos, int number)
    {
        var urlVideos = getVideos.Videos[number].UploadUrl;
        var videosId = getVideos.Videos[number].Id;
        var ownerId = getVideos.Videos[number].OwnerId;
        if (videosId.HasValue && ownerId.HasValue)
        {
            api.Likes.Delete(LikeObjectType.Video, videosId.Value, ownerId.Value);
            return $"Удалён лайк с {urlVideos}";
        }
        return "Удалён лайк с id" + getVideos.Videos[number].Id;
    }

    static string RemoveLikePost(VkApi api, WallGetObject getPosts, int number)
    {
        var urlPosts = getPosts.WallPosts[number].Text;
        var postsId = getPosts.WallPosts[number].Id;
        var ownerId = getPosts.WallPosts[number].OwnerId;
        if (postsId.HasValue && ownerId.HasValue)
        {
            api.Likes.Delete(LikeObjectType.Post, postsId.Value, ownerId.Value);
            return $"Удалён лайк с {urlPosts}";
        }
        return "Ошибка удаления лайка";
    }

    static string RemoveLikePhotos(VkApi api, VkCollection<Photo> getPhotos, int number)
    {
        var urlPhoto = getPhotos[number].Url;
        var photoId = getPhotos[number].Id;
        var ownerId = getPhotos[number].OwnerId;
        if (photoId.HasValue && ownerId.HasValue)
        {
            api.Likes.Delete(LikeObjectType.Photo, photoId.Value, ownerId.Value);
            return $"Удалён лайк с {urlPhoto}";
        }
        return "Ошибка удаления лайка";
    }
}