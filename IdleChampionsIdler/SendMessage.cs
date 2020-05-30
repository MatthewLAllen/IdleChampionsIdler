using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IdleChampionsIdler
{
    class SendMyMessage
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static void sendKeystroke()
        {
            const uint WM_KEYDOWN = 0x0100;
            const uint WM_KEYUP = 0x0101;
            const uint WM_SYSKEYDOWN = 0x0104;
            const uint Key_Down = 0x0001;
            const uint Key_Up = 0x0002;

            const uint VK_F1 = 0x70;
            const int VK_1 = 0x30;

            IntPtr hWnd = FindWindow(null, "Idle Champions");

            extraKeyInfo lParam = new extraKeyInfo();
            lParam.scanCode = (char)MapVirtualKey(VK_F1, 0);
            lParam.repeatCount = 1;
            int myParam = lParam.getint();
            var test = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_F1, (IntPtr)myParam);
            lParam.repeatCount = 1;
            lParam.prevKeyState = 1;
            lParam.transitionState = 1;
            System.Threading.Thread.Sleep(100);
            myParam = lParam.getint();
            var test2 = SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_F1, (IntPtr)myParam);
        }

        class extraKeyInfo
        {
            public ushort repeatCount;
            public char scanCode;
            public ushort extendedKey, prevKeyState, transitionState;

            public int getint()
            {
                return repeatCount | (scanCode << 16) | (extendedKey << 24) |
                    (prevKeyState << 30) | (transitionState << 31);
            }
        };

    }
}
