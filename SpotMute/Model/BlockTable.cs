using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace SpotMute.Model
{
    /*
     * A dictionary of blacklisted songs and artists. Each key is an artist name, while each value contains another dictionary of songs for this artist. 
     */
    public class BlockTable
    {
        private Dictionary<Artist, Dictionary<Song, Boolean>> dict; // Key: Artist, Value: [Key: Song title - string, Value: dummy - bool] -- value is a dummy because all we need is the song title & we want O(1) lookup.
        private String persistFilePath;
        private int count; // we need to keep our own size because we want the total # of entries, not just # of key:value pairs
        public BlockTable() // create new blank blockTable
        {
            dict = new Dictionary<Artist, Dictionary<Song, Boolean>>();
            count = 0;
        }

        public BlockTable(String filePath) : this() // call blank constructor, then try to add songs/artists from the blockTable conf file. Format example: S|artistnamehere|songnamehere or A|artistnamehere
        {
            this.persistFilePath = filePath;
            try
            {
                if (File.Exists(filePath))
                {
                    IEnumerable<String> lines = File.ReadLines(filePath);
                    foreach (String line in lines)
                    {
                        String[] args = line.Split('|');
                        if (args.Length == 2 && args[0][0] == 'A') // artist block
                        {
                            addArtist(new Artist(args[1]));
                        }
                        else if (args.Length == 3 && args[0][0] == 'S') // song block
                        {
                            addSong(new Song(args[1], args[2]));
                        }
                    }
                }
                else
                {
                    File.Create(filePath);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Got exception while instantiating blockTable with filepath '" + filePath + "': " + e);
            }
        }

        /*
         * Adds song by artistName to the blockTable. Single block. 
         */
        public void addSong(Song song)
        {
            if (song.getArtistName() == null || song.getSongTitle() == null || song.getArtistName().Length == 0 || song.getSongTitle().Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist or song specified.");
            }
            else
            {
                Artist reqArtist = new Artist(song.getArtistName());
                if (dict.ContainsKey(reqArtist) && dict[reqArtist] == null) return; // artist's songs are all blocked, ignore and return

                if (!dict.ContainsKey(reqArtist))
                {
                    dict.Add(reqArtist, new Dictionary<Song, Boolean>());
                }
                dict[reqArtist].Add(song, true);
                count++;
            }
        }

        /*
         * Removes song by artistName from the blockTable.
         */
        public void removeSong(Song song)
        {
            if (song.getArtistName() == null || song.getSongTitle() == null || song.getArtistName().Length == 0 || song.getSongTitle().Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist or song specified.");
            }
            Artist reqArtist = new Artist(song.getArtistName());
            if (dict.ContainsKey(reqArtist)) 
            {
                if (dict[reqArtist] == null) return; // can't remove one song from a globally blocked artist.
                dict[reqArtist].Remove(song);
                count--;
            }
        }

        /*
         * Adds all songs by artistName to the blockTable. Full block.
         */
        public void addArtist(Artist artist)
        {
            if (artist.getArtistName() == null || artist.getArtistName().Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist name specified.");
            }
            else if (dict.ContainsKey(artist) && dict[artist] != null)
            {
                count = count - dict[artist].Count + 1; // remove all the songs from the count, but add 1 for the new artist block
                dict[artist] = null;
            }
            else if (!dict.ContainsKey(artist))
            {
                dict.Add(artist, null);
                count++;
            }
        }

        /*
         * Removes artistName from the blockTable and all of its associated songs.
         */
        public void removeArtist(Artist artist)
        {
            if (artist == null || artist.getArtistName().Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist name specified.");
            } 
            else if(dict.ContainsKey(artist))
            {
                if (dict[artist] != null)
                    count -= dict[artist].Count; // subtract the number of songs by artist
                else
                    count--; // if we blocked all the songs by the artist, just subtract 1 (for the artist itself)
                dict.Remove(artist);
            }
        }

        /*
         * Just a shortcut to remove an old song->add a new song 
         */
        public void updateSong(Song oldSong, Song newSong)
        {
            removeSong(oldSong);
            addSong(newSong);
        }

        /*
         * Just a shortcut to remove an old artist->add a new artist
         */
        public void updateArtist(Artist oldArtist, Artist newArtist)
        {
            removeArtist(oldArtist);
            addArtist(newArtist);
        }


        /*
         * Returns the number of songs and artists in the blockTable. 
         */
        public int size()
        {
            return count;
        }

        /*
         * Tries to save the string representation of this object to persistFilePath.
         */
        public void save()
        {
            try
            {
                File.WriteAllText(persistFilePath, ToString());
            }
            catch (IOException e)
            {
                Console.WriteLine("Got exception while saving blockTable with filepath '" + persistFilePath + "': " + e);
            }
        }

        /*
         * Does the blockTable contain song by artistName? 
         */
        public Boolean contains(Song song)
        {
            if (song == null || song.getArtistName() == null || song.getSongTitle() == null)
            {
                return false;
            }
            Artist reqArtist = new Artist(song.getArtistName());
            return dict.ContainsKey(reqArtist) && (dict[reqArtist] == null || dict[reqArtist].ContainsKey(song)); // true if the artistname is in the dict, and we either have an artist full block or the artist's dictionary object contains the song...
        }

        /*
         * Convert our object to a string representation so that it can be serialized to a file.
         */
        public override String ToString()
        {
            StringBuilder str = new StringBuilder(); // used simply to save memory - no need to += a string and allocate new mem each time
            foreach (Artist artist in dict.Keys)
            {
                if (dict[artist] == null) // output artist line
                {
                    str.AppendLine("A|" + artist.getArtistName());
                }
                else // output song line
                {
                    foreach (Song song in dict[artist].Keys)
                    {
                        str.AppendLine("S|" + song.getArtistName() + "|" + song.getSongTitle());
                    }
                }
            }
            return str.ToString();
        }

        public List<KeyValuePair<Artist, Dictionary<Song, Boolean>>> toList()
        {
            return dict.ToList();
        }
    }
}
