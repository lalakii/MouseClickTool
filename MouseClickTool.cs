﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("")]
public class MouseClickTool : Form
{
    private readonly Button bs = new() { AutoSize = true };
    private readonly string[] cfg = ["F1", "1000", "0", "600"];
    private readonly bool cn = System.Globalization.CultureInfo.InstalledUICulture.Name.IndexOf("zh-", StringComparison.OrdinalIgnoreCase) > -1;
    private Input input;
    private TaskCompletionSource<int> ss;
    private int wait = 3;

    public MouseClickTool()
    {
        Application.EnableVisualStyles();
        var isDark = false;
        try { isDark = ShouldSystemUseDarkMode(); SetProcessDPIAware(); } catch { }
        Text = $"MouseClickTool {(Environment.Is64BitProcess ? " x64" : " x86")}";
        BackColor = isDark ? Color.FromArgb(50, 50, 50) : Color.GhostWhite;
        StartPosition = FormStartPosition.CenterScreen;
        Label dvl = new() { Text = cn ? "间隔(毫秒/ms):" : "Interval/(ms):", AutoSize = true, TextAlign = ContentAlignment.BottomCenter }, hkl = new() { Text = cn ? "快捷键(hotkey):" : "Hotkey(temp):", TextAlign = ContentAlignment.BottomCenter, AutoSize = true }, bc = new() { Text = "×", AutoSize = true, BackColor = Color.Transparent, Font = new("Consolas", DefaultFont.Size * 1.88f) }, bm = new() { AutoSize = true, Text = "—", Font = new(bc.Font.Name, bc.Font.Size * 0.8f), BackColor = bc.BackColor }, bh = new() { AutoSize = true, Text = "?", BackColor = bc.BackColor, Font = bc.Font };
        ComboBox ct = new() { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = isDark ? FlatStyle.Flat : FlatStyle.System }, hk = new() { DropDownStyle = ct.DropDownStyle, FlatStyle = ct.FlatStyle };
        TextBox dv = new();
        foreach (Control c in (Control[])[ct, hk, dv, dvl, hkl, bs, bc, bm, bh])
        {
            if (isDark)
            {
                c.ForeColor = Color.GhostWhite;
            }
            if (c.BackColor != bc.BackColor)
            {
                c.Font = new("Segoe UI", c.Font.Size);
                if (isDark)
                {
                    c.BackColor = BackColor;
                }
            }
            Controls.Add(c);
        }
        var strRPress = cn ? "右键长按(Right Long Press)" : "Right Long Press";
        ct.DropDownWidth = TextRenderer.MeasureText(strRPress, ct.Font).Width;
        ct.Items.AddRange([cn ? "左键(Left)" : "Left", cn ? "右键(Right)" : "Right", cn ? "左键长按(Left Long Press)" : "Left Long Press", strRPress, cn ? "向上滚动(Scroll Up)" : "Scroll Up", cn ? "向下滚动(Scroll Down)" : "Scroll Down"]);
        for (int i = 1; i < 13; i++)
        {
            hk.Items.Add($"F{i}");
        }
        const int hotkeyId = 0x233;
        hk.SelectedIndexChanged += (_, __) =>
        {
            UnregisterHotKey(Handle, hotkeyId);
            Enum.TryParse(hk.Text, out Keys key);
            RegisterHotKey(Handle, hotkeyId, 0x4000, key);
            cfg[0] = hk.Text;
            UpdateText();
        };
        dv.TextChanged += (_, __) => cfg[1] = dv.Text;
        ct.SelectedIndexChanged += (_, __) => cfg[2] = ct.SelectedIndex.ToString();
        bc.MouseEnter += (_, __) => bc.ForeColor = Color.IndianRed;
        bc.MouseLeave += (_, __) => bc.ForeColor = bs.ForeColor;
        bc.Click += (_, __) => { Hide(); ss?.TrySetCanceled(); Application.Exit(); };
        bm.MouseEnter += (_, __) => bm.ForeColor = Color.MediumPurple;
        bm.MouseLeave += (_, __) => bm.ForeColor = bs.ForeColor;
        bm.Click += (_, __) => WindowState = FormWindowState.Minimized;
        bh.MouseEnter += (_, __) => bh.ForeColor = Color.DodgerBlue;
        bh.MouseLeave += (_, __) => bh.ForeColor = bs.ForeColor;
        bh.Click += (_, __) => System.Diagnostics.Process.Start("https://mouseclicktool.sourceforge.io");
        bs.Click += (__, _) =>
        {
            bs.Enabled = false;
            if (ct.Enabled && ss == null)
            {
                if (int.TryParse(dv.Text, out int delay) && delay > -1)
                {
                    dv.Enabled = ct.Enabled = false;
                    var downFlag = MouseEventFlag.MOUSEEVENTF_LEFTDOWN;
                    var upFlag = MouseEventFlag.MOUSEEVENTF_LEFTUP;
                    if ((ct.SelectedIndex & 1) == 1)
                    {
                        downFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                        upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                    }
                    var MouseWheel = ct.SelectedIndex > 3;
                    if (MouseWheel)
                    {
                        downFlag = MouseEventFlag.MOUSEEVENTF_WHEEL;
                        int.TryParse(cfg[3], out int sc);
                        input.mkhi.mi.mouseData = ct.SelectedIndex > 4 ? -sc : sc;
                    }
                    var LongPress = ct.SelectedIndex > 1;
                    Task.Run(async () =>
                    {
                        for (int i = 1; i < wait; i++)
                        {
                            Invoke(() => bs.Text = $"{wait - i}");
                            await Task.Delay(1000);
                        }
                        var pressed = false;
                        var size = Marshal.SizeOf(input);
                        ss = new();
                        Invoke(() => UpdateText());
                        while (ss?.Task.IsCanceled == false)
                        {
                            await Task.Run(async () =>
                            {
                                if (!pressed || MouseWheel)
                                {
                                    input.mkhi.mi.dwFlags = downFlag;
                                    SendInput(1, ref input, size);
                                }
                                if (!LongPress)
                                {
                                    input.mkhi.mi.dwFlags = upFlag;
                                    SendInput(1, ref input, size);
                                }
                                else
                                {
                                    input.mkhi.mi.dwFlags = upFlag;
                                    pressed = true;
                                }
                                if (delay != 0)
                                {
                                    await Task.WhenAny(Task.Delay(delay), ss?.Task);
                                }
                            });
                        }
                        if (LongPress && !MouseWheel)
                        {
                            SendInput(1, ref input, size);
                        }
                        wait = 3;
                        await Task.Delay(delay == 0 ? 1 : 0);
                        ss = null;
                        Invoke(() => { dv.Enabled = ct.Enabled = true; UpdateText(); });
                    });
                }
                else
                {
                    MessageBox.Show(cn ? "鼠标点击间隔必须是一个自然数" : "Mouse click intervals must be natural numbers", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ss?.TrySetCanceled();
            }
        };
        Paint += (_, e) =>
        {
            using (var g = e.Graphics)
            {
                g.DrawString(Text, new("Candara", 12f), new SolidBrush(bs.ForeColor), 5, 7);
                Pen p = new(Color.MediumPurple, 7f);
                g.DrawLine(p, Width, 0, 0, 0);
                p.Color = isDark ? Color.LightGray : Color.FromArgb(50, 0, 0, 0);
                p.Width = .1f;
                g.DrawRectangle(p, 5, dvl.Top - 5, Width - 12, Height - dvl.Top);
            }
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : WindowState;
        };
        Load += (_, _) =>
        {
            hk.Width = dv.Width = (int)DefaultFont.Size * 8;
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
            ct.Top = dvl.Top - DiffHeight(ct.Height, dv.Height);
            dv.Top = dvl.Top - DiffHeight(dv.Height, ct.Height);
            hkl.Top = dvl.Top + dvl.Height + 8;
            hk.Left = dv.Left;
            hk.Top = hkl.Top - DiffHeight(hk.Height, hkl.Height);
            bs.Left = ct.Left;
            bs.Width = ct.DropDownWidth / 2;
            bs.Top = hk.Top - DiffHeight(bs.Height, hk.Height);
            ct.Width = bs.Width;
            Width = bs.Left + bs.Width + dvl.Left;
            Height = bs.Top + bs.Height + ft;
            bc.Left = Width - bc.Width - 4;
            bm.Left = bc.Left - bc.Width;
            bm.Top = (bc.Height - bm.Height) / 2;
            bh.Left = bm.Left - bc.Width - 3;
        };
        var fCfg = Path.Combine(Path.GetTempPath(), "lalaki_mouse_click_tool.ini");
        if (File.Exists(fCfg))
        {
            var tCfg = File.ReadAllLines(fCfg);
            cfg = (tCfg.Length == cfg.Length) ? tCfg : cfg;
        }
        int.TryParse(cfg[2], out int ctv);
        hk.SelectedItem = cfg[0];
        dv.Text = cfg[1];
        ct.SelectedIndex = ctv;
        FormClosing += (__, _) => { try { File.WriteAllLines(fCfg, cfg); } catch { } };
        Application.Run(this);
    }

    [Flags]
    private enum MouseEventFlag : uint
    {
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_WHEEL = 0x0800
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.Style = 0x20000;
            return cp;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg != 0x00A3)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0312)
            {
                wait = 0;
                bs.PerformClick();
            }
            else if (m.Msg == 0x84)
            {
                m.Result = (IntPtr)0x2;
            }
        }
    }

    private static int DiffHeight(int h0, int h1)
    {
        int value = h0 - h1;
        int mask = value >> 31;
        return ((value + mask) ^ mask) / 2;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

    //参考：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("UXTheme.dll", EntryPoint = "#138")]
    private static extern bool ShouldSystemUseDarkMode();

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private void UpdateText()
    {
        var str1 = cn ? "开始" : "Start";
        var str2 = cn ? "停止" : "Stop";
        bs.Text = $"{(ss == null ? str1 : str2)}({cfg[0]})";
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
        public int mouseData;
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