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
     * Simple class allowing users to edit contents of the blacklist.
     */
    public partial class BlockTableEditorForm : Form
    {
        private SpotifyController spotControl;
        public BlockTableEditorForm(SpotifyController spotControl)
        {
            this.spotControl = spotControl;
            InitializeComponent();
        }

        /*
         * Add all our blacklist items to the listbox.
         */
        private void BlackListForm_Load(object sender, EventArgs e)
        {
            blockedListBox.Items.Clear();
            List<KeyValuePair<Artist, Dictionary<Song, Boolean>>> list = spotControl.getBlockTable().toList();
            for (int i = 0; i < list.Count; i++)
            {
                Artist artist = list[i].Key;
                Dictionary<Song, Boolean> currSongs = list[i].Value;
                if (currSongs != null && currSongs.Keys.Count > 0)
                {
                    foreach (Song song in currSongs.Keys)
                    {
                        blockedListBox.Items.Add(song);
                    }
                }
                else
                {
                    blockedListBox.Items.Add(artist);
                }
            }
        }

        private void removeSongButton_Click(object sender, EventArgs e)
        {
            if (blockedListBox.SelectedItems.Count == 0) return;
            while(blockedListBox.SelectedItems.Count > 0)
            {
                IBlockableItem currSelectedItem = (IBlockableItem)blockedListBox.SelectedItems[0];
                if (currSelectedItem.getSongTitle() == null)
                {
                    Artist currSelectedArtist = new Artist(currSelectedItem.getArtistName());
                    spotControl.getBlockTable().removeArtist(currSelectedArtist);
                }
                else
                {
                    spotControl.getBlockTable().removeSong((Song)currSelectedItem);
                }
                blockedListBox.Items.Remove(currSelectedItem);
                spotControl.checkCurrentSong();
            }

            MessageBox.Show(this, "Removed selected song(s) successfully.", "SpotMute - Remove");
        }

        private void addSongButton_Click(object sender, EventArgs e)
        {
            using (var form = new AddItemDialog())
            {
                form.ShowDialog();
                if (form.resultType == AddItemDialog.RESULT_ARTIST)
                {
                    spotControl.getBlockTable().addArtist(new Artist(form.artist));
                    BlackListForm_Load(sender, e);
                    MessageBox.Show(this, "Added artist successfully.", "SpotMute - Add");
                }
                else if(form.resultType == AddItemDialog.RESULT_SONG)
                {
                    spotControl.getBlockTable().addSong(new Song(form.artist, form.song));
                    BlackListForm_Load(sender, e);
                    MessageBox.Show(this, "Added song successfully.", "SpotMute - Add");
                }
            }
        }
    }
}
