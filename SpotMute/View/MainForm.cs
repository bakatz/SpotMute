using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreAudioApi;
using System.Diagnostics;
using SpotMute.Controller;
using SpotMute.View;

namespace SpotMute
{
    /*
     * Defines the Main form for SpotMute. Displays various options to blacklist the current song or artist, and passes appropriate requests to SpotifyController
     */
    public partial class MainForm : Form
    {
        private SpotifyController spotControl;
        private void MainForm_Load(object sender, EventArgs e)
        {
            spotControl = new SpotifyController(consoleBox, nowPlayingLabel); // TODO: awkward to pass the console object and label to Spotify controller; kind of unrelated.
            stopButton.Enabled = false;
            blacklistArtistButton.Enabled = false;
            blacklistSongButton.Enabled = false;
        }

        /*
         * MainForm_FormClosing - when the user closes the application, save the blacklist to a file so it can be reloaded when the program re-opens. Also save the logs for easy viewing.
         */
        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            spotControl.addLog("Got FormClosing event in MainForm view. Trying to persist blacklist for use next time...");
            
            spotControl.persistBlockTable();
            spotControl.persistLogs();

            spotControl.addLog("Shutting down.");
            spotControl.stopListening();
        }


        /*
         * Required init for Windows forms. 
         */
        public MainForm()
        {
            InitializeComponent();
        }

        /*
         * stopButton_Click - when the 'stop' button is clicked, reset all of our buttons and labels to their original states.
         */
        private void stopButton_Click(object sender, EventArgs e)
        {
            spotControl.stopListening();
            stopButton.Enabled = false;
            startButton.Enabled = true;
            blacklistArtistButton.Enabled = false;
            blacklistSongButton.Enabled = false;
            this.Text = "SpotMute - Idle";
            nowPlayingLabel.Text = "N/A";
        }

        /*
        * startButton_Click - when the 'start' button is clicked, change our button/label state after starting the listener if success occurred.
        */
        private void startButton_Click(object sender, EventArgs e)
        {
            if (spotControl.startListening())
            {
                stopButton.Enabled = true; //success case
                startButton.Enabled = false;
                blacklistArtistButton.Enabled = true;
                blacklistSongButton.Enabled = true;
                this.Text = "SpotMute - Listening";
            }
            else
            {
                MessageBox.Show(this, "ERROR: Spotify doesn't appear to be running. Start Spotify and then click the Start button again.", "SpotMute - Error");
            }
        }

        /*
        * Button to blacklist the current song by the current artist.
        */
        private void blacklistSongButton_Click(object sender, EventArgs e)
        {
            spotControl.blockCurrentSong();
        }

        /*
        * Button to blacklist all of the songs by the current artist.
        */
        private void blacklistArtistButton_Click(object sender, EventArgs e)
        {
            spotControl.blockCurrentArtist();
        }

        /*
         * Toggles the visiblity of the log textarea - "console"
         */
        private void showLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consoleBox.Visible = !consoleBox.Visible;
            showLogToolStripMenuItem.Checked = !showLogToolStripMenuItem.Checked;
        }

        /*
         * The below 2 funtions display an about menu and usage menu for the application. TODO: extend to contain rich text.
         */
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "SpotMute v0.2 - a simple blacklist and volume control application for Spotify\n\nAuthor: Ben Katz (bakatz@vt.edu)\nBug tracker: https://github.com/bakatz/SpotifyAdMuter/issues\n\nPayPal donations as well as feature suggestions are appreciated and accepted at the above email. Thanks for using SpotMute!", "About SpotMute");
        }

        
        private void usageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Instructions:\n\n1. Click start.\n2. Verify that SpotMute has detected the current song.\n3. Click 'block artist' to block all songs by the"+
            "current artist, or 'block song'to block the current song. Elevator music will be played over the song in question.\n\nNote: pressing stop will reset your volume to what it was before the mute operation.", "SpotMute Usage");
        }

        /*
         * TODO: save blacklists to server? retreive blacklists FROM server? determine.
         */
        private void saveToServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Unimplemented functionality.", "SpotMute Save to Server");
        }

        /*
         * Displays a very simple form showing the contents of the blacklist. TODO: add some options to remove blacklist items here. TODO: fix song titles showing as 'Collection'
         */
        private void blacklistContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BlockTableForm blForm = new BlockTableForm(spotControl);
            blForm.Show();
        }
    }
}
