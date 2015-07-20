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
        private Process spotProc;

        public SpotifyInformation(SpotifyController controller) // our constructor has a controller reference so we can retreive information from the current audio session.
        {
            this.controller = controller;
        }

        /*
         * Returns the current song from the window title of spotify.
         */
        public Song GetCurrentSong()
        {
            UpdateNowPlaying();
            return currSong;

        }

        /*
         * Returns Spotify's current audio session. Used to modify the scalar volume level.
         */
        public AudioSessionControl GetSpotifyAudioSession()
        {
            return spotifyASC;
        }

        /*
         * Retreives the current 'now playing' item by parsing Spotify's window title.
         */
        public void UpdateNowPlaying()
        {
            if (!controller.isListening()) return;
            Process[] processes = Process.GetProcessesByName("spotify"); // we need to refresh the spotify process each time, because spotify tends to change process id when hiding/unhiding.
            if (processes.Length > 0)
            {

                foreach (Process process in processes)
                {
                    spotProc = process;
                    spotifyASC = controller.getSpotifyAudioSession(spotProc.Id);
                    if (spotifyASC != null)
                    {
                        break;
                    }
                }

                if (spotifyASC == null)
                {
                    controller.addLog("ERROR: couldn't find the necessary spotify process to attach to");
                    return;
                }

                String[] theItem = spotProc.MainWindowTitle.Split('-');
                if (theItem.Length >= 2)
                {
                    String title = theItem[1].Trim();
                    String artist = theItem[0].Trim();
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
            else
            {
                controller.addLog("ERROR: expected to find multiple spotify processes, found one or none");
            }
        }
    }
}
