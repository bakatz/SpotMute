using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    /*
     * Represents an Artist in Spotify. TODO: change blocktable such that Artists "have many" songs instead of creating an inner hashtable in BlockTable
     */
    public class Artist : BlockableItemBase
    {
        public Artist(String artistName)
        {
            ArtistName = artistName;
            //Artist
        }

        public override String ToString()
        {
            return "[Artist] " + ArtistName;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + ArtistName.GetHashCode();
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
            return ArtistName.Equals(p.ArtistName, StringComparison.CurrentCultureIgnoreCase);//.Equals(p.ArtistName);
        }
    }
}
