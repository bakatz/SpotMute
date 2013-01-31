using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    /*
     * Represents a song that could be played in Spotify. 
     */
    public class Song : IBlockableItem
    {
        private String artistName;
        private String title;
        public Song(String artistName, String title)
        {
            this.artistName = artistName;
            this.title = title;
        }

        public String getArtistName()
        {
            return artistName;
        }

        public String getSongTitle()
        {
            return title;
        }

        public override string ToString()
        {
            return "[Song] " + artistName + " - " + title;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + artistName.GetHashCode();
            hash = (hash * 7) + title.GetHashCode();
            return hash;
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Artist return false.
            Song p = obj as Song;
            if ((Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return artistName.Equals(p.getArtistName()) && title.Equals(p.getSongTitle());
        }
    }
}
