using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SpotMute.Controller;
using CoreAudioApi;
using System.Threading;

namespace SpotMute.Model
{
    
    /*
     * A class to hold information retreived from spotify. 
     */
    class SpotifyInformation
    {
        private String lastItem;
        private Song currSong;
        private SpotifyController controller;
        private AudioSessionControl spotifyASC;

       // private static Mutex modifySongMutex = new Mutex();
       // private static object modifySongLock;

        private Process spotProc;

        public SpotifyInformation(SpotifyController controller) // our constructor has a controller reference so we can retreive information from the current audio session.
        {
            this.controller = controller;
        }

        /*
         * Returns the current song from the window title of spotify.
         */
        public Song getCurrentSong()
        {
            updateNowPlaying();
            return currSong;

        }

        /*
         * Returns Spotify's current audio session. Used to modify the scalar volume level.
         */
        public AudioSessionControl getSpotifyAudioSession()
        {
            return spotifyASC;
        }

        /*
         * Retreives the current 'now playing' item by parsing Spotify's window title.
         */
        public void updateNowPlaying()
        {
            if (!controller.isListening()) return;
            Process[] procs = Process.GetProcessesByName("spotify"); // we need to refresh the spotify process each time, because spotify tends to change process id when hiding/unhiding.
            if (procs.Length > 0)
            {
                spotProc = procs[0];
                spotifyASC = controller.getSpotifyAudioSession(spotProc.Id);

                if (spotifyASC == null || spotProc.MainWindowTitle.Equals(lastItem)) // if we couldn't open an audio session or the song is the same as the last one, ignore
                {
                    return;
                }
                String[] theItem = spotProc.MainWindowTitle.Split('–'); // this is a 'long' dash (– vs. -)
                if (theItem.Length >= 2)
                {
                    String title = theItem[1].Trim();
                    String artist = theItem[0].Remove(0, 10).Trim(); // remove the prefix: "Spotify - "
                    controller.addLog("Artist: " + artist + ", Title: " + theItem[1]);
                    currSong = new Song(artist, title);
                    lastItem = spotProc.MainWindowTitle;
                }
                else
                {
                    currSong = null;
                    controller.addLog("WARN: couldn't parse the artist/title from the raw window title.");
                }
            }
        }
    }
}
