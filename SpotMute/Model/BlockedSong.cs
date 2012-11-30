using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotMute.Model
{
    class BlockedSong : ListItem
    {
        private String artistName;
        private String title;
        public BlockedSong(String artistName, String title)
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
            return artistName + " - " + title;
        }
    }
}
