using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: AssemblyVersion("2.2.0.0")]
[assembly: AssemblyFileVersion("2.2.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

[System.ComponentModel.DesignerCategory("")]
public class MouseClickTool : Form
{
    private readonly Button bs = new() { AutoSize = true, TextAlign = ContentAlignment.MiddleCenter };
    private readonly string[] cfg = ["F1", "1000", "0"];
    private readonly bool cn = System.Globalization.CultureInfo.InstalledUICulture.Name.IndexOf("zh-", StringComparison.OrdinalIgnoreCase) > -1;
    private readonly string title = $"MouseClickTool {(Environment.Is64BitProcess ? " x64" : " x86")}";
    private Input input;
    private bool running;
    private TaskCompletionSource<int> source;
    private int waitSeconds = 3;

    public MouseClickTool()
    {
        Text = title;
        BackColor = Color.GhostWhite;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        StartPosition = FormStartPosition.CenterScreen;
        Label dvl = new() { Text = cn ? "间隔(毫秒/ms):" : "Interval/(ms):", AutoSize = true, TextAlign = ContentAlignment.BottomCenter }, hkl = new()
        {
            Text = cn ? "快捷键(hotkey):" : "Hotkey(temp):",
            TextAlign = ContentAlignment.BottomCenter,
            AutoSize = true
        }, bc = new() { Text = "×", AutoSize = true, BackColor = Color.Transparent, Font = new Font("Consolas", DefaultFont.Size * 1.88f) }, bm = new() { AutoSize = true, Text = "—", Font = new Font(bc.Font.Name, bc.Font.Size * 0.8f), BackColor = bc.BackColor }, bh = new() { AutoSize = true, Text = "?", BackColor = bc.BackColor, Font = bc.Font };
        ComboBox ct = new() { DropDownStyle = ComboBoxStyle.DropDownList }, hk = new() { DropDownStyle = ct.DropDownStyle };
        TextBox dv = new();
        Controls.AddRange([ct, hk, dv, dvl, hkl, bs, bc, bm, bh]);
        foreach (Control c in Controls)
        {
            if (c.BackColor != bc.BackColor)
            {
                c.Font = new Font("Segoe UI", c.Font.Size);
            }
        }
        using (var g = ct.CreateGraphics())
        {
            string strRPress = cn ? "右键长按(Right Long Press)" : "Right Long Press";
            ct.Items.AddRange([cn ? "左键(Left)" : "Left", cn ? "右键(Right)" : "Right", cn ? "左键长按(Left Long Press)" : "Left Long Press", strRPress]);
            ct.DropDownWidth = (int)g.MeasureString(strRPress, new Font("Segoe UI", DefaultFont.Size)).Width;
        }
        for (int i = 1; i < 13; i++)
        {
            hk.Items.Add($"F{i}");
        }
        hk.SelectedIndexChanged += (_, __) =>
        {
            const int hotkeyId = 0x233;
            UnregisterHotKey(Handle, hotkeyId);
            Enum.TryParse(hk.Text, out Keys key);
            RegisterHotKey(Handle, hotkeyId, 0x4000, (uint)key);
            cfg[0] = key.ToString();
            UpdateText();
        };
        dv.TextChanged += (_, __) => cfg[1] = dv.Text;
        ct.SelectedIndexChanged += (_, __) => cfg[2] = ct.SelectedIndex.ToString();
        bc.MouseEnter += (_, __) => bc.ForeColor = Color.IndianRed;
        bc.MouseLeave += (_, __) => bc.ForeColor = Color.Black;
        bc.Click += (_, __) => { Hide(); running = false; Application.Exit(); };
        bm.MouseEnter += (_, __) => bm.ForeColor = Color.MediumPurple;
        bm.MouseLeave += (_, __) => bm.ForeColor = Color.Black;
        bm.Click += (_, __) => WindowState = FormWindowState.Minimized;
        bh.MouseEnter += (_, __) => bh.ForeColor = Color.DodgerBlue;
        bh.MouseLeave += (_, __) => bh.ForeColor = Color.Black;
        bh.Click += (_, __) => System.Diagnostics.Process.Start("https://github.com/lalakii/MouseClickTool");
        bs.Click += (__, _) =>
        {
            if (!running && ct.Enabled)
            {
                if (int.TryParse(dv.Text, out int delay) && delay > -1)
                {
                    dv.ReadOnly = running = true;
                    bs.Enabled = ct.Enabled = false;
                    Task.Run(async () =>
                    {
                        for (int i = 1; i < waitSeconds; i++)
                        {
                            Invoke(() => bs.Text = string.Format("{0}", waitSeconds - i));
                            await Task.Delay(1000);
                        }
                        var downFlag = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                        var upFlag = MouseEventFlag.MOUSEEVENTF_LEFTUP;
                        bool LongPress = false;
                        Invoke(() =>
                        {
                            UpdateText();
                            if (ct.SelectedIndex == 1 || ct.SelectedIndex == 3)
                            {
                                downFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                                upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                            }
                            if (ct.SelectedIndex >= 2)
                            {
                                LongPress = true;
                            }
                        });
                        source = new TaskCompletionSource<int>();
                        var size = Marshal.SizeOf(input);
                        if (!LongPress)
                        {
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
                        }
                        else
                        {
                            bool pressed = false;
                            while (running)
                            {
                                await Task.Run(async () =>
                                {
                                    if (!pressed)
                                    {
                                        pressed = true;
                                        input.mkhi.mi.dwFlags = downFlag;
                                        SendInput(1, ref input, size);
                                    }
                                    if (delay != 0)
                                    {
                                        await Task.WhenAny(Task.Delay(delay), source.Task);
                                    }
                                });
                            }
                        }
                        source = null;
                        if (delay == 0)
                        {
                            await Task.Delay(100);
                        }
                        Invoke(() =>
                        {
                            UpdateText();
                            dv.ReadOnly = false;
                            ct.Enabled = true;
                        });
                        waitSeconds = 3;
                    });
                }
                else
                {
                    MessageBox.Show(cn ? "鼠标点击间隔必须是一个自然数" : "Mouse click intervals must be natural numbers!", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bs.Enabled = running = false;
                if (ct.SelectedIndex >= 2)
                {
                    input.mkhi.mi.dwFlags = ct.SelectedIndex == 2 ? MouseEventFlag.MOUSEEVENTF_LEFTUP : MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                    SendInput(1, ref input, Marshal.SizeOf(input));
                }
                if (source?.Task.IsCompleted == false)
                {
                    source.SetCanceled();
                }
            }
        };
        Paint += (_, e) =>
        {
            using (var g = e.Graphics)
            {
                g.DrawString(title, new Font("Candara", 12f), Brushes.Black, 5, 7);
                var p0 = new Pen(Color.MediumPurple, 7f);
                g.DrawLine(p0, Width, 0, 0, 0);
                var p1 = new Pen(Color.FromArgb(60, 0, 0, 0));
                g.DrawRectangle(p1, 5, dvl.Top - 5, Width - 12, Height - dvl.Top);
            }
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
        };
        Load += (_, _) =>
        {
            hk.Width = dv.Width = (int)DefaultFont.Size * 6;
            if (dvl.Width > hkl.Width)
            {
                dvl.Left = 8;
                hkl.Left = dvl.Left + dvl.Width - hkl.Width;
            }
            else
            {
                hkl.Left = 8;
                dvl.Left = hkl.Left + hkl.Width - dvl.Width;
            }
            const int ft = 6;
            dvl.Top = bc.Height;
            dv.Left = dvl.Left + dvl.Width + ft;
            ct.Left = dv.Left + dv.Width + ft;
            ct.Top = dvl.Top - GetDiffHeight(ct.Height, dv.Height);
            dv.Top = dvl.Top - GetDiffHeight(dv.Height, ct.Height);
            hkl.Top = dvl.Top + dvl.Height + 8;
            hk.Left = dv.Left;
            hk.Top = hkl.Top - GetDiffHeight(hk.Height, hkl.Height);
            bs.Left = ct.Left;
            bs.Width = (int)(hk.Width * 2.1);
            bs.Top = hk.Top - GetDiffHeight(bs.Height, hk.Height);
            ct.Width = bs.Width;
            Width = bs.Left + bs.Width + dvl.Left;
            Height = bs.Top + bs.Height + ft;
            bc.Left = Width - bc.Width;
            bm.Left = bc.Left - bc.Width;
            bm.Top = (bc.Height - bm.Height) / 2;
            bh.Left = bm.Left - bc.Width - 3;
        };
        var fCfg = Path.Combine(Path.GetTempPath(), "lalaki_mouse_click_tool.ini");
        if (File.Exists(fCfg))
        {
            var tCfg = File.ReadAllLines(fCfg);
            if (tCfg.Length > 2)
            {
                cfg = tCfg;
            }
        }
        hk.SelectedItem = cfg[0];
        dv.Text = cfg[1];
        int.TryParse(cfg[2], out int ctv);
        ct.SelectedIndex = ctv;
        FormClosing += (__, _) => File.WriteAllLines(fCfg, cfg);
    }

    [Flags]
    private enum MouseEventFlag : uint
    {
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.Style &= 0x800000 | 0x20000;
            return cp;
        }
    }

    [STAThread]
    public static void Main()
    {
        try { SetProcessDPIAware(); } catch { }
        Application.EnableVisualStyles();
        Application.Run(new MouseClickTool());
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x00A3) { return; }
        base.WndProc(ref m);
        if (m.Msg == 0x0312)
        {
            waitSeconds = 0;
            bs.PerformClick();
        }
        else if (m.Msg == 0x84)
        {
            m.Result = (IntPtr)0x2;
        }
    }

    private static int GetDiffHeight(int h0, int h1)
    {
        return Math.Abs(h0 - h1) / 2;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    //参考：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private void UpdateText()
    {
        var str1 = cn ? "停止" : "Stop";
        var str2 = cn ? "开始" : "Start";
        bs.Text = $"{(running ? str1 : str2)}({cfg[0]})";
        bs.Enabled = true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public int type;
        public MouseKeybdhardwareInputUnion mkhi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlag dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct MouseKeybdhardwareInputUnion
    {
        [FieldOffset(0)]
        public MouseInput mi;
    }
}