using System;
using System.Linq;
using System.Threading;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using static System.Console;
using static VkNet.Enums.SafetyEnums.LikeObjectType;

internal static class Likes
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
        }

        WriteLine("Завершенно..");

        WriteLine("Удаляем лайки с постов через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikePosts; i++)
        {
            WriteLine("Удалили лайк с " + RemoveLikePost(api, getPosts, i));
        }

        WriteLine("Завершенно..");

        WriteLine("Удаляем лайки с видео через 1 секунду");
        Thread.Sleep(1000);

        for (int i = 0; i < countLikeVideos; i++)
        {
            WriteLine("Удалили лайк с " + RemoveLikeVideo(api, getVideos, i));
        }

        return api;
    }

    private static string RemoveLikeVideo(VkApi api, FaveVideoEx getVideos, int number)
    {
        var delete = api.Likes.Delete(LikeObjectType.Video, (long) getVideos.Videos[number].Id,
            getVideos.Videos[number].OwnerId);
        return "Удалён лайк с id" + getVideos.Videos[number].Id;
    }

    static string RemoveLikePost(VkApi api, WallGetObject getPosts, int number)
    {
        var delete = api.Likes.Delete(LikeObjectType.Post, (long) getPosts.WallPosts[number].Id,
            getPosts.WallPosts[number].OwnerId);
        return "Удалён лайк с id" + getPosts.WallPosts[number].Id;
    }

    // Пример как избегать предупреждений
    static string RemoveLikePhotos(VkApi api, VkCollection<Photo> getPhotos, int number)
    {
        if (getPhotos[number].Id is var photoId &&
            photoId.HasValue &&
            getPhotos[number].OwnerId is var ownerId &&
            ownerId.HasValue)
        {
            var delete = api.Likes.Delete(LikeObjectType.Photo, photoId.Value, ownerId.Value);
            // TODO лучше возвращать ссылку а не id т к id пользователю ничего не говорит
            return $"Удалён лайк с {photoId}";
        }
        // TODO лучше все сообщения возвращать на английском
        return "Ошибка удаления лайка";
    }
}