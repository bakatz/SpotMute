using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    class BlockedArtist : ListItem
    {
        private String artistName;
        public BlockedArtist(String artistName)
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
            return "All songs by " + this.artistName;
        }
    }
}
