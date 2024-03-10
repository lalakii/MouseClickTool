using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright © 2024 lalaki.cn")]
[assembly: AssemblyVersion("1.5.0.0")]
[assembly: AssemblyFileVersion("1.5.0.0")]
namespace MouseClickTool
{/// <summary>
/// 怎么简单怎么来了
/// </summary>
/// 
    public partial class main : Form
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
        //新方法：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        class MouseSimulator
        {
            [DllImport("user32.dll")]
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
        const int waitSeconds = 4;
        bool isRunning = false;
        void SystemExit()
        {
            Hide();
            isRunning = false;
            Environment.Exit(0);
        }
        delegate void testClick();
        public main()
        {
            InitializeComponent();
            this.Icon = SystemIcons.Application;
            this.comboBox1.SelectedIndex = 0;
            url.Click += (s, e) => Process.Start(url.Text);
            btn_close.Click += (s, e) => SystemExit();
            btn_min.Click += (s, e) => WindowState = FormWindowState.Minimized;
            btn_close.MouseHover += (s, e) => btn_close.ForeColor = Color.IndianRed;
            btn_close.MouseLeave += (s, e) => btn_close.ForeColor = Control.DefaultForeColor;
            btn_min.MouseHover += (s, e) => btn_min.ForeColor = Color.DodgerBlue;
            btn_min.MouseLeave += (s, e) => btn_min.ForeColor = Control.DefaultForeColor;
            Resize += (s, e) =>
            {
                if (WindowState == FormWindowState.Maximized) WindowState = FormWindowState.Normal;
            };
            MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Tag = e.Location;
                }
            };
            MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && Tag is Point offset)
                {
                    Cursor = Cursors.SizeAll;
                    Point newLocation = this.PointToScreen(e.Location);
                    newLocation.Offset(-offset.X, -offset.Y);
                    this.Location = newLocation;
                }
            };
            MouseUp += (s, e) => Cursor = Cursors.Default;
            Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var pen = new Pen(Color.HotPink, 7f))
                    g.DrawLine(pen, Width, 0, 0, 0);
                using (var border = new Pen(Color.FromArgb(55, 0, 0, 0)))
                    g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
            };
            btn_start.Click += (s, e) =>
            {
                if (isRunning)
                {
                    SystemExit();
                    return;
                }
                if (int.TryParse(is_ms.Text, out int result) && result > 0)
                {
                    isRunning = true;
                    is_ms.ReadOnly = true;
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Run(async () =>
                        {
                            var startText = btn_start.Text;
                            for (int i = 1; i < waitSeconds; i++)
                            {
                                btn_start.Text = string.Format("{0}({1})", startText, waitSeconds - i);
                                await Task.Delay(1000);
                            }
                        });
                        btn_start.Text = "停止";
                        testClick mouseClick = new testClick(MouseSimulator.ClickRightMouseButton);
                        if (this.comboBox1.SelectedIndex == 0)
                        {
                            mouseClick = new testClick(MouseSimulator.ClickLeftMouseButton);
                        }
                        while (isRunning)
                        {
                            await Task.Run(async () =>
                            {
                                mouseClick.Invoke();
                                await Task.Delay(result);
                            });
                        }
                    });
                }
                else
                {
                    MessageBox.Show("输入的数字不正确，必须是正整数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }
    }
}