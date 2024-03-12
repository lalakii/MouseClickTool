using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]
[assembly: AssemblyFileVersion("1.6.0.0")]
[assembly: AssemblyVersion("1.6.0.0")]
namespace MouseClickTool
{/// <summary>
 /// 怎么简单怎么来了
 /// </summary>
 /// 
    partial class main : Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
        [DllImport("shell32.dll")]
        static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        //新方法：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
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
        INPUT input;
        void ClickMouseButton(MouseEventFlags downFlag, MouseEventFlags upFlag)
        {
            input.mkhi.mi.dwFlags = downFlag;
            var size = Marshal.SizeOf(input);
            SendInput(1, ref input, size);
            input.mkhi.mi.dwFlags = upFlag;
            SendInput(1, ref input, size);
        }
        const int waitSeconds = 4;
        bool isRunning = false;
        main()
        {
            InitializeComponent();
            Icon = SystemIcons.Application;
            clickType.SelectedIndex = 0;
            btnUrl.Click += (__, _) => ShellExecute(0, "open", btnUrl.Text, "", "", 1);
            btnClose.Click += (__, _) =>
            {
                isRunning = false;
                Hide();
                Application.Exit();
            };
            btnMin.Click += (__, _) => WindowState = FormWindowState.Minimized;
            btnClose.MouseHover += (__, _) => btnClose.ForeColor = Color.IndianRed;
            btnClose.MouseLeave += (__, _) => btnClose.ForeColor = Control.DefaultForeColor;
            btnMin.MouseHover += (__, _) => btnMin.ForeColor = Color.DodgerBlue;
            btnMin.MouseLeave += (__, _) => btnMin.ForeColor = Control.DefaultForeColor;
            Resize += (_, __) =>
            {
                if (WindowState == FormWindowState.Maximized) WindowState = FormWindowState.Normal;
            };
            MouseDown += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Tag = e.Location;
                }
            };
            MouseMove += (_, e) =>
            {
                if (e.Button == MouseButtons.Left && Tag is Point offset)
                {
                    Cursor = Cursors.SizeAll;
                    var location = PointToScreen(e.Location);
                    location.Offset(-offset.X, -offset.Y);
                    Location = location;
                }
            };
            MouseUp += (_, __) => Cursor = Cursors.Default;
            Paint += (_, e) =>
            {
                var g = e.Graphics;
                using (var pen = new Pen(Color.HotPink, 7f))
                    g.DrawLine(pen, Width, 0, 0, 0);
                using (var border = new Pen(Color.FromArgb(55, 0, 0, 0)))
                    g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
            };
            btnStart.Click += (__, _) =>
            {
                if (isRunning)
                {
                    isRunning = false;
                    return;
                }
                if (!clickType.Enabled) return;
                if (int.TryParse(delayVal.Text, out int result) && result > -1)
                {
                    delayVal.ReadOnly = isRunning = true;
                    btnStart.Enabled = clickType.Enabled = false;
                    Task.Run(async () =>
                    {
                        await Task.Run(async () =>
                        {
                            var startText = btnStart.Text;
                            for (int i = 1; i < waitSeconds; i++)
                            {
                                btnStart.Text = string.Format("{0}({1})", startText, waitSeconds - i);
                                await Task.Delay(1000);
                            }
                        });
                        btnStart.Enabled = true;
                        btnStart.Text = "停止";
                        MouseEventFlags downFlag = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                        MouseEventFlags upFlag = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                        if (clickType.SelectedIndex == 0)
                        {
                            downFlag = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                            upFlag = MouseEventFlags.MOUSEEVENTF_LEFTUP;
                        }
                        while (isRunning)
                        {
                            await Task.Run(async () =>
                            {
                                ClickMouseButton(downFlag, upFlag);
                                await Task.Delay(result);
                            });
                        }
                        delayVal.ReadOnly = false;
                        clickType.Enabled = true;
                        btnStart.Text = "开始";
                    });
                }
                else
                    MessageBox.Show("鼠标点击间隔，必须是一个大于或等于0的数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }
        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}