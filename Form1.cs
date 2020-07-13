using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseClickTool
{/// <summary>
/// 怎么简单怎么来了
/// </summary>
    public partial class Form1 : Form
    {
        //新方法：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        internal class MouseSimulator
        {
            [DllImport("user32.dll", SetLastError = true)]
            static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

            [StructLayout(LayoutKind.Sequential)]
            struct INPUT
            {
                public SendInputEventType type;
                public MouseKeybdhardwareInputUnion mkhi;
            }

            [StructLayout(LayoutKind.Explicit)]
            struct MouseKeybdhardwareInputUnion
            {
                [FieldOffset(0)]
                public MouseInputData mi;
            }

            [Flags]
            enum MouseEventFlags : uint
            {
                MOUSEEVENTF_LEFTDOWN = 0x0002,
                MOUSEEVENTF_LEFTUP = 0x0004,
                MOUSEEVENTF_RIGHTDOWN = 0x0008,
                MOUSEEVENTF_RIGHTUP = 0x0010,
            }

            [StructLayout(LayoutKind.Sequential)]
            struct MouseInputData
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public MouseEventFlags dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            enum SendInputEventType : int
            {
                InputMouse
            }

            public static void ClickLeftMouseButton()
            {
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

                INPUT mouseUpInput = new INPUT();
                mouseUpInput.type = SendInputEventType.InputMouse;
                mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
                SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            }

            public static void ClickRightMouseButton()
            {
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

                INPUT mouseUpInput = new INPUT();
                mouseUpInput.type = SendInputEventType.InputMouse;
                mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            this.MaximizeBox = false;
        }

        private void is_begin_Click(object sender, EventArgs e)
        {
            if (is_begin.Text == "停止")
            {
                Environment.Exit(0);
                return;
            }
            string ms = is_ms.Text;
            int result;
            if (int.TryParse(ms, out result) && result > 0)
            {
                is_ms.ReadOnly = true;
                Task.Factory.StartNew(async () =>
                 {
                     await Task.Run(() =>
                     {
                         for (int i = 1; i < 5; i++)
                         {
                             is_begin.Text = string.Format("开始({0})", 5 - i);
                             Thread.Sleep(1000);
                         }
                     });
                     is_begin.Text = string.Format("停止");
                     if (this.comboBox1.SelectedIndex == 0)
                     {
                         for (; ; )
                         {
                             await Task.Run(() =>
                             {
                                 MouseSimulator.ClickLeftMouseButton();
                                 Thread.Sleep(result);
                             });
                         }
                     }
                     else
                     {
                         for (; ; )
                         {
                             await Task.Run(() =>
                             {
                                 MouseSimulator.ClickRightMouseButton();
                                 Thread.Sleep(result);
                             });
                         }
                     }

                 });
            }
            else
            {
                MessageBox.Show("输入的数字不正确，必须是正整数");
            }
        }
    }
}