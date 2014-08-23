using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    /*
     * Represents a song that could be played in Spotify. 
     */
    public class Song : BlockableItemBase
    {
        public Song(String artistName, String title)
        {
            ArtistName = artistName;
            SongTitle = title;
        }

        public override string ToString()
        {
            return "[Song] " + ArtistName + " - " + SongTitle;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + ArtistName.GetHashCode();
            hash = (hash * 7) + SongTitle.GetHashCode();
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
            return ArtistName.Equals(p.ArtistName, StringComparison.CurrentCultureIgnoreCase) && SongTitle.Equals(p.SongTitle, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
