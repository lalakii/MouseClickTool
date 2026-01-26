using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("")]
public class MouseClickTool : Form
{
    private readonly System.Security.Cryptography.RNGCryptoServiceProvider p = new();
    private readonly string[] cfg;
    private Input m;
    private int wait = 3;
    private TaskCompletionSource<int>? z;

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

        var isChinese = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("zh"， StringComparison.OrdinalIgnoreCase);
        var cl = System.Globalization.CultureInfo.CurrentUICulture;
        var cn = isChinese;
        cfg = ["F1", "1000", "0", "600"， string.Empty, 
               cn ? "开始" : "Start ", cn ? "停止" : "Stop ", 
               cn ? "点击次数(Count):" : "Click Count:", cn ? "程序路径(Path):" : "Program Path:", 
               string.Empty, "False", 
               cn ? "脚本文件(File):" : "Select Script:", 
               string.Empty, "False", "MouseClickTool"];
        BackColor = dark ? Color.FromArgb(50, 50, 50) : Color.GhostWhite;
        StartPosition = FormStartPosition.CenterScreen;
        Label a0 = new() { Text = cn ? "间隔(毫秒/ms):" : "Interval/(ms):", AutoSize = true, TextAlign = ContentAlignment.BottomCenter }, d0 = new() { Text = cn ? "快捷键(Hotkey):" : "Hotkey(temp):", TextAlign = a0.TextAlign, AutoSize = true }, t2 = new() { Text = "×", AutoSize = true, BackColor = Color.Transparent, Font = new("Consolas", DefaultFont.Size * 1.88f) }, t1 = new() { AutoSize = true, Text = "—", Font = new(t2.Font.Name, t2.Font.Size * 0.8f), BackColor = t2.BackColor }, t0 = new() { AutoSize = true, Text = "?", BackColor = t2.BackColor, Font = t2.Font }, b0 = new() { AutoSize = true, TextAlign = a0.TextAlign, Text = cn ? "定时触发(Trigger):" : "Timed Trigger:" }, c0 = new() { Text = cfg[6], AutoSize = true, TextAlign = a0.TextAlign }, e0 = new() { AutoSize = true, TextAlign = ContentAlignment.BottomRight };
        ComboBox a2 = new() { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = dark ? FlatStyle.Flat : FlatStyle.System }, d1 = new() { DropDownStyle = a2.DropDownStyle, FlatStyle = a2.FlatStyle };
        DateTimePicker b1 = new() { ShowUpDown = true, Format = DateTimePickerFormat.Custom, CustomFormat = cl.DateTimeFormat.UniversalSortableDateTimePattern };
        TextBox a1 = new(), c1 = new();
        CheckBox cb0 = new() { Text = cn ? "随机扰动" : "Random Perturbation", AutoSize = true, Checked = false }, cb1 = new() { Text = cn ? "记录日志" : "Record Logs", AutoSize = true, Checked = false, };
        var runMode = 0; // 0 default, 1 createProcess, 2 runAsScript
        c1.TextChanged += (_, _) => cfg[a2.SelectedIndex switch
        {
            6 => 9,
            7 => 12,
            _ => 4,
        }] = c1.Text;
        Button d2 = new() { AutoSize = true, Tag = cfg };
        foreach (var c in (Control[])[d2, a2, d1, a1, a0, b0, d0, b1, t2, t1, t0, c0, c1, cb0, e0, cb1])
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
        a2.Items.AddRange([cn ? "左键(Left)" : "Left", cn ? "右键(Right)" : "Right", cn ? "左键长按(Left Long Press)" : "Left Long Press", strRPress, cn ? "向上滚动(Scroll Up)" : "Scroll Up", cn ? "向下滚动(Scroll Down)" : "Scroll Down", cn ? "启动程序(Launch Program)" : "Launch Program", cn ? "自定义脚本(Custom Script)" : "Custom Script"]);
        for (int i = 1; i < 13; i++)
        {
            d1.Items.Add($"F{i}");
        }

