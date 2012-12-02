using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpotMute.Model;
using SpotMute.Controller;

namespace SpotMute.View
{
    /*
     * Simple class showing the contents of the blacklist.
     */
    public partial class BlockTableForm : Form
    {
        private SpotifyController spotControl;
        public BlockTableForm(SpotifyController spotControl)
        {
            this.spotControl = spotControl;
            
            InitializeComponent();
        }

        /*
         * Convert our blacklist to a list. DataGridView only supports lists and arrays. Listener for DataGridView will automatically fire and display the list.
         */
        private void BlackListForm_Load(object sender, EventArgs e)
        {
            List<KeyValuePair<String, Dictionary<String, Boolean>>> list = spotControl.getBlockTable().toList();
            for (int i = 0; i < list.Count; i++)
            {
                String artistName = list[i].Key;
                Dictionary<String, Boolean> currSongs = list[i].Value;
                if (currSongs != null && currSongs.Keys.Count > 0)
                {
                    foreach (String songTitle in currSongs.Keys)
                    {
                        blockedListBox.Items.Add(new BlockedSong(artistName, songTitle));
                    }
                }
                else
                {
                    blockedListBox.Items.Add(new BlockedArtist(artistName));
                }
            }
        }

        private void removeArtistButton_Click(object sender, EventArgs e)
        {
            if (blockedListBox.SelectedIndices.Count == 0) return;
            foreach (int index in blockedListBox.SelectedIndices)
            {
                ListItem currSelectedSong = (ListItem)blockedListBox.Items[index];
                spotControl.getBlockTable().removeArtist(currSelectedSong.getArtistName());
                blockedListBox.Items.RemoveAt(index);
                spotControl.checkCurrentSong();
            }

            
            MessageBox.Show(this, "Removed selected artist(s) successfully.", "SpotMute - Remove");
        }

        private void removeSongButton_Click(object sender, EventArgs e)
        {
            Boolean gotError = false;
            if (blockedListBox.SelectedIndices.Count == 0) return;
            foreach (int index in blockedListBox.SelectedIndices)
            {
                ListItem currSelectedSong = (ListItem)blockedListBox.Items[index];
                if (currSelectedSong.getSongTitle() == null)
                {
                    spotControl.addLog("Error - can't remove song. Reason: " + currSelectedSong.getArtistName() + " is blocked globally. Use the 'remove artist' button.");
                    gotError = true;
                    continue;
                }
                spotControl.getBlockTable().removeSong(currSelectedSong.getArtistName(), currSelectedSong.getSongTitle());
                blockedListBox.Items.RemoveAt(index);
                spotControl.checkCurrentSong();
            }

            if (!gotError)
            {
                MessageBox.Show(this, "Removed selected song(s) successfully.", "SpotMute - Remove");
            }
            else
            {
                MessageBox.Show(this, "One or more of the selected songs could not be removed. Check the log for details.", "SpotMute - Warning");
            }
        }
    }
}
