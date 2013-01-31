using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    /*
     * Represents an Artist in Spotify. TODO: change blocktable such that Artists "have many" songs instead of creating an inner hashtable in BlockTable
     */
    public class Artist : IBlockableItem
    {
        private String artistName;
        public Artist(String artistName)
        {
            this.artistName = artistName;
        }
        public String getSongTitle()
        {
            return null;
        }

        public String getArtistName()
        {
            return this.artistName;
        }

        public override String ToString()
        {
            return "[Artist] " + this.artistName;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + artistName.GetHashCode();
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
            Artist p = obj as Artist;
            if ((Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return artistName.Equals(p.getArtistName());
        }
    }
}
