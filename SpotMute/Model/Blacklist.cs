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
    public class BlackList
    {
        private Dictionary<String, Dictionary<String, Boolean>> dict; // Key: Artist, Value: [Key: Song title - string, Value: dummy - bool] -- value is a dummy because all we need is the song title & we want O(1) lookup.
        private String persistFilePath;
        private int count; // we need to keep our own size because we want the total # of entries, not just # of key:value pairs
        public BlackList() // create new blank blacklist
        {
            dict = new Dictionary<String, Dictionary<String, Boolean>>();
            count = 0;
        }

        public BlackList(String filePath) : this() // call blank constructor, then try to add songs/artists from the blacklist conf file. Format example: S|artistnamehere|songnamehere or A|artistnamehere
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
                    //System.Windows.Forms.MessageBox.Show(ToString());
                }
                else
                {
                    File.Create(filePath);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Got exception while instantiating blacklist with filepath '" + filePath + "': " + e);
            }
        }

        /*
         * Adds song by artistName to the blacklist. Single block. 
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
         * Adds all songs by artistName to the blacklist. Full block.
         */
        public void addArtist(String artistName)
        {
            if (artistName == null || artistName.Length == 0)
            {
                throw new ArgumentException("Null or zero-length artist name specified.");
            }

            if (!dict.ContainsKey(artistName))
            {
                dict.Add(artistName, null);
                count++;
            }
        }


        /*
         * Returns the number of songs and artists in the blacklist. 
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
                Console.WriteLine("Got exception while saving blacklist with filepath '" + persistFilePath + "': " + e);
            }
        }

        /*
         * Does the blacklist contain song by artistName? 
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
        public Dictionary<String, Dictionary<String, Boolean>> getDictionary()
        {
            return dict;
        }
    }
}
