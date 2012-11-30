using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CoreAudioApi;
using System.Windows.Forms;
using SpotMute.Model;
using System.Media;
using System.Threading;
using System.IO;
using WMPLib;

namespace SpotMute.Controller
{
    /*
     * Checks the Spotify window for title updates, mutes songs appropriately, and receives update requests to update the model/view.
     */
    public class SpotifyController
    {
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, 
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        //TODO: put these in some external configuration file
        private const String BLOCKTABLE_FILE_PATH = "blocktable.conf"; // TODO: should move to blocktable if decision is made to remove/modify 2nd constructor from blocktable
        private String REPLACEMENT_AUDIO_PATH = "elevator.mp3";
        private const String LOG_FILE_PATH = "log.txt";

        // Constants from winuser.h
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        // End constants from winuser.h


        private IntPtr spotHook;
        private Process spotProc = null;

        private BlockTable blockTable;
        private SpotifyInformation spotInfo;
        private WinEventDelegate procDelegate;
        private MMDevice defaultDevice = null;
        private TextBox console;
        private Label nowPlayingLabel;
        private float savedVol;
        private WindowsMediaPlayer player;


        public SpotifyController(TextBox console, Label nowPlaying)
        {
            this.console = console;
            this.nowPlayingLabel = nowPlaying;
            player = new WindowsMediaPlayer();
            player.URL = REPLACEMENT_AUDIO_PATH;
            player.controls.stop();
            blockTable = new BlockTable(BLOCKTABLE_FILE_PATH);
            spotInfo = new SpotifyInformation(this);
            savedVol = -1;
        }

        /*
         * Makes a Win32 API call to start listening for window title changes in Spotify. Returns true if we got a successful hook. 
         */
        public Boolean startListening()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator(); // get multimedia device enumerator.
            defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia); // grab our default sound device
            procDelegate = new WinEventDelegate(WinEventProc);

            Process[] procs = Process.GetProcessesByName("spotify"); // find the spotify process by name.
            if (procs.Length > 0)
            {
                addLog("Found spotify proc.");
                spotProc = procs[0]; // in the case of multiple spotify processes, just use the first one.
                // Listen for window title changes in the spotify process, using spotify's pid
                spotHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                        procDelegate, (uint)spotProc.Id, 0, WINEVENT_OUTOFCONTEXT);

