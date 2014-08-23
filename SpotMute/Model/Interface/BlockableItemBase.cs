using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    /*
     * Represents an item that can be blocked by this program. Implementing classes: Artist, Song 
     */
    public class BlockableItemBase
    {
        public string ArtistName { get; protected set; }
        public string SongTitle { get; protected set; }
    }
}
