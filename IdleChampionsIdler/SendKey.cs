using System;
using System.Windows.Forms;
using System.Threading;

namespace IdleChampionsIdler
{
    class SendKey
    {


        public static IntPtr SetWindow()
        {
            IntPtr curWin = WindowUtil.GetForegroundWindow();
            IntPtr gameWin = WindowUtil.GetIdleChampsWindow();
            WindowUtil.SetForegroundWindow(gameWin);

            return curWin;
        }

        public static void ReturnWindow(IntPtr returnWindow)
        {
            WindowUtil.SetForegroundWindow(returnWindow);
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