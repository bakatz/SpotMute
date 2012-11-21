using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SpotMute.Controller
{
    /*
     * Plays MP3 files using the winmm API. 
     */
    class MP3Player
    {
        private string _command;
        private bool isOpen;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        /*
         * Send command to stop the current MP3 without a callback function. (this is why last 3 params are null, 0, and IntPtr.Zero) 
         */
        public void Close()
        {
            _command = "close MediaFile";
            mciSendString(_command, null, 0, IntPtr.Zero);
            isOpen = false;
        }

        /*
         * Send command to open a MP3 file without a callback function. (this is why last 3 params are null, 0, and IntPtr.Zero) 
         */
        public void Open(string sFileName)
        {
            if (File.Exists(sFileName))
            {
                _command = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
                mciSendString(_command, null, 0, IntPtr.Zero);
                isOpen = true;
            }
        }

        /*
         * Send command to play the current MP3 without a callback function. (this is why last 3 params are null, 0, and IntPtr.Zero)  If loop is true, the song will play indefinitely. 
         */
        public void Play(bool loop)
        {
            if (isOpen)
            {
                _command = "play MediaFile";
                if (loop)
                    _command += " REPEAT";
                mciSendString(_command, null, 0, IntPtr.Zero);
            }
        }
    }
}
