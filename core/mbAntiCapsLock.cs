
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace RED.mbnq
{
    public class mbAntiCapsLockManager
    {
        const int VK_CAPITAL = 0x14;
        const uint KEYEVENTF_KEYUP = 0x0002;
        private bool mIsAntiCapsLockEnabled = true;
        private Thread capsLockMonitorThread;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // Method to monitor and disable Caps Lock
        private void MonitorCapsLock()
        {
            while (mIsAntiCapsLockEnabled)
            {
                // Check if Caps Lock is on
                if (((ushort)GetKeyState(VK_CAPITAL) & 0xffff) != 0)
                {
                    // Turn off Caps Lock by sending both key down and key up events
                    keybd_event((byte)VK_CAPITAL, 0x45, 0, (UIntPtr)0);                 // Key down
                    keybd_event((byte)VK_CAPITAL, 0x45, KEYEVENTF_KEYUP, (UIntPtr)0);   // Key up
                }
                Thread.Sleep(100);
            }
        }
        public void StartCapsLockMonitor()
        {
            if (capsLockMonitorThread == null || !capsLockMonitorThread.IsAlive)
            {
                capsLockMonitorThread = new Thread(MonitorCapsLock);
                capsLockMonitorThread.IsBackground = true;
                mIsAntiCapsLockEnabled = true;
                capsLockMonitorThread.Start();
            }
        }
        public void StopCapsLockMonitor()
        {
            if (capsLockMonitorThread != null && capsLockMonitorThread.IsAlive)
            {
                mIsAntiCapsLockEnabled = false;
                capsLockMonitorThread.Join();  // Wait for the thread to stop
            }
        }
    }
}
