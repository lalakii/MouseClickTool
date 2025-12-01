using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyVersion("2.9.1.0")]

[System.ComponentModel.DesignerCategory("")]
public class MouseClickTool : Form
{
    private readonly Random useRandom = new Random();
    private Input m;
    private int wait = 3;
    private TaskCompletionSource<int>? z;
    private bool useRandomInterval;
    private string[] cfg;

    public MouseClickTool()
    {
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        Application.EnableVisualStyles();
        var dark = false;
        try
        {
            dark = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) == 0;
            SetProcessDPIAware();
        }
        catch
        {
        }

        var cl = InputLanguage.CurrentInputLanguage.Culture;
        var cn = cl.Name.IndexOf("zh-", StringComparison.OrdinalIgnoreCase) > -1;
        cfg = ["F1", "1000", "0", "600", string.Empty, cn ? "开始" : "Start ", cn ? "停止" : "Stop ", cn ? "点击次数(Count):" : "Click Count:", cn ? "程序路径(Path):" : "Program Path:", string.Empty];
        Text = $"MouseClickTool {(Environment.Is64BitProcess ? " x64" : " x86")}";
        BackColor = dark ? Color.FromArgb(50, 50, 50) : Color.GhostWhite;
        StartPosition = FormStartPosition.CenterScreen;
        Label a0 = new() { Text = cn ? "间隔(毫秒/ms):" : "Interval/(ms):", AutoSize = true, TextAlign = ContentAlignment.BottomCenter }, d0 = new() { Text = cn ? "快捷键(Hotkey):" : "Hotkey(temp):", TextAlign = a0.TextAlign, AutoSize = true }, t2 = new() { Text = "×", AutoSize = true, BackColor = Color.Transparent, Font = new("Consolas", DefaultFont.Size * 1.88f) }, t1 = new() { AutoSize = true, Text = "—", Font = new(t2.Font.Name, t2.Font.Size * 0.8f), BackColor = t2.BackColor }, t0 = new() { AutoSize = true, Text = "?", BackColor = t2.BackColor, Font = t2.Font }, b0 = new() { AutoSize = true, TextAlign = a0.TextAlign, Text = cn ? "定时触发(Trigger):" : "Timed Trigger:" }, c0 = new() { Text = cfg[6], AutoSize = true, TextAlign = a0.TextAlign };
        ComboBox a2 = new() { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = dark ? FlatStyle.Flat : FlatStyle.System }, d1 = new() { DropDownStyle = a2.DropDownStyle, FlatStyle = a2.FlatStyle };
        DateTimePicker b1 = new() { ShowUpDown = true, Format = DateTimePickerFormat.Custom, CustomFormat = cl.DateTimeFormat.UniversalSortableDateTimePattern };
        TextBox a1 = new(), c1 = new();

        CheckBox randomCheckBox = new() { Text = "开启随机间隔", AutoSize = true, Checked = false };
        randomCheckBox.CheckedChanged += (sender, e) =>
        {
            useRandomInterval = randomCheckBox.Checked;
        };

        var startApp = false;
        c1.TextChanged += (_, _) => cfg[startApp ? 9 : 4] = c1.Text;
        Button d2 = new() { AutoSize = true, Tag = cfg };
        foreach (var c in (Control[])[d2, a2, d1, a1, a0, b0, d0, b1, t2, t1, t0, c0, c1, randomCheckBox])
        {
            if (dark)
            {
                c.ForeColor = Color.GhostWhite;
            }

            if (c.BackColor != t2.BackColor)
            {
                c.Font = new("Segoe UI", c.Font.Size);
                if (dark)
                {
                    c.BackColor = BackColor;
                }
            }

            Controls.Add(c);
        }

        var strRPress = cn ? "右键长按(Right Long Press)" : "Right Long Press";
        a2.DropDownWidth = TextRenderer.MeasureText(strRPress, a2.Font).Width;
        a2.Items.AddRange([cn ? "左键(Left)" : "Left", cn ? "右键(Right)" : "Right", cn ? "左键长按(Left Long Press)" : "Left Long Press", strRPress, cn ? "向上滚动(Scroll Up)" : "Scroll Up", cn ? "向下滚动(Scroll Down)" : "Scroll Down", cn ? "启动程序(Launch Program)" : "Launch Program"]);
        for (int i = 1; i < 13; i++)
        {
            d1.Items.Add($"F{i}");
        }

