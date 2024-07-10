using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: AssemblyVersion("1.7.0.1")]
[assembly: AssemblyFileVersion("1.7.0.1")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

namespace MouseClickTool
{   /// <summary>
    /// 怎么简单怎么来了
    /// </summary>
    internal partial class main : Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("shell32.dll")]
        private static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        //新方法：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;
        }

        [Flags]
        private enum MouseEventFlags : uint
        {
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void ClickMouseButton(MouseEventFlags downFlag, MouseEventFlags upFlag)
        {
            input.mkhi.mi.dwFlags = downFlag;
            var size = Marshal.SizeOf(input);
            SendInput(1, ref input, size);
            input.mkhi.mi.dwFlags = upFlag;
            SendInput(1, ref input, size);
        }

        private INPUT input;
        private const int waitSeconds = 3;
        private bool isRunning = false;
        private int hotkey_id = 0x233;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && (int)m.WParam == hotkey_id)
            {
                btnStart.PerformClick();
            }
            base.WndProc(ref m);
        }

        private string GetStartText()
        {
            return "开始 (" + config[0] + ")";
        }

        private string[] config = new[] { "F1", "1000", "0" };

        private main()
        {
            InitializeComponent();
            for (int i = 1; i < 13; i++)
            {
                hotkeys.Items.Add("F" + i);
            }
            var configFile = Path.Combine(Path.GetTempPath(), "lalaki_mouse_click_tool.ini");
            if (File.Exists(configFile))
            {
                var tempConfig = File.ReadAllLines(configFile);
                if (tempConfig.Length > 2)
                {
                    config = tempConfig;
                }
            }
            hotkeys.SelectedIndexChanged += (_, __) =>
             {
                 UnregisterHotKey(Handle, hotkey_id);
                 Enum.TryParse(hotkeys.Text, out Keys key);
                 RegisterHotKey(Handle, hotkey_id, 0x4000, (uint)key);
                 var keyStr = key.ToString();
                 config[0] = keyStr;
                 btnStart.Text = GetStartText();
             };
            delayVal.TextChanged += (_, __) => config[1] = delayVal.Text;
            hotkeys.SelectedItem = config[0];
            delayVal.Text = config[1];
            clickType.SelectedIndexChanged += (_, __) => config[2] = clickType.SelectedIndex == 0 ? "0" : "1";
            int.TryParse(config[2], out int clickTypeIndex);
            clickType.SelectedIndex = clickTypeIndex;
            btnUrl.Click += (__, _) => ShellExecute(0, "open", btnUrl.Text, "", "", 1);
            btnClose.Click += (__, _) =>
            {
                isRunning = false;
                Hide();
                Application.Exit();
            };
            FormClosing += (__, _) => File.WriteAllLines(configFile, config);
            btnMin.Click += (__, _) => WindowState = FormWindowState.Minimized;
            btnClose.MouseHover += (__, _) => btnClose.ForeColor = Color.IndianRed;
            btnClose.MouseLeave += (__, _) => btnClose.ForeColor = Control.DefaultForeColor;
            btnMin.MouseHover += (__, _) => btnMin.ForeColor = Color.DodgerBlue;
            btnMin.MouseLeave += (__, _) => btnMin.ForeColor = Control.DefaultForeColor;
            Resize += (_, __) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : WindowState;
            MouseUp += (_, __) => Cursor = Cursors.Default;
            MouseDown += (_, e) => Tag = (e.Button == MouseButtons.Left) ? (object)e.Location : null;
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
                if (isRunning || !clickType.Enabled)
                {
                    isRunning = false;
                    return;
                }
                if (int.TryParse(delayVal.Text, out int delay) && delay > -1)
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
                                btnStart.Text = string.Format("{0}", waitSeconds - i);
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
                                if (delay != 0)
                                    await Task.Delay(delay);
                            });
                        }
                        delayVal.ReadOnly = false;
                        clickType.Enabled = true;
                        btnStart.Text = GetStartText();
                    });
                }
                else
                    MessageBox.Show("鼠标点击间隔，必须是一个大于或等于0的数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }

        [STAThread]
        private static void Main()
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}