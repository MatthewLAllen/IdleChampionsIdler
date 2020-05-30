using System;
using System.Drawing;
using System.Threading;

namespace IdleChampionsIdler
{
    public class GetWindowSnapshot
    {
   
        private static bool _wasMinimized = false;
        private static IntPtr _curWin;

        public static Bitmap GetSnapshot(bool saveSnap = false)
        {
            IntPtr idleChampsHandle = WindowUtil.GetIdleChampsWindow();
            if (idleChampsHandle == IntPtr.Zero)
                return null;

            _curWin = WindowUtil.GetForegroundWindow();

            #region Ensure the IdleChamps window is fully visible, and topmost
            #region Restore if Minimized
            Rectangle bounds;
            
            if (WindowUtil.IsMinimized(idleChampsHandle))
            {
                _wasMinimized = true;
                WindowUtil.Restore(idleChampsHandle);
                Thread.Sleep(1000);//restore animation time holder, could pull this from registry? Snapshot is wrong if it's still expanding.
            }
            else
                _wasMinimized = false;
            #endregion Restore if Minimized

            WindowUtil.SetForegroundWindow(idleChampsHandle);

            #region If the window has a portion off-screen, fix that (assumes putting the top corner at 10,10 will fix the problem)
            //edge case, if the window (whether we restored it or not) is off the edge of the screen, move it so that it isn't.
            //rather than testing things, just put the upper left at 10,10
            //assume taskbar is 50px
            WindowUtil.FixOffScreen(idleChampsHandle);

            #endregion If the window has a portion off-screen, fix that (assumes putting the top corner at 10,10 will fix the problem)
            #endregion Ensure the IdleChamps window is fully visible, and topmost

            #region Snapshot the window into a Bitmap Obj
            var rect = new WindowUtil.Rect();
            WindowUtil.GetWindowRect(idleChampsHandle, ref rect);

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
                IntPtr ptr = WindowUtil.GetIdleChampsWindow();
                if (ptr == IntPtr.Zero)
                    return;
                WindowUtil.Minimize(ptr);
            }

            if (_curWin != IntPtr.Zero)
                WindowUtil.SetForegroundWindow(_curWin);
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