        const int hotkeyId = 0x233;
        d1.SelectedIndexChanged += (_, _) =>
        {
            UnregisterHotKey(Handle, hotkeyId);
            if (Enum.TryParse(d1.Text, out Keys key))
            {
                RegisterHotKey(Handle, hotkeyId, 0x4000, key);
                cfg[0] = d1.Text;
                UpdateText();
            }
        };
        const int ft = 6;
        a1.TextChanged += (_, _) => cfg[1] = a1.Text;
        a2.SelectedIndexChanged += (_, _) =>
        {
            cfg[2] = $"{a2.SelectedIndex}";
            startApp = a2.SelectedIndex == a2.Items.Count - 1;
            if (startApp)
            {
                c1.Text = cfg[9];
                c0.Text = cfg[8];
            }
            else
            {
                c1.Text = cfg[4];
                c0.Text = cfg[7];
            }

            c0.Left = a0.Width + a0.Left - c0.Width;
        };
        t2.MouseEnter += (_, _) => t2.ForeColor = Color.IndianRed;
        t2.MouseLeave += (_, _) => t2.ForeColor = d2.ForeColor;
        t2.Click += (_, _) =>
        {
            Hide();
            z?.TrySetCanceled();
            Application.Exit();
        };
        t1.MouseEnter += (_, _) => t1.ForeColor = Color.MediumPurple;
        t1.MouseLeave += (_, _) => t1.ForeColor = d2.ForeColor;
        t1.Click += (_, _) => WindowState = FormWindowState.Minimized;
        t0.MouseEnter += (_, _) => t0.ForeColor = Color.DodgerBlue;
        t0.MouseLeave += (_, _) => t0.ForeColor = d2.ForeColor;
        t0.Click += (_, _) => CreateProcess("https://mouseclicktool.sourceforge.io", null);
        Paint += (_, e) =>
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : WindowState;
            using var g = e.Graphics;
            g.DrawString(Text, new("Candara", 12f), new SolidBrush(d2.ForeColor), 5, 7);
            Pen p = new(Color.MediumPurple, 7f);
            g.DrawLine(p, Width, 0, 0, 0);
            p.Color = dark ? Color.LightGray : Color.FromArgb(50, 0, 0, 0);
            p.Width = .1f;
            g.DrawRectangle(p, 5, a0.Top - 5, Width - 12, Height - a0.Top);
        };
        Load += (_, _) =>
        {
            d1.Width = a1.Width = (int)DefaultFont.Size * 10;
            Control a = a0, b = b0;
            if (b.Width > a.Width)
            {
                a = b0;
                b = a0;
            }

            a.Left = 8;
            b.Left = a.Left + a.Width - b.Width;
            a0.Top = t2.Height;
            a1.Left = a0.Left + a0.Width + ft;
            a2.Left = a1.Left + a1.Width + ft;
            a2.Top = a0.Top - HeightDiff(a2.Height, a1.Height);
            a1.Top = a0.Top - HeightDiff(a1.Height, a2.Height);
            b0.Top = a0.Top + a0.Height + 8;
            c0.Top = b0.Top + b0.Height + 8;
            d0.Top = c0.Top + c0.Height + 8;
            d0.Left = a0.Width + a0.Left - d0.Width;
            b1.Left = a1.Left;
            c0.Left = a0.Width + a0.Left - c0.Width;
            c1.Top = c0.Top - HeightDiff(c1.Height, c0.Height);
            c1.Left = c0.Left + c0.Width + ft;
            d1.Left = a1.Left;
            d1.Top = d0.Top - HeightDiff(d1.Height, b0.Height);
            b1.Top = b0.Top - HeightDiff(b1.Height, b0.Height);
            d2.Left = a2.Left;
            d2.Width = a2.DropDownWidth * 4 / (cn ? 8 : 5);
            d2.Top = d1.Top - HeightDiff(d2.Height, d1.Height);
            a2.Width = d2.Width;
            c1.Width = a2.Left - a0.Left - a0.Width + a2.Width - ft;
            Width = d2.Left + d2.Width + 12;
            b1.Width = d2.Width + d1.Width + ft;
            Height = d2.Top + d2.Height + ft;
            t2.Left = Width - t2.Width - 4;
            t1.Left = t2.Left - t2.Width;
            t1.Top = HeightDiff(t2.Height, t1.Height);
            t0.Left = t1.Left - t2.Width - 3;

            randomCheckBox.Left = d0.Left;
            randomCheckBox.Top = d2.Bottom + 8;
            this.Height = randomCheckBox.Bottom + 15;
        };
        var ini = Path.Combine(Path.GetTempPath(), $"MouseClickTool_{(cn ? "zh" : "en")}.ini");
        if (File.Exists(ini))
        {
            var tCfg = File.ReadAllLines(ini);
            cfg = (tCfg.Length == cfg.Length) ? tCfg : cfg;
        }

        int.TryParse(cfg[2], NumberStyles.Integer, cl, out int ctv);
        d1.SelectedItem = cfg[0];
        a1.Text = cfg[1];
        a2.SelectedIndex = ctv;
        FormClosing += (_, _) =>
        {
            try
            {
                File.WriteAllLines(ini, cfg);
            }
            catch
            {
            }
        };
        d2.Click += (_, _) =>
        {
            d2.Enabled = false;
            if (a2.Enabled && z == null)
            {
                if (int.TryParse(a1.Text, out int delay) && delay > -1)
                {
                    a1.Enabled = a2.Enabled = b1.Enabled = c1.Enabled = false;
                    var downFlag = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                    var upFlag = MouseEventFlag.MOUSEEVENTF_LEFTUP;
                    if ((a2.SelectedIndex & 1) == 1)
                    {
                        downFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                        upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                    }

                    var mouseWheel = a2.SelectedIndex > 3;
                    if (mouseWheel)
                    {
                        downFlag = MouseEventFlag.MOUSEEVENTF_WHEEL;
                        int.TryParse(cfg[3], NumberStyles.Integer, cl, out int sc);
                        m.mi.mouseData = a2.SelectedIndex > 4 ? -sc : sc;
                    }

                    var longPress = a2.SelectedIndex > 1;
                    Task.Run(async () =>
                    {
                        for (int i = 1; i < wait; i++)
                        {
                            Invoke(() => d2.Text = $"{wait - i}");
                            await Task.Delay(1000);
                        }

                        var pressed = false;
                        var size = Marshal.SizeOf(m);
                        z = new();
                        Invoke(() => UpdateText());
                        var tg = b1.Value < DateTime.Now;
                        uint.TryParse(c1.Text.Trim(), NumberStyles.Integer, cl, out uint num);
                        for (ulong count = 0; num < 1 || count < num; count++)
                        {
                            if (z?.Task.IsCanceled == true)
                            {
                                break;
                            }

                            if (tg)
                            {
                                if (startApp)
                                {
                                    int idx = c1.Text.IndexOf(' ');
                                    string? args = null;
                                    if (idx != -1)
                                    {
                                        args = c1.Text.Substring(idx);
                                    }

                                    CreateProcess(c1.Text.Split(' ')[0], args);
                                    break;
                                }
                                else
                                {
                                    if (!pressed || mouseWheel)
                                    {
                                        m.mi.dwFlags = downFlag;
                                        _ = SendInput(1, ref m, size);
                                    }

                                    if (!longPress)
                                    {
                                        m.mi.dwFlags = upFlag;
                                        _ = SendInput(1, ref m, size);
                                    }
                                    else
                                    {
                                        m.mi.dwFlags = upFlag;
                                        pressed = true;
                                    }
                                }
                            }
                            else
                            {
                                tg = b1.Value < DateTime.Now;
                            }

                            if (delay != 0)
                            {
                                int actualDelay = delay;
                                if (useRandomInterval)
                                {
                                    // 随机系数：0.8 ~ 1.2
                                    double randomFactor = (useRandom.NextDouble() * 0.4) + 0.8;
                                    actualDelay = (int)Math.Round(delay * randomFactor);
                                }

                                await Task.WhenAny(Task.Delay(actualDelay), z?.Task);
                            }
                        }

                        if (longPress && !mouseWheel)
                        {
                            _ = SendInput(1, ref m, size);
                        }

                        await Task.Delay(delay == 0 ? 5 : 0);
                        wait = 3;
                        z = null;
                        Invoke(() =>
                        {
                            a1.Enabled = a2.Enabled = b1.Enabled = c1.Enabled = true;
                            b1.Value = DateTime.Now;
                            UpdateText();
                        });
                    });
                }
                else
                {
                    MessageBox.Show(cn ? "鼠标点击间隔必须是一个自然数" : "Mouse click intervals must be natural numbers", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                z?.TrySetCanceled();
            }
        };
        Application.Run(this);
    }

    [Flags]
    private enum MouseEventFlag
    {
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_WHEEL = 0x0800,
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.Style &= 0x20000 | 0x800000;
            return cp;
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == 0x84)
        {
            m.Result = (IntPtr)0x2;
        }
        else if (m.Msg == 0x0312)
        {
            wait = 0;
            ((Button)Controls[0]).PerformClick();
        }
    }

    private static void CreateProcess(string path, string? args)
    {
        new Thread(() =>
        {
            try
            {
                System.Diagnostics.Process.Start(path, args);
            }
            catch
            {
            }
        }).Start();
    }

    private static int HeightDiff(int h0, int h1)
    {
        int value = h0 - h1;
        int mask = value >> 31;
        return ((value + mask) ^ mask) / 2;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vk);

    // 参考：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
    [DllImport("user32.dll")]
    private static extern int SendInput(int nInputs, ref Input pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private void UpdateText()
    {
        Controls[0].Text = $"{(z == null ? cfg[5] : cfg[6])}({cfg[0]})";
        Controls[0].Enabled = true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public int type;
        public MouseInput mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int dx;
        public int dy;
        public int mouseData;
        public MouseEventFlag dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    }
}