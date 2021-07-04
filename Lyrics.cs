using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AireLogicTechTest
{
    public class Lyrics
    {
        public async Task<int> GetLyricCountAsync(string artistName, string songName)
        {
            var lyricCount = 0;
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?artist={artistName}&song={songName}");
            var lyricsStr = await response.Content.ReadAsStringAsync();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(lyricsStr);
            var xmlElem = xmlDoc.GetElementsByTagName("Lyric");
            var lyrics = xmlElem.Item(0).InnerText;
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            lyricCount = lyrics.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            return lyricCount;
        }

        // *** NEW METHODS ***
        //public int? songLyricCount = null;
        public async Task<int> GetSongLyricCount(string artistName, string songName)
        {
            var lyricsXML = await GetSongXML(artistName, songName);
            var songLyrics = ParseSongXML(lyricsXML);
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            return songLyrics.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public async Task<string> GetSongXML(string artistName, string songName)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?artist={artistName}&song={songName}");
            var lyricsXML = await response.Content.ReadAsStringAsync();
            return lyricsXML;
        }

        public string ParseSongXML(string lyricsXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(lyricsXML);
            var xmlElem = xmlDoc.GetElementsByTagName("Lyric");
            var lyrics = xmlElem.Item(0).InnerText;
            return lyrics;
        }
    }
}
