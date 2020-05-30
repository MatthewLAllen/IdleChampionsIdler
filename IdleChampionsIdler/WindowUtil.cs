using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

namespace IdleChampionsIdler
{
    public class WindowUtil
    {
        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;

        public static IntPtr GetIdleChampsWindow()
        {
            IntPtr gameWin = FindWindow("UnityWndClass", "Idle Champions");
            return gameWin;
        }

        public static void FixOffScreen(IntPtr IdleChampsHandle)
        {
            var rect = new Rect();

            GetWindowRect(IdleChampsHandle, ref rect);
            if (rect.Bottom > SystemInformation.VirtualScreen.Height - 50 || rect.Right > SystemInformation.VirtualScreen.Width)//does this account for the taskbar at the bottom? Probably not.
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                SetWindowPos(IdleChampsHandle, 0, 10, 10, width, height, SWP_NOZORDER | SWP_SHOWWINDOW);
            }
        }


        public static void Minimize(IntPtr ptr)
        {
            ShowWindow(ptr, ShowWindowEnum.Minimize);
        }

        public static void Restore(IntPtr ptr)
        {
            ShowWindow(ptr, ShowWindowEnum.Restore);
        }


        public static bool IsMinimized(IntPtr ptr)
        {
            AutomationElement ae = AutomationElement.FromHandle(ptr);
            object windowPattern = null;
            if (ae.TryGetCurrentPattern(WindowPattern.Pattern, out windowPattern))
            {
                WindowVisualState state = ((WindowPattern)windowPattern).Current.WindowVisualState;
                if (state == WindowVisualState.Minimized)
                    return true;
            }
            return false;
        }
    }
}
