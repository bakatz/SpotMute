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

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

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

        // Set volume to 2% to simulate a "mute"
        private const float VOLUME_SCALED_PCT = 0.02F;

        // Enable logging?
        private const bool ENABLE_LOGGING = true;


        private IntPtr spotHook;
        private Process spotProc = null;

        private BlockTable blockTable;
        private SpotifyInformation spotInfo;
        private WinEventDelegate procDelegate;
        private MMDevice defaultDevice = null;
        private Label nowPlayingLabel;
        private ToolStripMenuItem playElevatorMusic;
        private float savedVol;
        private WindowsMediaPlayer player;

        private Boolean isRunning;
        private Boolean isMusicPlaying;
        private StringBuilder logs;

        public SpotifyController(Label nowPlaying, ToolStripMenuItem playElevatorMusic)
        {
            this.playElevatorMusic = playElevatorMusic;
            this.nowPlayingLabel = nowPlaying;
            this.logs = new StringBuilder();
            isRunning = false;
            player = new WindowsMediaPlayer();
            player.URL = REPLACEMENT_AUDIO_PATH;
            player.controls.stop();
            blockTable = new BlockTable(BLOCKTABLE_FILE_PATH);
            spotInfo = new SpotifyInformation(this);
            savedVol = -1;
        }

        public Boolean isListening()
        {
            return isRunning;
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
                    isRunning = true;
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
            isRunning = false;
        }

        /*
         * Writes a line to the console. Can be seen by toggling console in the Show menu.
         */
        public void addLog(String line)
        {
            if (ENABLE_LOGGING)
            {
                String currTime = DateTime.Now.ToString();
                String outputLine = currTime + ": " + line;
                logs.AppendLine(outputLine);
                Console.WriteLine(outputLine);
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
         * Changes spotify's individual volume to a percentage of the master volume for the system, saving the user's previous volume beforehand for restoring later.
         * Returns false if the volume was already muted, otherwise true
         */
        private Boolean forceSpotifyMute()
        {
            AudioSessionControl spotifyASC = spotInfo.getSpotifyAudioSession();
            if (Math.Abs(spotifyASC.SimpleAudioVolume.MasterVolume - (VOLUME_SCALED_PCT / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar)) < 0.0001 && savedVol > 0) // bugfix: muting twice leads to never unmuting even on a whitelisted song
            {
                addLog("User's volume is already at 5%, will not try to change vol.");
                return false;
            }

            addLog("Got spotify volume before mute: " + spotifyASC.SimpleAudioVolume.MasterVolume);
            savedVol = spotifyASC.SimpleAudioVolume.MasterVolume;
            addLog("Setting spotify volume to: " + (VOLUME_SCALED_PCT / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
            
            //TODO: test further percentage values. might be able to push 2%, 1%, ... , 0.01%
            spotifyASC.SimpleAudioVolume.MasterVolume = VOLUME_SCALED_PCT / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
            return true;
        }

        /*
         * Adds Spotify's current song to the blockTable, mutes it, and plays elevator music over it.
         */
        public void blockCurrentSong()
        {
            Song currSong = spotInfo.getCurrentSong();
            if (currSong != null)
            {
                blockTable.addSong(currSong);
                addLog("Current song added to the blockTable.");
                trySkipSong();
            }
        }

        private void sendKeyPress(Keys key)
        {
            addLog("Sending keypress: " + key);
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }

        /*
         * Adds Spotify's current artist to the blockTable, mutes it, and plays elevator music over it.
         */
        public void blockCurrentArtist()
        {
            Song currSong = spotInfo.getCurrentSong();
            if (currSong != null)
            {
                
                blockTable.addArtist(new Artist(currSong.getArtistName()));
                addLog("Current artist added to the blockTable.");
                trySkipSong();
            }
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
            addLog("Saving " + logs.Length + " bytes of logs to disk.");
            try
            {
                File.WriteAllText(LOG_FILE_PATH, logs.ToString());
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
            isMusicPlaying = true;
        }

        /*
        *  Stops the mp3 at REPLACEMENT_AUDIO_PATH.
        */
        private void stopReplacementMusic()
        {
            addLog("Stopping replacement music.");
            player.controls.stop();
            isMusicPlaying = false;
        }

        private Boolean isReplacementMusicPlaying()
        {
            return isMusicPlaying;
        }

        /*
         * Validates that the current song is not in the blockTable. If it is in the blockTable, the song is muted and startReplacementMusic() is called. 
         */
        public void checkCurrentSong()
        {
            Song currSong = spotInfo.getCurrentSong();
            if (currSong == null)
            {
                nowPlayingLabel.Text = "[PAUSED]";
                return;
            }
            nowPlayingLabel.Text = currSong.getArtistName() + " - " + currSong.getSongTitle();
            if (!blockTable.contains(currSong))
            {
                if (isReplacementMusicPlaying())
                {
                    stopReplacementMusic();
                    if (savedVol > 0)
                    {
                        addLog("Resetting master volume to: " + savedVol);
                        AudioSessionControl spotifyASC = spotInfo.getSpotifyAudioSession();
                        spotifyASC.SimpleAudioVolume.MasterVolume = savedVol;
                    }
                }
                addLog("Got new Spotify item: " + spotInfo.getCurrentSong());
            }
            else
            {
                //if (isReplacementMusicPlaying() && !isSkipping)
                //{
                //    stopReplacementMusic();
                //}
                trySkipSong();
            }
        }

        private static Boolean isSkipping = false;
        public void trySkipSong()
        {
            if (isSkipping) return; // bugfix: song would be attempted to be skipped over and over because spotify was sending multiple window title events.
            isSkipping = true;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            Object[] retObj = { autoEvent, false };

            new Thread(new ParameterizedThreadStart(trySkipSongThread)).Start(retObj);
            addLog("Trying to skip song - waiting for a return from worker thread.");
            autoEvent.WaitOne();
            addLog("Got our return, failure value: " + (Boolean)retObj[1]);
            if ((Boolean)retObj[1])
            {
                addLog("Failed to skip the song. Muting song.");
                Boolean muteSuccess = forceSpotifyMute();
                if (playElevatorMusic.Checked && !isReplacementMusicPlaying())
                {
                    playReplacementMusic();
                }

                // spotify will try to stop when its individual volume is changed on an unskippable song. if we were already muted and made no changes to the volume, no need to try to counter this.
                if (muteSuccess)
                {
                    sendKeyPress(Keys.MediaPlayPause);
                    addLog("After play, got this song: " + spotInfo.getCurrentSong());
                }
                isSkipping = false;
            }
            else
            {
                isSkipping = false;
                checkCurrentSong();
            }
            
        }

        private void trySkipSongThread(object stateInfo)
        {
                Song songBefore = spotInfo.getCurrentSong();
                
                sendKeyPress(Keys.MediaNextTrack);

                Thread.Sleep(1000); // give some time for events to be sent to spotify, window title to be changed, etc. TODO: add some sort of event to avoid sleeping.
                Song songAfter = spotInfo.getCurrentSong();
                addLog("Worker thread: Tried to skip, song before: " + songBefore + " vs. " + songAfter);
                if (songAfter == null || songBefore.Equals(songAfter))
                {
                    ((Object[])stateInfo)[1] = true;
                }
                ((AutoResetEvent)(((Object[])stateInfo)[0])).Set();
        }

        
        /*
         * Every time the window title is updated for our spotify process, a Win32 event gets fired. See delegate declaration.
         */
        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            addLog("WinEventProc: got new window title.");
            checkCurrentSong();
        }
    }


}
