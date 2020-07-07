using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

        private const uint RightDown = 0x0008;
        private const uint RightUp = 0x0010;
        private const uint LeftDown = 0x0002;
        private const uint LeftUp = 0x0004;

        public static void SendRightClick(uint posX, uint posY)
        {
            mouse_event(RightDown, posX, posY, 0, new System.IntPtr());
            mouse_event(RightUp, posX, posY, 0, new System.IntPtr());
        }
        public static void SendLeftClick(uint posX, uint posY)
        {
            mouse_event(LeftDown, posX, posY, 0, new System.IntPtr());
            mouse_event(LeftUp, posX, posY, 0, new System.IntPtr());
        }
        public Form1()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
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
                                 uint x = (uint)Cursor.Position.X;
                                 uint y = (uint)Cursor.Position.Y;
                                 SendLeftClick(x, y);
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
                                 uint x = (uint)Cursor.Position.X;
                                 uint y = (uint)Cursor.Position.Y;
                                 SendRightClick(x, y);
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