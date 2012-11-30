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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           //TODO: delete option here.
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
            if (blockedListBox.SelectedIndices.Count == 0) return;
            foreach (int index in blockedListBox.SelectedIndices)
            {
                ListItem currSelectedSong = (ListItem)blockedListBox.Items[index];
                spotControl.getBlockTable().removeSong(currSelectedSong.getArtistName(), currSelectedSong.getSongTitle());
                blockedListBox.Items.RemoveAt(index);
                spotControl.checkCurrentSong();
            }

            MessageBox.Show(this, "Removed selected song(s) successfully.", "SpotMute - Remove");
        }
    }
}
