using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace IdleChampionsIdler
{
    public class GetWindowSnapshot
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        const short SWP_NOMOVE = 0X2;
        const short SWP_NOSIZE = 1;
        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;

        private static bool _wasMinimized = false;
        private static IntPtr _curWin;

        public static Bitmap GetSnapshot(bool saveSnap = false)
        {
            IntPtr idleChampsHandle = SendKey.GetIdleChampsWindow();
            if (idleChampsHandle == IntPtr.Zero)
                return null;

            _curWin = GetForegroundWindow();

            #region Ensure the IdleChamps window is fully visible, and topmost
            #region Restore if Minimized
            Rectangle bounds;
            var rect = new Rect();
            if (IsMinimized(idleChampsHandle))
            {
                _wasMinimized = true;
                Restore(idleChampsHandle);
                Thread.Sleep(1000);//restore animation time holder, could pull this from registry? Snapshot is wrong if it's still expanding.
            }
            else
                _wasMinimized = false;
            #endregion Restore if Minimized

            SetForegroundWindow(idleChampsHandle);

            #region If the window has a portion off-screen, fix that (assumes putting the top corner at 10,10 will fix the problem)
            //edge case, if the window (whether we restored it or not) is off the edge of the screen, move it so that it isn't.
            //rather than testing things, just put the upper left at 10,10
            //assume taskbar is 50px
            GetWindowRect(idleChampsHandle, ref rect);
            if (rect.Bottom > SystemInformation.VirtualScreen.Height - 50 || rect.Right > SystemInformation.VirtualScreen.Width)//does this account for the taskbar at the bottom? Probably not.
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                SetWindowPos(idleChampsHandle, 0, 10, 10, width, height, SWP_NOZORDER | SWP_SHOWWINDOW);
            }
            #endregion If the window has a portion off-screen, fix that (assumes putting the top corner at 10,10 will fix the problem)
            #endregion Ensure the IdleChamps window is fully visible, and topmost

            #region Snapshot the window into a Bitmap Obj
            GetWindowRect(idleChampsHandle, ref rect);

            bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            #endregion Snapshot the window into a Bitmap Obj

            #region Return desktop state to where it was

            #endregion Return desktop state to where it was

            //test writing to desktop, used to determine where the autoprogress arrow is
            if (saveSnap)
                try
                {
                    result.Save(@"C:\users\Dave\Desktop\test.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex) { Console.WriteLine(ex); }

            return result;//remember to dispose result, bitmap leaks otherwise.
        }

        public static void ReturnToPreviousWindowstate()
        {

            if (_wasMinimized)
            {
                IntPtr ptr = SendKey.GetIdleChampsWindow();
                if (ptr == IntPtr.Zero)
                    return;
                Minimize(ptr);
            }

            if (_curWin != IntPtr.Zero)
                SetForegroundWindow(_curWin);
        }

        private static void Minimize(IntPtr ptr)
        {
            ShowWindow(ptr, ShowWindowEnum.Minimize);
        }

        private static void Restore(IntPtr ptr)
        {
            ShowWindow(ptr, ShowWindowEnum.Restore);
        }

        private static bool IsMinimized(IntPtr ptr)
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

        public static bool PureGreenInSubRectangle(Bitmap img, int top, int left, int height, int width, bool saveSubSnap = false)
        {
            Rectangle cloneRect = new Rectangle(left, top, width, height);
            System.Drawing.Imaging.PixelFormat format = img.PixelFormat;
            using (Bitmap cloneBitmap = img.Clone(cloneRect, format))//could iterate through the original bitmap but this is more clear.
            {
                if (saveSubSnap)
                    try
                    {
                        cloneBitmap.Save(@"C:\users\dave\desktop\subpic.png"); //if this doesn't contain the autoprogress arrow, that's the issue.
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }

                int greenToArgb = Color.FromArgb(255, 0, 255, 0).ToArgb();//this pure green appears to be used for the autoprogress toggle, and nowhere else on the UI.  Could give false positives?
                for (int x = 0; x < cloneBitmap.Width; x++)
                {
                    for (int y = 0; y < cloneBitmap.Height; y++)
                    {
                        Color c = cloneBitmap.GetPixel(x, y);
                        if (c.ToArgb() == greenToArgb)
                            return true;
                    }
                }
                return false;
            }
        }
    }
}
