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
        //[DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        //[DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);
        //[DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);


        // Constants from winuser.h
        const uint EVENT_SYSTEM_FOREGROUND = 3;
        const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        private static Process spotProc = null;
        private static float savedVol = 0;
        private static List<String> blackList;
        private static String lastItem = null;

        private static MMDeviceEnumerator devEnum = null;
        private static MMDevice defaultDevice = null;
        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        static WinEventDelegate procDelegate;

        public static void Main()
        {
	    devEnum = new MMDeviceEnumerator();
	    defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            procDelegate = new WinEventDelegate(WinEventProc);
            blackList = new List<String>();
            Process[] procs = Process.GetProcessesByName("spotify");
            if (procs.Length > 0)
            {
                spotProc = procs[0];
                
                // MessageBox provides the necessary mesage loop that SetWinEventHook requires.
                //MessageBox.Show("Tracking focus, close message box to exit.");
                // Listen for foreground changes across all processes/threads on current desktop...
                IntPtr hhook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                        procDelegate, (uint)spotProc.Id, 0, WINEVENT_OUTOFCONTEXT);
                MessageBox.Show("Click OK to stop muting ads.");
                UnhookWinEvent(hhook);
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
            Console.WriteLine("Got spotify volume: " + spotifyASC.SimpleAudioVolume.MasterVolume);
            savedVol = spotifyASC.SimpleAudioVolume.MasterVolume;
            Console.WriteLine("Setting to new volume: " + (0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
            spotifyASC.SimpleAudioVolume.MasterVolume = 0.05f / defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //if(hwnd.ToInt32() == spotProc.Handle.ToInt32())
            Process[] procs = Process.GetProcessesByName("spotify"); // we need to refresh the spotify process each time, because spotify tends to change process id when hiding/unhiding.
            if (procs.Length > 0)
            {
                spotProc = procs[0];
                AudioSessionControl spotifyASC = getSpotifyAudioSession(spotProc.Id);
                if (spotifyASC == null) return;

                if (spotProc.MainWindowTitle.Equals(lastItem)) { // Spotify likes to change the window title to the same thing over and over when you highlight things. Hackaround for this one.
                    return;
                } else if (!blackList.Contains(spotProc.MainWindowTitle)) {
                    if (savedVol > 0)
                        spotifyASC.SimpleAudioVolume.MasterVolume = savedVol;
                    lastItem = spotProc.MainWindowTitle;
                    Console.WriteLine("Got new Spotify item: " + spotProc.MainWindowTitle + ". Add to blacklist? (y/n)");
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(false);
                    }
                    char resp = Console.ReadKey().KeyChar;
                    Console.WriteLine("Got response: " + resp + " for " + spotProc.MainWindowTitle);
                    if (resp == 'n') {
                        Console.WriteLine("Current item will not be added to the blacklist.");
                        return;
                    }
                    Console.WriteLine("Current item added to the blacklist. Muting volume.");
                    blackList.Add(spotProc.MainWindowTitle);
                    forceSpotifyMute(spotifyASC);

                } else {
                    Console.WriteLine("Found an ad in the blacklist! Muting for duration of the ad.");
                    forceSpotifyMute(spotifyASC);
                }
            }
        }
    }
}
