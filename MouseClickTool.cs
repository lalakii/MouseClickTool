using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: AssemblyVersion("1.7.0.3")]
[assembly: AssemblyFileVersion("1.7.0.3")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

namespace def
{   /// <summary>
    /// 怎么简单怎么来了
    /// </summary>
    public partial class MouseClickTool : Form
    {
        private Input input;
        private bool running = false;
        private const int waitSeconds = 3;
        private const int hotkeyId = 0x233;
        private TaskCompletionSource<int> source;
        private readonly string[] config = { "F1", "1000", "0" };

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        //参考：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct Input
        {
            public int type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInput mi;
        }

        [Flags]
        private enum MouseEventFlag : uint
        {
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlag dwFlags;
            public uint time;
            public unsafe ulong* dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && (int)m.WParam == hotkeyId)
            {
                btnStart.PerformClick();
            }
            base.WndProc(ref m);
        }

        private void UpdateBtnStartText()
        {
            btnStart.Text = (running ? "停止" : "开始") + "(" + config[0] + ")";
        }

        public MouseClickTool()
        {
            InitializeComponent();
            var configFile = Path.Combine(Path.GetTempPath(), "lalaki_mouse_click_tool.ini");
            if (File.Exists(configFile))
            {
                var tempConfig = File.ReadAllLines(configFile);
                if (tempConfig.Length > 2)
                {
                    config = tempConfig;
                }
            }
            for (int i = 1; i < 13; i++)
            {
                hotkeys.Items.Add("F" + i);
            }
            hotkeys.SelectedIndexChanged += (_, __) =>
             {
                 UnregisterHotKey(Handle, hotkeyId);
                 Enum.TryParse(hotkeys.Text, out Keys key);
                 RegisterHotKey(Handle, hotkeyId, 0x4000, (uint)key);
                 var keyStr = key.ToString();
                 config[0] = keyStr;
                 UpdateBtnStartText();
             };
            delayVal.TextChanged += (_, __) => config[1] = delayVal.Text;
            clickType.SelectedIndexChanged += (_, __) => config[2] = clickType.SelectedIndex == 0 ? "0" : "1";
            hotkeys.SelectedItem = config[0];
            delayVal.Text = config[1];
            int.TryParse(config[2], out int clickTypeIndex);
            clickType.SelectedIndex = clickTypeIndex;
            btnUrl.Click += (__, _) => Process.Start(btnUrl.Text);
            btnClose.Click += (__, _) =>
            {
                Hide();
                running = false;
                Application.Exit();
            };
            FormClosing += (__, _) => File.WriteAllLines(configFile, config);
            btnHide.Click += (__, _) => WindowState = FormWindowState.Minimized;
            btnClose.MouseHover += (__, _) => btnClose.ForeColor = Color.IndianRed;
            btnHide.MouseHover += (__, _) => btnHide.ForeColor = Color.MediumPurple;
            EventHandler leave = delegate (object o, EventArgs __) { ((Control)o).ForeColor = DefaultForeColor; };
            btnHide.MouseLeave += leave;
            btnClose.MouseLeave += leave;
            Resize += (_, __) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : WindowState;
            MouseUp += (_, __) => Cursor = Cursors.Default;
            MouseDown += (_, e) => Tag = (e.Button == MouseButtons.Left) ? e.Location : null;
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
                using (var pen = new Pen(Color.MediumPurple, 7f))
                    g.DrawLine(pen, Width, 0, 0, 0);
                using (var border = new Pen(Color.FromArgb(60, 0, 0, 0)))
                    g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
                g.DrawString(Text, new Font("Consolas", 16f), Brushes.DimGray, 15, 20);
            };
            btnStart.Click += (__, _) =>
            {
                if (!running && clickType.Enabled)
                {
                    if (int.TryParse(delayVal.Text, out int delay) && delay > -1)
                    {
                        delayVal.ReadOnly = running = true;
                        btnStart.Enabled = clickType.Enabled = false;
                        Task.Run(async () =>
                        {
                            for (int i = 1; i < waitSeconds; i++)
                            {
                                btnStart.Text = string.Format("{0}", waitSeconds - i);
                                await Task.Delay(1000);
                            }
                            btnStart.Enabled = true;
                            UpdateBtnStartText();
                            var downFlag = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                            var upFlag = MouseEventFlag.MOUSEEVENTF_LEFTUP;
                            if (clickType.SelectedIndex == 1)
                            {
                                downFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                                upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                            }
                            var size = Marshal.SizeOf(input);
                            source = new TaskCompletionSource<int>();
                            while (running)
                            {
                                await Task.Run(async () =>
                                {
                                    input.mkhi.mi.dwFlags = downFlag;
                                    SendInput(1, ref input, size);
                                    input.mkhi.mi.dwFlags = upFlag;
                                    SendInput(1, ref input, size);
                                    if (delay != 0)
                                    {
                                        await Task.WhenAny(Task.Delay(delay), source.Task);
                                    }
                                });
                            }
                            if (delay == 0)
                            {
                                await Task.Delay(1000);
                            }
                            UpdateBtnStartText();
                            delayVal.ReadOnly = false;
                            btnStart.Enabled = clickType.Enabled = true;
                        });
                    }
                    else
                    {
                        MessageBox.Show("鼠标点击间隔，必须是等于或大于0的整数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    btnStart.Enabled = running = false;
                    try
                    {
                        source.SetCanceled();
                    }
                    catch
                    {
                    }
                }
            };
        }

        [STAThread]
        public static void Main()
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.Run(new MouseClickTool());
        }
    }
}