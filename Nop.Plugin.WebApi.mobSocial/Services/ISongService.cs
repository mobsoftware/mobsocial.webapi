using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ISongService : IBaseEntityWithPictureService<Song, SongPicture>
    {
        IList<Song> SearchSongs(string Term, int Count = 15, int Page = 1, bool SearchDescriptions = false, bool SearchArtists = false, string ArtistName = "", bool PublishedOnly = true);

        IList<Song> SearchSongs(string Term, out int TotalPages, int Count = 15, int Page = 1, bool SearchDescriptions = false, bool SearchArtists = false, string ArtistName = "", bool PublishedOnly = true);

        Song GetSongByRemoteEntityId(string RemoteEntityId);

        Song GetSongByProductId(int ProductId);
    }
}
