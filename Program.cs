using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AireLogicTechTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayMainAsync();
        }

        static void DisplayMainAsync()
        {
            var notExit = true;
            do
            {
                Console.Clear();
                Console.WriteLine("------- MAIN MENU -------");
                Console.WriteLine("(Select an option below)");
                Console.WriteLine("");
                Console.WriteLine("1. Enter a single artist.");
                Console.WriteLine("2. Compare multiple artists.");
                Console.WriteLine("3. Exit.");
                var inputValue = Console.ReadKey().Key;
                if (inputValue == ConsoleKey.D1)
                {
                    DisplaySingleArtist().Wait();
                }
                else if (inputValue == ConsoleKey.D2)
                {
                    DisplayMultipleArtists().Wait();
                }
                else if (inputValue == ConsoleKey.D3)
                {
                    notExit = false;
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Please enter valid entry.");
                }
            } while (notExit);
        }

        static async Task DisplaySingleArtist()
        {
            var notExit = true;
            do
            {
                Console.Clear();
            Console.WriteLine("------- SINGLE ARTIST -------");
            Console.WriteLine("");
            Console.WriteLine("* PLEASE NOTE: partial matches will be taken into account *");
            Console.WriteLine("* Enter 'exit' to go back to main menu *");
            Console.WriteLine("Enter artist name:");
            var artist = Console.ReadLine().ToLower().Trim();
                if (artist != "exit")
                {
                    var artistClass = new Artist();
                    Console.WriteLine($"Artist entered: {artist}");
                    var artistSongList = artistClass.GetArtistSongList(artist);
                    foreach (var artistSong in artistSongList)
                    {
                        var lyricClass = new Lyrics();
                        var lyricCount = await lyricClass.GetSongLyricCount(artistSong.artistName, artistSong.songName);
                        artistSong.wordCount = lyricCount;
                    }
                    DisplaySongList(artistSongList);
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to enter another artist");
                    Console.ReadLine();

                }
                else
                {
                    notExit = false;
                }
            } while (notExit);
        }

        static async Task DisplayMultipleArtists()
        {
            var artistList = new List<string>();
            var notExit = true;
            do
            {
                Console.Clear();
                Console.WriteLine("------- MULTIPLE ARTISTS -------");
                Console.WriteLine("");
                Console.WriteLine("* PLEASE NOTE: partial matches will be taken into account *");
                Console.WriteLine("* Enter 'finish' to complete artist entry and start comparing *");
                Console.WriteLine("* Enter 'exit' to go back to main menu *");
                Console.WriteLine("");
                if (artistList.Any())
                {
                    Console.WriteLine("Artists Added");
                    foreach (var addedArtist in artistList.OrderBy(x => x))
                    {
                        Console.WriteLine(addedArtist);
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("Enter artist name (or enter 'finish' to complete entries):");
                var artist = Console.ReadLine().ToLower().Trim();
                if (artist == "exit")
                {
                    notExit = false;
                }
                else if (artist == "finish")
                {
                    var artistDataList = new List<ArtistList>();
                    foreach (var artistName in artistList)
                    {
                        var aveWordCount = 0;
                        var aveSongCount = 0;
                        var artistClass = new Artist();
                        var artistSongList = artistClass.GetArtistSongList(artistName);
                        foreach (var artistSong in artistSongList)
                        {
                            var lyricClass = new Lyrics();
                            var lyricCount = await lyricClass.GetSongLyricCount(artistSong.artistName, artistSong.songName);
                            artistSong.wordCount = lyricCount;
                            if (lyricCount > 0)
                            {
                                aveSongCount++;
                                aveWordCount += lyricCount;
                            }
                        }
                        artistDataList.Add(new ArtistList()
                        {
                            artistName = artistName,
                            aveWordCount = aveWordCount / aveSongCount
                        });
                    }
                    DisplayMultiArtistList(artistDataList);
                    artistList.Clear();
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to enter other artists to compare");
                    Console.ReadLine();
                }
                else
                {
                    artistList.Add(artist);
                }
            } while (notExit);
            DisplayMainAsync();
        }

        static void DisplaySongList(List<ArtistSongs> artistSongList)
        {
            var aveSongCount = 0;
            var aveLyricCount = 0;
            foreach (var artistSong in artistSongList)
            {
                Console.WriteLine($"{artistSong.AlbumName} - {artistSong.songName}: {artistSong.wordCount}");
                if (artistSong.wordCount > 0)
                {
                    aveSongCount++;
                    aveLyricCount += artistSong.wordCount;
                }
            }
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Average Word Count: {aveLyricCount / aveSongCount}");
        }

        static void DisplayMultiArtistList(List<ArtistList> artistDataList)
        {
            foreach (var artistData in artistDataList)
            {
                Console.WriteLine($"{artistData.artistName}         {artistData.aveWordCount}");
            }
        }
    }
}
