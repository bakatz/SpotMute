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
        private Dictionary<String, Dictionary<String, Boolean>> dict; // Key: Artist, Value: [Key: Song title - string, Value: dummy - bool] -- value is a dummy because all we need is the song title & we want O(1) lookup.
        private String persistFilePath;
        private int count; // we need to keep our own size because we want the total # of entries, not just # of key:value pairs
        public BlockTable() // create new blank blockTable
        {
            dict = new Dictionary<String, Dictionary<String, Boolean>>();
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
                            addArtist(args[1]);
                        }
                        else if (args.Length == 3 && args[0][0] == 'S') // song block
                        {
                            addSong(args[1], args[2]);
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
        public void addSong(String artistName, String song)
        {
            if (artistName == null || song == null || artistName.Length == 0 || song.Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist or song specified.");
            }
            else
            {
                if (dict.ContainsKey(artistName) && dict[artistName] == null) return; // artist's songs are all blocked, ignore and return

                if (!dict.ContainsKey(artistName))
                {
                    dict.Add(artistName, new Dictionary<String, Boolean>());
                }
                dict[artistName].Add(song, true);
                count++;
            }
        }

        /*
         * Removes song by artistName from the blockTable.
         */
        public void removeSong(String artistName, String song)
        {
            if (artistName == null || song == null || artistName.Length == 0 || song.Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist or song specified.");
            }
            else if (dict.ContainsKey(artistName)) 
            {
                if (dict[artistName] == null) return; // can't remove one song from a globally blocked artist.
                dict[artistName].Remove(song);
                count--;
            }
        }

        /*
         * Adds all songs by artistName to the blockTable. Full block.
         */
        public void addArtist(String artistName)
        {
            if (artistName == null || artistName.Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist name specified.");
            }
            else if (dict.ContainsKey(artistName) && dict[artistName] != null)
            {
                dict[artistName] = null;
            }
            else if (!dict.ContainsKey(artistName))
            {
                dict.Add(artistName, null);
                count++;
            }
        }

        /*
         * Removes artistName from the blockTable and all of its associated songs.
         */
        public void removeArtist(String artistName)
        {
            if (artistName == null || artistName.Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist name specified.");
            } 
            else if(dict.ContainsKey(artistName))
            {
                if (dict[artistName] != null)
                    count -= dict[artistName].Count; // subtract the number of songs by artist
                else
                    count--; // if we blocked all the songs by the artist, just subtract 1 (for the artist itself)
                dict.Remove(artistName);
            }
        }

        /*
         * Just a shortcut to remove an old song->add a new song 
         */
        public void updateSong(String artistName, String oldSong, String newSong)
        {
            removeSong(artistName, oldSong);
            addSong(artistName, newSong);
        }

        /*
         * Just a shortcut to remove an old artist->add a new artist
         */
        public void updateArtist(String oldArtistName, String newArtistName)
        {
            removeArtist(oldArtistName);
            addArtist(newArtistName);
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
        public Boolean contains(String artistName, String song)
        {
            if (artistName == null || song == null)
            {
                return false;
            }
            return dict.ContainsKey(artistName) && (dict[artistName] == null || dict[artistName].ContainsKey(song)); // true if the artistname is in the dict, and we either have an artist full block or the artist's dictionary object contains the song...
        }

        /*
         * Convert our object to a string representation so that it can be serialized to a file.
         */
        public override String ToString()
        {
            StringBuilder str = new StringBuilder(); // used simply to save memory - no need to += a string and allocate new mem each time
            foreach (String artistName in dict.Keys)
            {
                if (dict[artistName] == null) // output artist line
                {
                    str.AppendLine("A|" + artistName);
                }
                else // output song line
                {
                    foreach (String songName in dict[artistName].Keys)
                    {
                        str.AppendLine("S|" + artistName + "|" + songName);
                    }
                }
            }
            return str.ToString();
        }

        /*
         * TODO: don't return the underlying data structure here. Return list representation.
         */
        public List<KeyValuePair<String, Dictionary<String, Boolean>>> toList()
        {
            return dict.ToList();
        }
    }
}
