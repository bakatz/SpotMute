/* 
 * Spotify Ad Muter
 * "Mutes" spotify ads based on a blacklist that resides in memory.
 * 
 * Author: Ben Katz (bakatz@vt.edu)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using CoreAudioApi;

namespace SpotifyAdMuter
{
    class Program
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);


        // Constants from winuser.h
        const uint EVENT_SYSTEM_FOREGROUND = 3;
        const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        const uint WINEVENT_OUTOFCONTEXT = 0;
        // End constants from winuser.h

        private static Process spotProc = null;
        private static float savedVol = 0;
        private static List<String> blackList;
        private static String lastItem = null;


        private static MMDevice defaultDevice = null;
        static WinEventDelegate procDelegate;

        public static void Main()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator(); // get multimedia device enumerator.
            defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia); // grab our default sound device
            procDelegate = new WinEventDelegate(WinEventProc);
            blackList = new List<String>();
            Process[] procs = Process.GetProcessesByName("spotify"); // find the spotify process by name.
            if (procs.Length > 0)
            {
                spotProc = procs[0]; // in the case of multiple spotify processes, just use the first one.
                // Listen for window title changes in the spotify process, using spotify's pid
                IntPtr hhook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                        procDelegate, (uint)spotProc.Id, 0, WINEVENT_OUTOFCONTEXT);

                // MessageBox provides the necessary mesage loop that SetWinEventHook requires.
                MessageBox.Show("Click OK to stop muting ads.");
                UnhookWinEvent(hhook);
            }
            else
            {
                MessageBox.Show("Spotify doesn't appear to be running. Start spotify normally and then run this program again.");
            }

        }

        static AudioSessionControl getSpotifyAudioSession(int spotProcId)
        {
            for (int i = 0; i < defaultDevice.AudioSessionManager.Sessions.Count; i++)
            {
                AudioSessionControl currASC = defaultDevice.AudioSessionManager.Sessions[i];
                if (currASC.ProcessID == spotProcId)
                {
                    return currASC;
                }
            }
            return null;
        }

        static void forceSpotifyMute(AudioSessionControl spotifyASC)
        {
            if (Math.Abs(spotifyASC.SimpleAudioVolume.MasterVolume - (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar)) < 0.0001 && savedVol > 0) // bugfix: muting twice leads to never unmuting even on a whitelisted song
            {
                Console.WriteLine("Not going to set savedvol, Difference: " + Math.Abs(savedVol - (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar)));
                Console.WriteLine(savedVol + " vs calc: " + (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                Console.WriteLine("User's volume is already at 5%, ignoring mute request.");
                return;
            }
            Console.WriteLine("Got spotify volume before mute: " + spotifyASC.SimpleAudioVolume.MasterVolume);
            savedVol = spotifyASC.SimpleAudioVolume.MasterVolume;
            Console.WriteLine("Setting spotify volume to: " + (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
            // The description says "mute" because Spotify will pause entirely if the volume is muted. 5% is the minimum value
            // to be quiet enough to be considered muted and still keep spotify running.
            spotifyASC.SimpleAudioVolume.MasterVolume = 0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;

            //TODO: play elevator music (or user's choice) over the "muted" ad.
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Process[] procs = Process.GetProcessesByName("spotify"); // we need to refresh the spotify process each time, because spotify tends to change process id when hiding/unhiding.
            if (procs.Length > 0)
            {
                spotProc = procs[0];
                AudioSessionControl spotifyASC = getSpotifyAudioSession(spotProc.Id);
                if (spotifyASC == null) return;

                if (spotProc.MainWindowTitle.Equals(lastItem))
                { // Spotify likes to change the window title to the same thing over and over when you highlight things. Hackaround for this one.
                    return;
                }
                else if (!blackList.Contains(spotProc.MainWindowTitle))
                {
                    if (savedVol > 0)
                    {
                        Console.WriteLine("Resetting master volume to: " + savedVol);
                        spotifyASC.SimpleAudioVolume.MasterVolume = savedVol;
                    }
                    lastItem = spotProc.MainWindowTitle;
                    Console.WriteLine("Got new Spotify item: " + spotProc.MainWindowTitle + ". Add to blacklist? (y/n)");
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(false);
                    }
                    char resp = Console.ReadKey().KeyChar;
                    if (resp == 'n')
                    {
                        Console.WriteLine("Current item will not be added to the blacklist.");
                        return;
                    }
                    Console.WriteLine("Current item added to the blacklist. Muting volume.");
                    blackList.Add(spotProc.MainWindowTitle);
                    forceSpotifyMute(spotifyASC);

                }
                else
                {
                    Console.WriteLine("Found an ad in the blacklist: " + (spotProc.MainWindowTitle) + ". Muting for duration of the ad.");
                    forceSpotifyMute(spotifyASC);
                }
            }
        }
    }
}
