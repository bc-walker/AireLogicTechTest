using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AireLogicTechTest
{
    public class ArtistList
    {
        public string artistName { get; set; }
        public int aveWordCount { get; set; }
    }

    public class ArtistSongs
    {
        public int artistId { get; set; }
        public string artistName { get; set; }
        public int albumId { get; set; }
        public string AlbumName { get; set; }
        public int songId { get; set; }
        public string songName { get; set; }
        public int wordCount { get; set; }
    }
    public class Track
    {
        [JsonProperty("track")]
        public TrackAttributes track { get; set; }
    }
    public class TrackAttributes
    {
        public int track_id { get; set; }
        public string track_name { get; set; }
        public TrackNameTranslationList[] track_name_translation_list { get; set; }
        public int track_rating { get; set; }
        public int commontrack_id { get; set; }
        public int instrumental { get; set; }
        [JsonProperty("explicit")]
        public int _explicit { get; set; }
        public int has_lyrics { get; set; }
        public int has_subtitles { get; set; }
        public int has_richsync { get; set; }
        public int num_favourite { get; set; }
        public int album_id { get; set; }
        public string album_name { get; set; }
        public int artist_id { get; set; }
        public string artist_name { get; set; }
        public string track_share_url { get; set; }
        public string track_edit_url { get; set; }
        public int restricted { get; set; }
        public DateTime? updated_time { get; set; }
        public MusicGenreList primary_genres { get; set; }
    }
    public class TrackNameTranslationList
{

}
    public class MusicGenreList
    {
        public MusicGenre[] music_genre_list { get; set; }
    }
    public class MusicGenre
    {
        public int music_genre_id { get; set; }
        public int music_genre_parent_id { get; set; }
        public string music_genre_name { get; set; }
        public string music_genre_name_extended { get; set; }
        public string music_genre_vanity { get; set; }
    }

    public class Artist
    {
        public List<ArtistList> artistDataList = new List<ArtistList>();

        public string songListJSON = string.Empty;
        public List<ArtistSongs> GetArtistSongList(string artistName)
        {
            Task.Run(() => GetSongListJSON(artistName)).Wait();
            while (string.IsNullOrEmpty(songListJSON))
            {
                Thread.Sleep(1000);
            }
            var artistSongList = ParseSongListJSON();
            return artistSongList;
        }

        public async Task GetSongListJSON(string artistName)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api.musixmatch.com/ws/1.1/track.search?format=json&callback=callback&q_artist={artistName}&quorum_factor=1&apikey=8c7047146c560186fdcddb57fff2991f");
            songListJSON = await response.Content.ReadAsStringAsync();
        }

        public List<ArtistSongs> ParseSongListJSON()
        {
            var artistSongs = new List<ArtistSongs>();
            var jsonObject = JsonConvert.DeserializeObject<JObject>(songListJSON);
            var jToken = jsonObject.GetValue("message");
            var json = jToken["body"];
            var trackListStr = json["track_list"].ToString().Replace("explicit", "_explicit");
            List<Track> track_list = JsonConvert.DeserializeObject<List<Track>>(trackListStr);
            foreach (Track track in track_list)
            {
                artistSongs.Add(new ArtistSongs()
                {
                    artistId = track.track.artist_id,
                    artistName = track.track.artist_name,
                    albumId = track.track.album_id,
                    AlbumName = track.track.album_name,
                    songId = track.track.track_id,
                    songName = track.track.track_name,
                    wordCount = 0
                    //await new Lyrics().GetLyricCountAsync(track.track.artist_name, track.track.track_name)
                });
            }
            return artistSongs;
        }
    }
}
