using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace IdleChampionsIdler
{
    class SendKey
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static IntPtr SetWindow()
        {
            IntPtr curWin = GetForegroundWindow();
            IntPtr gameWin = GetIdleChampsWindow();
            SetForegroundWindow(gameWin);

            return curWin;
        }

        public static IntPtr GetIdleChampsWindow()
        {
            IntPtr gameWin = FindWindow("UnityWndClass", "Idle Champions");
            return gameWin;
        }

        public static void ReturnWindow(IntPtr returnWindow)
        {
            SetForegroundWindow(returnWindow);
        }

        public static void Send(string KeyToSend)
        {
            int sleepTime = 20;

            Thread.Sleep(sleepTime);
            SendKeys.SendWait(KeyToSend);
            Thread.Sleep(sleepTime);

        }
    }
}