        d1.Items.AddRange(["Home", "End"]);
        const int hotkeyId = 0x233;
        d1.SelectedIndexChanged += (_, _) =>
        {
            UnregisterHotKey(Handle, hotkeyId);
            if (Enum.TryParse(d1.Text, out Keys key))
            {
                RegisterHotKey(Handle, hotkeyId, 0x4000, key);
                cfg[0] = d1.Text;
                UpdateText();
                d2.Focus();
            }
        };
        const int ft = 6;
        a1.TextChanged += (_, _) => cfg[1] = a1.Text;
        a2.SelectedIndexChanged += (_, _) =>
        {
            cfg[2] = $"{a2.SelectedIndex}";
            switch (a2.SelectedIndex)
            {
                case 6:
                    c0.Text = cfg[8];
                    c1.Text = cfg[9];
                    runMode = 1;
                    break;
                case 7:
                    c0.Text = cfg[11];
                    c1.Text = cfg[12];
                    runMode = 2;
                    break;
                default:
                    c0.Text = cfg[7];
                    c1.Text = cfg[4];
                    runMode = 0;
                    break;
            }

            c0.Left = a0.Width + a0.Left - c0.Width;
        };
        c1.Click += (_, _) =>
        {
            if (a2.SelectedIndex > 5)
            {
                using OpenFileDialog fd = new() { CheckFileExists = true, CheckPathExists = true, Multiselect = false, Filter = runMode == 1 ? "*.*|*.*" : $"*.msck ({(cn ? "MouseClickTool脚本" : "MouseClickTool Scripts")})|*.msck" };
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    c1.Text = fd.FileName;
                }
            }
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
            d1.Width = a1.Width = (int)DefaultFont.Size * 9;
            a0.Left = 8 + Math.Abs(a0.Width - b0.Width);
            b0.Left = a0.Right - b0.Width;
            a0.Top = t2.Height;
            a1.Left = a0.Right + ft;
            a2.Left = a1.Right + ft;
            a2.Top = a0.Top - HeightDiff(a2.Height, a1.Height);
            a1.Top = a0.Top - HeightDiff(a1.Height, a2.Height);
            b0.Top = a0.Bottom + ft;
            c0.Top = b0.Bottom + ft;
            d0.Top = c0.Bottom + ft;
            d0.Left = a0.Right - d0.Width;
            b1.Left = a1.Left;
            c0.Left = a0.Right - c0.Width;
            c1.Top = c0.Top - HeightDiff(c1.Height, c0.Height);
            c1.Left = c0.Right + ft;
            d1.Left = a1.Left;
            d1.Top = d0.Top - HeightDiff(d1.Height, b0.Height);
            b1.Top = b0.Top - HeightDiff(b1.Height, b0.Height);
            d2.Left = a2.Left;
            d2.Width = a2.DropDownWidth * 4 / (cn ? 7 : 5);
            d2.Top = d1.Top - HeightDiff(d2.Height, d1.Height);
            a2.Width = d2.Width;
            Width = d2.Right + 12;
            b1.Width = d2.Right - d1.Left;
            c1.Width = b1.Width;
            t2.Left = Width - t2.Width - 4;
            t1.Left = t2.Left - t2.Width;
            t1.Top = HeightDiff(t2.Height, t1.Height);
            t0.Left = t1.Left - t2.Width - 3;
            cb0.Left = d0.Left;
            cb0.Top = d2.Bottom + ft;
            e0.Top = cb0.Top - HeightDiff(cb0.Height, e0.Height);
            cb1.Left = cb0.Right + ft;
            cb1.Top = cb0.Top;
            Height = cb0.Bottom + ft;
        };
        var ini = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MouseClickTool.ini");
        if (File.Exists(ini))
        {
            var tCfg = File.ReadAllLines(ini);
            if (tCfg.Length == cfg.Length)
            {
                // 仅恢复数值设置项，防止配置文件里的旧语言字符串覆盖当前的 UI 语言
                int[] persistIndices = { 0, 1, 2, 3, 4, 9, 10, 12, 13, 14 };
                foreach (int i in persistIndices) cfg[i] = tCfg[i];
            }
        }
        
        Text = $"{cfg[14]} {(Environment.Is64BitProcess ? " x64" : " x86")}";
        int.TryParse(cfg[2], NumberStyles.Integer, cl, out int ctv);
        d1.SelectedItem = cfg[0];
        a1.Text = cfg[1];
        a2.SelectedIndex = ctv;
        _ = bool.TryParse(cfg[10], out bool r1);
        _ = bool.TryParse(cfg[13], out bool r9);
        cb1.Checked = r9;
        cb0.Checked = r1;
        cb0.CheckedChanged += (_, _) =>
        {
            r1 = cb0.Checked;
            cfg[10] = $"{r1}";
        };
        cb1.CheckedChanged += (_, _) =>
        {
            r9 = cb1.Checked;
            cfg[13] = $"{r9}";
        };
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
        byte[] r0 = new byte[4];
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
                        var tg = b1.Value < DateTime.Now;
                        ulong.TryParse(c1.Text.Trim(), NumberStyles.Integer, cl, out ulong num);
                        var unrestricted = num < 1;
                        Invoke(() =>
                        {
                            UpdateText();
                            e0.Visible = !unrestricted;
                        });
                        var runAsScript = runMode == 2;
                        string[]? scriptArr = null;
                        var scriptIndex = 0;
                        var scriptCount = 0;
                        if (runAsScript)
                        {
                            var scriptFile = c1.Text.Trim();
                            if (File.Exists(scriptFile))
                            {
                                scriptArr = File.ReadAllLines(scriptFile);
                                scriptCount = scriptArr.Length;
                            }
                            else
                            {
                                z?.TrySetCanceled();
                            }
                        }

                        for (ulong count = 0; unrestricted || count < num || longPress; count++)
                        {
                            if (z?.Task.IsCanceled == true)
                            {
                                break;
                            }

                            if (tg)
                            {
                                if (runMode == 1)
                                {
                                    CreateProcess("cmd.exe", $"/c \"{c1.Text}\"");
                                    break;
                                }
                                else if (runAsScript)
                                {
                                    // run as script
                                    if (scriptArr != null)
                                    {
                                        if (scriptIndex > scriptCount - 1)
                                        {
                                            scriptIndex = 0;
                                        }

                                        var rawLine = scriptArr[scriptIndex].Trim().TrimEnd(')');
                                        var rIndex = rawLine.IndexOf('(');
                                        string[]? scriptLine = null;
                                        if (rIndex != -1)
                                        {
                                            scriptLine = new string[2];
                                            scriptLine[0] = rawLine.Substring(0, rIndex);
                                            scriptLine[1] = rawLine.Substring(rIndex + 1);
                                        }

                                        scriptIndex++;
                                        if (scriptLine == null || scriptLine[0].StartsWith("#"))
                                        {
                                            continue;
                                        }

                                        if (scriptLine.Length > 1)
                                        {
                                            var eventType = scriptLine[0].Trim().ToLower();
                                            var scriptCommand = scriptLine[1];
                                            var args = scriptCommand.Split(',');
                                            pressed = false;
                                            if (args.Length > 1)
                                            {
                                                _ = int.TryParse(args[0], out int posX);
                                                _ = int.TryParse(args[1], out int posY);
                                                var screen = Screen.PrimaryScreen.Bounds;
                                                m.mi.dx = posX * 65535 / screen.Width;
                                                m.mi.dy = posY * 65535 / screen.Height;
                                                m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_MOVE | MouseEventFlag.MOUSEEVENTF_ABSOLUTE;
                                                SendInput(size);
                                            }

                                            if (r9)
                                            {
                                                try
                                                {
                                                    File.AppendAllText("MouseClickTool.LOG", $"[{DateTime.Now}] {eventType} {scriptCommand}\r\n");
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            switch (eventType)
                                            {
                                                case "delay":
                                                case "sleep":
                                                    if (int.TryParse(scriptCommand, out delay))
                                                    {
                                                        await Task.WhenAny(Task.Delay(delay), z?.Task);
                                                    }

                                                    break;
                                                case "left_click":
                                                    m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                                                    pressed = true;
                                                    break;
                                                case "right_click":
                                                    m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                                                    pressed = true;
                                                    break;
                                                case "left_click_long":
                                                    upFlag = MouseEventFlag.MOUSEEVENTF_LEFTUP;
                                                    if (args.Length > 2 && args[2].Contains("1"))
                                                    {
                                                        upFlag = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                                                    }

                                                    m.mi.dwFlags = upFlag;
                                                    SendInput(size);
                                                    break;
                                                case "right_click_long":
                                                    upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                                                    if (args.Length > 2 && args[2].Contains("1"))
                                                    {
                                                        upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                                                    }

                                                    m.mi.dwFlags = upFlag;
                                                    SendInput(size);
                                                    break;
                                                case "mouse_wheel":
                                                    m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_WHEEL;
                                                    int.TryParse(args[0], NumberStyles.Integer, cl, out int sc);
                                                    m.mi.mouseData = sc;
                                                    break;
                                                case "create_process":
                                                    CreateProcess("cmd.exe", $"/c {scriptCommand}");
                                                    continue;
                                                case "title":
                                                    Text = scriptCommand.Trim('\"');
                                                    cfg[14] = Text;
                                                    Invoke((MethodInvoker)Invalidate);
                                                    continue;
                                                case "exit":
                                                case "quit":
                                                    InvokeOnClick(t2, null);
                                                    continue;
                                                case "once":
                                                case "break":
                                                    z?.TrySetCanceled();
                                                    continue;
                                                default:
                                                    continue;
                                            }

                                            SendInput(size);
                                            if (pressed)
                                            {
                                                m.mi.dwFlags = m.mi.dwFlags == MouseEventFlag.MOUSEEVENTF_LEFTDOWN ? MouseEventFlag.MOUSEEVENTF_LEFTUP : MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                                                SendInput(size);
                                            }
                                        }

                                        continue;
                                    }
                                }
                                else
                                {
                                    if (!pressed || mouseWheel)
                                    {
                                        m.mi.dwFlags = downFlag;
                                        SendInput(size);
                                    }

                                    if (!longPress)
                                    {
                                        m.mi.dwFlags = upFlag;
                                        SendInput(size);
                                        Invoke(() =>
                                        {
                                            e0.Text = $"{(cn ? "剩余次数" : "Remaining Runs")}:{num - count - 1}";
                                            e0.Left = Width - e0.Width - 12;
                                        });
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
                                int r5 = delay;
                                if (r1)
                                {
                                    // 随机系数：0.8 ~ 1.2
                                    p.GetBytes(r0);
                                    int r2 = BitConverter.ToInt32(r0, 0);
                                    double r3 = (r2 & 0x7FFFFFFF) / (double)0x7FFFFFFF;
                                    double r4 = (r3 * (1.2 - 0.8)) + 0.8;
                                    r5 = (int)Math.Round(delay * r4);
                                }

                                await Task.WhenAny(Task.Delay(r5), z?.Task);
                            }
                        }

                        if (longPress && !mouseWheel)
                        {
                            SendInput(size);
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
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_ABSOLUTE = 0x8000,
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
        ThreadPool.UnsafeQueueUserWorkItem(
            _ =>
        {
            try
            {
                System.Diagnostics.Process.Start(path, args);
            }
            catch
            {
            }
        },
            0);
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

    private void SendInput(int cbSize)
    {
        _ = SendInput(1, ref m, cbSize);
    }

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

