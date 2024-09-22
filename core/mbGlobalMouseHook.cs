
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RED.mbnq
{
    public static class GlobalMouseHook
    {

        // delegate for the hook procedure
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc proc = HookCallback;

        // handle for the hook
        private static IntPtr hookId = IntPtr.Zero;

        #region Import
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] 
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region init
        private const int WH_MOUSE_LL = 14;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        public static void SetHook() { hookId = SetHook(proc); }
        public static void Unhook()  { UnhookWindowsHookEx(hookId); }
        private static IntPtr SetHook(HookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        #endregion

        #region forSniperMode
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (ZoomMode.IsZoomModeEnabled)
                {
                    if (wParam == (IntPtr)WM_RBUTTONDOWN)
                    {
                        ZoomMode.StartHoldTimer();
                    }
                    else if (wParam == (IntPtr)WM_RBUTTONUP)
                    {
                        ZoomMode.StopHoldTimer();
                        ZoomMode.HideZoomOverlay();
                    }
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
        #endregion

    }
}