                if (spotHook != null)
                {
                    addLog("Spotify hook successful. Started.");
                    checkCurrentSong();
                    return true;
                }
                else
                {
                    addLog("ERROR: bad return value from SetWinEventHook(). Abort.");
                }
            }
            return false;
        }

        /*
         * Unhooks the hook obtained from startListening(), resets our spotify volume to the normal volume, and stops any replacement music that might be playing.
         */
        public void stopListening()
        {
            AudioSessionControl spotifyASC = spotInfo.getSpotifyAudioSession();
            if (spotifyASC != null && savedVol != -1)
            {
                spotifyASC.SimpleAudioVolume.MasterVolume = savedVol;
            }

            if (spotHook != null)
            {
                UnhookWinEvent(spotHook);
            }
            stopReplacementMusic();
            addLog("STOP!");
        }

        /*
         * Writes a line to the console. Can be seen by toggling console in the Show menu.
         */
        public void addLog(String line)
        {
            if (console != null)
            {
                console.AppendText(DateTime.Now.ToString() + ": " + line + "\r\n");
            }
        }

        /*
         * Uses CoreAudioApi to get Spotify's Audio session associated with the process id given. 
         */
        public AudioSessionControl getSpotifyAudioSession(int spotProcId)
        {
            for (int i = 0; i < defaultDevice.AudioSessionManager.Sessions.Count; i++) // possible TODO: good spot for binary search if there are large numbers of spotify processes.
            {
                AudioSessionControl currASC = defaultDevice.AudioSessionManager.Sessions[i];
                if (currASC.ProcessID == spotProcId)
                {
                    return currASC;
                }
            }
            return null;
        }

        /*
         * Changes spotify's individual volume to 5% of the master volume for the system, saving the user's previous volume beforehand for restoring later.
         */
        private void forceSpotifyMute()
        {
            AudioSessionControl spotifyASC = spotInfo.getSpotifyAudioSession();
            if (Math.Abs(spotifyASC.SimpleAudioVolume.MasterVolume - (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar)) < 0.0001 && savedVol > 0) // bugfix: muting twice leads to never unmuting even on a whitelisted song
            {
                addLog("Not going to set savedvol, Difference: " + Math.Abs(savedVol - (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar)));
                addLog(savedVol + " vs calc: " + (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                addLog("User's volume is already at 5%, ignoring mute request.");
                return;
            }
            addLog("Got spotify volume before mute: " + spotifyASC.SimpleAudioVolume.MasterVolume);
            savedVol = spotifyASC.SimpleAudioVolume.MasterVolume;
            addLog("Setting spotify volume to: " + (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
            // Spotify will pause entirely if the volume is muted. 5% is the minimum value
            // to be quiet enough to be considered muted and still keep spotify running.

            //TODO: test further percentage values. might be able to push 4, 3, ... , 0.1%
            spotifyASC.SimpleAudioVolume.MasterVolume = 0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        /*
         * Adds Spotify's current song to the blockTable, mutes it, and plays elevator music over it.
         */
        public void blockCurrentSong()
        {
            addLog("Current song added to the blockTable. Muting volume.");
            blockTable.addSong(spotInfo.getCurrentArtist(), spotInfo.getCurrentSong());
            forceSpotifyMute();
            playReplacementMusic();
        }

        /*
         * Adds Spotify's current artist to the blockTable, mutes it, and plays elevator music over it.
         */
        public void blockCurrentArtist()
        {
            addLog("Current artist added to the blockTable. Muting volume.");
            blockTable.addArtist(spotInfo.getCurrentArtist());
            forceSpotifyMute();
            playReplacementMusic();
        }

        /*
        * Called on application exit to save the blockTable to the disk.
        */
        public void persistBlockTable()
        {
            addLog("Saving " + blockTable.size() + " blockTable items to disk.");
            blockTable.save();
        }

        /*
        * Called on application exit to save the blockTable to the disk.
        */
        public void persistLogs()
        {
            addLog("Saving " + console.Text.Length + " bytes of logs to disk.");
            try
            {
                File.WriteAllText(LOG_FILE_PATH, console.Text);
            }
            catch (IOException e)
            {
                Console.WriteLine("Got exception while saving logs with filepath '" + LOG_FILE_PATH + "': " + e);
            }
        }

        /*
        * Returns the blockTable associated with this instance of the controller.
        */
        public BlockTable getBlockTable()
        {
            return blockTable;
        }

        /*
         *  Plays the mp3 at REPLACEMENT_AUDIO_PATH at system volume.
         */
        private void playReplacementMusic()
        {
            addLog("Playing replacement music at: " + REPLACEMENT_AUDIO_PATH);
            player.controls.play();
        }

        /*
        *  Stops the mp3 at REPLACEMENT_AUDIO_PATH.
        */
        private void stopReplacementMusic()
        {
            addLog("Stopping replacement music.");
            player.controls.stop();
        }

        /*
         * Validates that the current song is not in the blockTable. If it is in the blockTable, the song is muted and startReplacementMusic() is called. 
         */
        public void checkCurrentSong()
        {
            String artist = spotInfo.getCurrentArtist();
            String song = spotInfo.getCurrentSong();
            nowPlayingLabel.Text = artist + " - " + song;
            if (!blockTable.contains(artist, song))
            {
                stopReplacementMusic();
                if (savedVol > 0)
                {
                    addLog("Resetting master volume to: " + savedVol);
                    AudioSessionControl spotifyASC = spotInfo.getSpotifyAudioSession();
                    spotifyASC.SimpleAudioVolume.MasterVolume = savedVol;
                }
                addLog("Got new Spotify item: " + spotProc.MainWindowTitle + ". Add to blockTable?");

            }
            else
            {
                addLog("Found an ad in the blockTable: " + (spotProc.MainWindowTitle) + ". Muting for duration of the ad.");
                forceSpotifyMute();
                playReplacementMusic();
            }
        }

        /*
         * Every time the window title is updated for our spotify process, this Win32 event gets fired. See delegate declaration.
         */
        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            checkCurrentSong();
        }
    }


}
