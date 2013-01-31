using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpotMute.View
{
    public partial class AddItemDialog : Form
    {
        public const int RESULT_ARTIST = 1;
        public const int RESULT_SONG = 2;
        public const int RESULT_NONE = -1;
        public String song { get; set; }
        public String artist { get; set; }
        public int resultType { get; set; }

        public AddItemDialog()
        {
            InitializeComponent();
            song = "";
            artist = "";
            resultType = RESULT_NONE;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (songTextBox.Text.Length == 0 && artistTextBox.Text.Length > 0)
            {
                artist = artistTextBox.Text;
                resultType = RESULT_ARTIST;
                this.Close();
            }
            else if (songTextBox.Text.Length > 0 && artistTextBox.Text.Length > 0)
            {
                artist = artistTextBox.Text;
                song = songTextBox.Text;
                resultType = RESULT_SONG;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error - Invalid song/artist specified.", "SpotMute - Error");
            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
