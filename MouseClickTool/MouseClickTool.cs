using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("")]
public class MouseClickTool : Form
{
    private readonly System.Security.Cryptography.RNGCryptoServiceProvider p = new();
    private readonly Dictionary<byte, string> lang = [];
    private readonly string[] cfg;
    private TaskCompletionSource<int>? z;
    private int wait = 3;
    private Input m;
    private IntPtr hh;
    private LLMP? hp;
    private int ht;

    public MouseClickTool()
    {
        Application.EnableVisualStyles();
        AutoScaleMode = AutoScaleMode.Dpi;
        var cl = CultureInfo.CurrentUICulture;
        var dark = false;
        try
        {
            dark = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) == 0;
            SetProcessDpiAwarenessContext((IntPtr)(-4)); // v2
        }
        catch
        {
        }

        cfg = ["F1", "1000", "0", "600", string.Empty, string.Empty, "False", string.Empty, "False", "MouseClickTool", string.Empty, string.Empty];
        var ini = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MouseClickTool_V3.3.ini");
        if (File.Exists(ini))
        {
            var tCfg = File.ReadAllLines(ini);
            if (tCfg.Length == cfg.Length)
            {
                cfg = tCfg;
            }
        }

        if (cl.Name.Contains("zh"))
        {
            lang.Add(0, "开始");
            lang.Add(1, "停止");
            lang.Add(2, "点击次数(Count)");
            lang.Add(3, "程序路径(Path)");
            lang.Add(4, "脚本文件(File)");
            lang.Add(5, "间隔(毫秒/ms)");
            lang.Add(6, "快捷键(Hotkey)");
            lang.Add(7, "定时触发(Trigger)");
            lang.Add(8, "随机扰动");
            lang.Add(9, "记录日志");
            lang.Add(10, "右键长按(Right Long Press)");
            lang.Add(11, "左键(Left)");
            lang.Add(12, "右键(Right)");
            lang.Add(13, "左键长按(Left Long Press)");
            lang.Add(14, "向上滚动(Scroll Up)");
            lang.Add(15, "向下滚动(Scroll Down)");
            lang.Add(16, "启动程序(Launch Program)");
            lang.Add(17, "自定义脚本(Custom Script)");
            lang.Add(18, "鼠标中键(Middle)");
            lang.Add(19, "自定义(Custom)");
            lang.Add(20, "按下组合键(Esc取消)");
            lang.Add(21, "录制中: 按 Ctrl/Alt/Shift+主键, Esc 取消");
            lang.Add(22, "快捷键 {0} 注册失败，可能已被其他程序占用");
            lang.Add(23, "按 Del 键删除此快捷键");
            lang.Add(24, "需含 Ctrl/Alt/Shift (Esc取消)");
            lang.Add(25, "剩余次数");
            lang.Add(26, "MouseClickTool 脚本");
            lang.Add(27, "窗体缩放");
            lang.Add(28, "语言/Language");
            lang.Add(29, "默认语言");
            lang.Add(30, "获取帮助");
        }
        else
        {
            lang.Add(0, "Start");
            lang.Add(1, "Stop");
            lang.Add(2, "Click Count");
            lang.Add(3, "Program Path");
            lang.Add(4, "Script File");
            lang.Add(5, "Interval (ms)");
            lang.Add(6, "Hotkey");
            lang.Add(7, "Scheduled Trigger");
            lang.Add(8, "Random Jitter");
            lang.Add(9, "Log Output");
            lang.Add(10, "Right Long Press");
            lang.Add(11, "Left Click");
            lang.Add(12, "Right Click");
            lang.Add(13, "Left Long Press");
            lang.Add(14, "Scroll Up");
            lang.Add(15, "Scroll Down");
            lang.Add(16, "Launch Program");
            lang.Add(17, "Custom Script");
            lang.Add(18, "Middle Click");
            lang.Add(19, "Custom");
            lang.Add(20, "Press key combination (Esc to cancel)");
            lang.Add(21, "Recording: Press Ctrl/Alt/Shift + key, Esc to cancel");
            lang.Add(22, "Failed to register hotkey {0}. It might be used by another app.");
            lang.Add(23, "Press Del to clear hotkey");
            lang.Add(24, "Must include Ctrl/Alt/Shift (Esc to cancel)");
            lang.Add(25, "Remaining Clicks");
            lang.Add(26, "MouseClickTool Script");
            lang.Add(27, "UI Scale");
            lang.Add(28, "Language");
            lang.Add(29, "Default Language");
            lang.Add(30, "Get Help");
        }

        var langIni = string.Empty;
        if (!string.IsNullOrWhiteSpace(cfg[11]))
        {
            langIni = $"{cfg[11]}.ini";
        }

        if (File.Exists(langIni))
        {
            foreach (var it in File.ReadAllLines(langIni))
            {
                var arr = it.Split('=');
                if (arr.Length > 1)
                {
                    var key = arr[0].Trim();
                    if (byte.TryParse(key, out byte keyInt))
                    {
                        var value = arr[1].Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            lang[keyInt] = value;
                        }
                    }
                }
            }
        }

        var colorPrimary = Color.FromArgb(255, 93, 89, 214);
        BackColor = dark ? Color.FromArgb(255, 32, 32, 32) : Color.GhostWhite;
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true; // 焦点在下拉框/按钮上时窗体也要先收到按键，否则组合键捕获与 Esc 取消都不会触发
        Label a0 = new() { Text = $"{lang[5]} : ", AutoSize = true, TextAlign = ContentAlignment.BottomCenter }, d0 = new() { Text = $"{lang[6]} : ", TextAlign = a0.TextAlign, AutoSize = true }, t2 = new() { Text = "×", AutoSize = true, Font = new("Consolas", DefaultFont.Size * 1.88f) }, t1 = new() { AutoSize = true, Text = "—", Font = new(t2.Font.Name, t2.Font.Size * 0.8f, FontStyle.Bold) }, b0 = new() { AutoSize = true, TextAlign = a0.TextAlign, Text = $"{lang[7]} : " }, c0 = new() { Text = lang[1], AutoSize = true, TextAlign = ContentAlignment.MiddleCenter }, e0 = new() { AutoSize = true, TextAlign = ContentAlignment.MiddleRight }, d3 = new() { AutoSize = false, TextAlign = ContentAlignment.MiddleCenter }, t3 = new() { Text = "\uE944", AutoSize = true, Font = new("Segoe Fluent Icons", 13f) };
        ComboBox a2 = new() { DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = dark ? FlatStyle.Flat : FlatStyle.System }, d1 = new() { DropDownStyle = a2.DropDownStyle, FlatStyle = a2.FlatStyle };
        DateTimePicker b1 = new() { ShowUpDown = true, Format = DateTimePickerFormat.Custom, CustomFormat = cl.DateTimeFormat.UniversalSortableDateTimePattern };
        TextBox a1 = new(), c1 = new();
        CheckBox cb0 = new() { Text = lang[8], AutoSize = true, Checked = false }, cb1 = new() { Text = lang[9], AutoSize = true, Checked = false, };
        var runMode = 0; // 0 default, 1 createProcess, 2 runAsScript
        c1.TextChanged += (_, _) => cfg[a2.SelectedIndex switch
        {
            6 => 5,
            7 => 7,
            _ => 4,
        }] = c1.Text;

        // Label h0 = new() { AutoSize = true, Visible = false }; // 快捷键操作提示(录制中/可删除)
        Button d2 = new() { FlatStyle = dark ? FlatStyle.Flat : FlatStyle.System };
        if (!float.TryParse(cfg[10], NumberStyles.Integer, cl, out float fontScale))
        {
            fontScale = 1f;
        }
        else
        {
            fontScale /= 100f;
        }

        foreach (var c in (Control[])[d2, a2, d1, a1, a0, b0, d0, b1, t2, t1, c0, c1, cb0, e0, cb1, d3, t3])
        {
            if (dark)
            {
                c.BackColor = BackColor;
                c.ForeColor = Color.GhostWhite;
            }

            if (c != t1 && c != t2 && c != t3)
            {
                c.Font = new("Segoe UI", c.Font.Size * fontScale, FontStyle.Regular);
            }
            else
            {
                c.Font = new(c.Font.FontFamily, c.Font.Size * fontScale);
                c.BackColor = Color.Transparent;
            }

            if (fontScale > 1 && c is CheckBox cbx)
            {
                var size = (int)(16 * fontScale);
                var x = SystemInformation.MenuCheckSize.Width - 5;
                var y = c.Bottom - SystemInformation.MenuCheckSize.Height;
                c.Padding = new(size, 0, 0, 0);
                cbx.Appearance = Appearance.Normal;
                cbx.FlatStyle = FlatStyle.Flat;
                cbx.FlatAppearance.BorderSize = 1;
                c.Paint += (_, cbe) =>
                {
                    ControlPaint.DrawCheckBox(cbe.Graphics, x, y, size, size, cbx.Checked ? ButtonState.Checked : ButtonState.Normal);
                };
            }

            Controls.Add(c);
        }

        var strRPress = lang[10];
        a2.DropDownWidth = TextRenderer.MeasureText(strRPress, a2.Font).Width + SystemInformation.VerticalScrollBarArrowHeight;
        a2.Items.AddRange([lang[11], lang[12], lang[13], strRPress, lang[14], lang[15], lang[16], lang[17]]);
        for (int i = 1; i < 13; i++)
        {
            d1.Items.Add($"F{i}");
        }

        var middle = lang[18];
        var custom = lang[19];
        d1.Items.AddRange(["Home", "End", middle, custom]);

        // 预留最长组合键文本，避免下拉列表与收起状态截断
        var mw = new int[] { TextRenderer.MeasureText(middle, d1.Font).Width, TextRenderer.MeasureText(custom, d1.Font).Width, SystemInformation.VerticalScrollBarArrowHeight + TextRenderer.MeasureText("Ctrl+Alt+Shift+F12", d1.Font).Width }.Max();
        d1.DropDownWidth = mw;
        const int hotkeyId = 0x233;
        var hk = false;
        d1.SelectedIndexChanged += (_, _) =>
        {
            UnregisterHotKey(Handle, hotkeyId);
            UnhookMouse();
            hk = false;
            if (d1.Text == custom)
            {
                hk = true;
                d2.Text = lang[20];
                d2.Enabled = false;
                e0.Text = lang[21];
                e0.Left = Width - e0.Width - 12;

                // h0.Visible = true;
                return;
            }

            if (d1.Text == middle)
            {
                HookMouse();
            }
            else if (TryParseHotkey(d1.Text, out int mods, out var key))
            {
                if (!RegisterHotKey(Handle, hotkeyId, mods, key))
                {
                    MessageBox.Show(string.Format(lang[22], d1.Text), null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                return;
            }

            cfg[0] = d1.Text == middle ? "Middle" : d1.Text;
            UpdateText();
            e0.Text = d1.Text.Contains('+') ? lang[23] : string.Empty;
            e0.Left = Width - e0.Width - 12;

            // h0.Visible = d1.Text.Contains('+');
            d2.Focus();
        };
        const int ft = 7;
        a1.TextChanged += (_, _) => cfg[1] = a1.Text;
        a2.SelectedIndexChanged += (_, _) =>
        {
            cfg[2] = $"{a2.SelectedIndex}";
            switch (a2.SelectedIndex)
            {
                case 6:
                    c0.Text = $"{lang[3]} : ";
                    c1.Text = cfg[5];
                    runMode = 1;
                    break;
                case 7:
                    c0.Text = $"{lang[4]} : ";
                    c1.Text = cfg[7];
                    runMode = 2;
                    break;
                default:
                    c0.Text = $"{lang[2]} : ";
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
                using OpenFileDialog fd = new() { CheckFileExists = true, CheckPathExists = true, Multiselect = false, Filter = runMode == 1 ? "*.*|*.*" : $"*.msck ({lang[26]})|*.msck" };
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    c1.Text = fd.FileName;
                }
            }
        };
        ContextMenuStrip cm = new(), cmp = new();
        ToolStripMenuItem scaleMenu = new(lang[27]), langMenu = new(lang[28]), langDef = new(lang[29]), webMenu = new(lang[30]);
        EventHandler handler = new((s, _) =>
        {
            if (s is ToolStripMenuItem ti)
            {
                var p = ti.OwnerItem;
                if (p == webMenu)
                {
                    switch (ti.Text)
                    {
                        case "Github":
                            CreateProcess("https://github.com/lalakii/MouseClickTool", null);
                            break;
                        case "SourceForge":
                            CreateProcess("https://mouseclicktool.sourceforge.io", null);
                            break;
                    }
                }
                else
                {
                    if (p == scaleMenu)
                    {
                        cfg[10] = $"{ti.Tag}";
                    }
                    else if (p == langMenu)
                    {
                        cfg[11] = $"{ti.Tag}";
                    }

                    try
                    {
                        Application.ExitThread();
                    }
                    finally
                    {
                        Application.Restart();
                    }
                }
            }
        });

        var checkedDefItem = true;
        ToolStripMenuItem? scaleDef = null;
        for (int i = 50; i < 401; i += 25)
        {
            ToolStripMenuItem mu = new($"{i}%")
            {
                Tag = i,
                Checked = $"{i}" == cfg[10],
            };
            scaleMenu.DropDownItems.Add(mu);
            mu.Click += handler;
            if (mu.Checked && i != 100)
            {
                checkedDefItem = false;
            }

            if (i == 100)
            {
                scaleDef = mu;
            }
        }

        scaleDef?.Checked = checkedDefItem;
        langDef.Click += handler;
        langMenu.DropDownItems.Add(langDef);
        checkedDefItem = true;
        foreach (var it in Directory.GetFiles(".", "*.ini"))
        {
            var name = Path.GetFileNameWithoutExtension(it);
            ToolStripMenuItem mu = new(name)
            {
                Tag = name,
                Checked = $"{name}" == cfg[11],
            };
            langMenu.DropDownItems.Add(mu);
            mu.Click += handler;
            if (mu.Checked)
            {
                checkedDefItem = false;
            }
        }

        langDef.Checked = checkedDefItem;
        cmp.Items.AddRange([scaleMenu, langMenu, webMenu]);
        ToolStripMenuItem[] tsa = [new("SourceForge", null, handler), new("Github", null, handler)];
        webMenu.DropDownItems.AddRange(tsa);
        t3.Click += (_, _) =>
        {
            cmp.Show(t3, new(-t3.Width, 0));
        };
        t3.MouseEnter += (_, _) => t3.ForeColor = Color.LawnGreen;
        t3.MouseLeave += (_, _) => t3.ForeColor = a0.ForeColor;
        t2.MouseEnter += (_, _) => t2.ForeColor = Color.OrangeRed;
        t2.MouseLeave += (_, _) => t2.ForeColor = a0.ForeColor;
        t2.Click += (_, _) =>
            {
                Hide();
                z?.TrySetCanceled();
                Application.Exit();
            };
        t1.MouseEnter += (_, _) => t1.ForeColor = colorPrimary;
        t1.MouseLeave += (_, _) => t1.ForeColor = a0.ForeColor;
        t1.Click += (_, _) => WindowState = FormWindowState.Minimized;
        KeyDown += (_, e) =>
        {
            if (!hk)
            {
                // 非录制状态:焦点不在输入框且当前选中自定义组合时,Del 删除该快捷键
                if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None && d1.Text.Contains('+') && !a1.Focused && !c1.Focused)
                {
                    e.Handled = e.SuppressKeyPress = true;
                    d1.Items.Remove(d1.Text);
                    d1.SelectedItem = "F1";
                }

                return;
            }

            e.Handled = e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Escape)
            {
                hk = false;
                d1.SelectedItem = cfg[0] == "Middle" ? middle : cfg[0];
            }
            else if (e.KeyCode is Keys.Control or Keys.ControlKey or Keys.Shift or Keys.ShiftKey or Keys.Menu or Keys.LMenu or Keys.RMenu or Keys.LWin or Keys.RWin)
            {
                // 纯修饰键，继续等待主键
            }
            else if (e.Modifiers == Keys.None)
            {
                d2.Text = lang[24];
            }
            else
            {
                var combo = $"{((e.Modifiers & Keys.Control) != 0 ? "Ctrl+" : string.Empty)}{((e.Modifiers & Keys.Alt) != 0 ? "Alt+" : string.Empty)}{((e.Modifiers & Keys.Shift) != 0 ? "Shift+" : string.Empty)}{e.KeyCode}";
                hk = false;
                if (cfg[0] != "Middle" && cfg[0].Contains('+') && d1.Items.Contains(cfg[0]))
                {
                    d1.Items.Remove(cfg[0]);
                }

                d1.Items.Insert(d1.Items.IndexOf(custom), combo);
                cfg[0] = combo;
                d1.SelectedItem = combo;
            }
        };
        Paint += (_, e) =>
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : WindowState;
            using var g = e.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawString(Text, new("Segoe UI", 12f * fontScale), new SolidBrush(d2.ForeColor), 5, 4);
            using Pen p = new(colorPrimary, 7.7f);
            g.DrawLine(p, Width, 0, 0, 0);
            p.Color = dark ? Color.White : Color.DarkGray;
            p.Width = .1f;
            g.DrawRectangle(p, 5, a0.Top - 5, Width - 12, Height - a0.Top);
        };
        int.TryParse(cfg[2], NumberStyles.Integer, cl, out int ctv);
        if (cfg[0] != "Middle" && !d1.Items.Contains(cfg[0]))
        {
            if (TryParseHotkey(cfg[0], out _, out _))
            {
                d1.Items.Insert(d1.Items.IndexOf(custom), cfg[0]);
                d1.SelectedItem = cfg[0];
            }
            else
            {
                d1.SelectedItem = "F1";
            }
        }
        else
        {
            d1.SelectedItem = cfg[0] == "Middle" ? middle : cfg[0];
        }

        a1.Text = cfg[1];
        a2.SelectedIndex = ctv;
        Load += (_, _) =>
        {
            a0.Left = Math.Abs(a0.Width - b0.Width) + ft;
            b0.Left = a0.Right - b0.Width;
            a0.Top = t2.Height;
            a1.Left = a0.Right + ft;
            d1.Width = a1.Width = Math.Max((int)DefaultFont.Size * 9, d1.DropDownWidth + ft);
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
            d2.Width = new int[] { a2.DropDownWidth, TextRenderer.MeasureText(lang[20], d2.Font).Width, TextRenderer.MeasureText($"{lang[1]} (Ctrl+Alt+Shift+F12)", d2.Font).Width }.Max();
            d2.Height = d1.Height; // 去掉 AutoSize 后必须显式设高度,否则按钮过细、下一行(复选框)上移与快捷键行重叠
            d2.Top = d1.Top - HeightDiff(d2.Height, d1.Height);
            d3.Height = d2.Height - 2;
            d3.Top = d2.Top + 1;
            d3.Left = d2.Left + 1;
            a2.Width = d2.Width;
            Width = d2.Right + 12;
            b1.Width = d2.Right - d1.Left;
            c1.Width = b1.Width;
            t2.Left = Width - t2.Width - ft;
            t1.Left = t2.Left - t2.Width;
            t2.Top = 1;
            t1.Top = (int)(HeightDiff(t2.Height, t1.Height) + 0.5);
            t3.Left = t1.Left - t1.Width - (int)(12 * fontScale);
            t3.Top = t2.Top + HeightDiff(t2.Height, t3.Height);
            cb0.Left = ft * 2;
            cb0.Top = d2.Bottom + ft;
            e0.Top = cb0.Top - HeightDiff(cb0.Height, e0.Height);
            cb1.Left = cb0.Left;
            cb1.Top = cb0.Bottom + ft - 3;
            e0.MaximumSize = new(Width - cb0.Width - 3, 0);
            e0.Left = Width - e0.Width - 12;

            // h0.Left = cb0.Left;
            // h0.Top = cb0.Bottom + ft;
            Height = cb1.Bottom + 16;
        };

        Text = $"{cfg[9]}";
        _ = bool.TryParse(cfg[6], out bool r1);
        _ = bool.TryParse(cfg[8], out bool r9);
        cb1.Checked = r9;
        cb0.Checked = r1;
        cb0.CheckedChanged += (_, _) =>
        {
            r1 = cb0.Checked;
            cfg[6] = $"{r1}";
        };
        cb1.CheckedChanged += (_, _) =>
        {
            r9 = cb1.Checked;
            cfg[8] = $"{r9}";
        };
        FormClosing += (_, _) =>
        {
            UnhookMouse();
            try
            {
                File.WriteAllLines(ini, cfg);
            }
            catch
            {
            }
        };
        byte[] r0 = new byte[4];
        if (dark)
        {
            d2.TextChanged += (_, _) =>
            {
                d3.Text = d2.Text;
            };
            d2.EnabledChanged += (_, _) =>
            {
                d3.Visible = !d2.Enabled;
                d3.Width = d2.Width - 2;
                d3.BringToFront();
            };
        }

        d2.Click += (_, _) =>
        {
            d2.Enabled = false;
            if (a2.Enabled && z == null)
            {
                _ = ulong.TryParse(a1.Text, out ulong delay);
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
                    var tg = (int)(b1.Value - DateTime.Now).TotalMilliseconds;
                    ulong.TryParse(c1.Text.Trim(), NumberStyles.Integer, cl, out ulong total);
                    var unrestricted = total < 1;
                    Invoke(() =>
                    {
                        UpdateText();
                        e0.Visible = !unrestricted;
                        if (e0.Visible)
                        {
                            var arr = e0.Text.Split(':');
                            if (arr.Length > 1 && ulong.TryParse(arr[1], out ulong savedTotal) && savedTotal > 0)
                            {
                                total = savedTotal;
                            }
                        }
                    });
                    var runAsScript = runMode == 2;
                    var scriptIndex = 0;
                    var scriptCount = 0;
                    string[]? scriptArr = null;
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

                    if (tg > 0)
                    {
                        await Task.WhenAny(Task.Delay(tg), z?.Task);
                    }

                    for (ulong count = 0; z != null && !z.Task.IsCanceled && (unrestricted || count < total || longPress); count++)
                    {
                        if (runAsScript)
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
                                        var arg0 = args[0].Trim();
                                        var arg1 = args[1].Trim();
                                        if (!arg0.Equals("null", StringComparison.OrdinalIgnoreCase) && !arg1.Equals("null", StringComparison.OrdinalIgnoreCase))
                                        {
                                            _ = int.TryParse(arg0, out int posX);
                                            _ = int.TryParse(arg1, out int posY);
                                            var screen = Screen.PrimaryScreen.Bounds;
                                            m.mi.dx = posX * 65535 / screen.Width;
                                            m.mi.dy = posY * 65535 / screen.Height;
                                            m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_MOVE | MouseEventFlag.MOUSEEVENTF_ABSOLUTE;
                                            SendInput(size);
                                        }
                                    }

                                    if (r9)
                                    {
                                        try
                                        {
                                            File.AppendAllText("MouseClickTool.LOG", $"[{DateTime.Now}] {eventType} {scriptCommand}${Environment.NewLine}");
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    switch (eventType)
                                    {
                                        case "delay":
                                        case "sleep":
                                            if (ulong.TryParse(scriptCommand, out delay))
                                            {
                                                await Task.WhenAny(Task.Delay(TimeSpan.FromMilliseconds(delay)), z?.Task);
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
                                            break;
                                        case "right_click_long":
                                            upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTUP;
                                            if (args.Length > 2 && args[2].Contains("1"))
                                            {
                                                upFlag = MouseEventFlag.MOUSEEVENTF_RIGHTDOWN;
                                            }

                                            m.mi.dwFlags = upFlag;
                                            break;
                                        case "mouse_wheel":
                                            m.mi.dwFlags = MouseEventFlag.MOUSEEVENTF_WHEEL;
                                            if (args.Length > 0)
                                            {
                                                int.TryParse(args[0], NumberStyles.Integer, cl, out int sc);
                                                m.mi.mouseData = sc;
                                            }

                                            break;
                                        case "create_process":
                                            CreateProcess("cmd.exe", $"/c {scriptCommand}");
                                            continue;
                                        case "title":
                                            Invoke((MethodInvoker)(() =>
                                            {
                                                Text = scriptCommand.Trim('\"');
                                                cfg[9] = Text;
                                                Invalidate();
                                            }));
                                            continue;
                                        case "exit":
                                        case "quit":
                                            Invoke((MethodInvoker)(() => InvokeOnClick(t2, null)));
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
                        else if (runMode == 1)
                        {
                            CreateProcess("cmd.exe", $"/c \"{c1.Text}\"");
                            break;
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
                                    if (e0.Visible)
                                    {
                                        e0.Text = $"{lang[25]}: {total - count - 1}";
                                        e0.Left = Width - e0.Width - 12;
                                    }
                                });
                            }
                            else
                            {
                                m.mi.dwFlags = upFlag;
                                pressed = true;
                            }
                        }

                        if (delay != 0)
                        {
                            ulong r5 = delay;
                            if (r1)
                            {
                                // 随机系数：0.8 ~ 1.2
                                p.GetBytes(r0);
                                int r2 = BitConverter.ToInt32(r0, 0);
                                double r3 = (r2 & 0x7FFFFFFF) / (double)0x7FFFFFFF;
                                double r4 = (r3 * (1.2 - 0.8)) + 0.8;
                                r5 = (ulong)Math.Round(delay * r4);
                            }

                            await Task.WhenAny(Task.Delay(TimeSpan.FromMilliseconds(r5)), z?.Task);
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
                        if (d1.Text.Contains("+"))
                        {
                            e0.Text = lang[23];
                            e0.Left = Width - e0.Width - 12;
                        }

                        a1.Enabled = a2.Enabled = b1.Enabled = c1.Enabled = true;
                        b1.Value = DateTime.Now;
                        UpdateText();
                    });
                });
            }
            else
            {
                z?.TrySetCanceled();
            }
        };
        hp = MouseHookCb;
        Application.Run(this);
    }

    private delegate IntPtr LLMP(int nCode, IntPtr wParam, IntPtr lParam);

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
            Trigger();
        }
    }

    private static void CreateProcess(string path, string? args)
    {
        Task.Run(() => System.Diagnostics.Process.Start(path, args));
    }

    private static int HeightDiff(int h0, int h1)
    {
        int value = h0 - h1;
        int mask = value >> 31;
        return ((value + mask) ^ mask) / 2;
    }

    // 解析热键字符串："F1" → 单键；"Ctrl+Alt+K" → 修饰键组合。mods 含 MOD_NOREPEAT(0x4000)
    private static bool TryParseHotkey(string? text, out int mods, out Keys key)
    {
        mods = 0x4000;
        key = Keys.None;

        // net462 的 IsNullOrEmpty 缺少 NotNullWhen 注解,显式判空让空值分析通过
        if (text == null || text.Length == 0)
        {
            return false;
        }

        foreach (var t in text.Split('+'))
        {
            var s = t.Trim();
            if (s.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || s.Equals("Control", StringComparison.OrdinalIgnoreCase))
            {
                mods |= 0x2;
            }
            else if (s.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                mods |= 0x1;
            }
            else if (s.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                mods |= 0x4;
            }
            else if (s.Equals("Win", StringComparison.OrdinalIgnoreCase))
            {
                mods |= 0x8;
            }
            else if (!Enum.TryParse(s, true, out key) || key == Keys.None)
            {
                return false;
            }
        }

        return key != Keys.None;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vk);

    // 参考：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
    [DllImport("user32.dll")]
    private static extern int SendInput(int nInputs, ref Input pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LLMP lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    private void SendInput(int cbSize)
    {
        _ = SendInput(1, ref m, cbSize);
    }

    private void UpdateText()
    {
        var d2 = Controls.OfType<Button>().First();
        d2.Text = $"{(z == null ? lang[0] : lang[1])} ({cfg[0]})";
        d2.Enabled = true;
    }

    private void Trigger()
    {
        wait = 0;
        Controls.OfType<Button>().FirstOrDefault().PerformClick();
    }

    private void HookMouse()
    {
        if (hh == IntPtr.Zero)
        {
            hh = SetWindowsHookEx(14, hp ??= MouseHookCb, GetModuleHandle(null), 0);
        }
    }

    private void UnhookMouse()
    {
        if (hh != IntPtr.Zero)
        {
            UnhookWindowsHookEx(hh);
            hh = IntPtr.Zero;
        }
    }

    private IntPtr MouseHookCb(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // 松开中键才触发（按下不触发，便于按住预备/瞄准；按住期间目标可能处于中键特殊状态，
        // 如浏览器自动滚动，此时合成的连击会被吞掉）。500ms 去抖：滚轮回弹易产生二次 WM_MBUTTONUP，会误触发"停止"
        if (nCode >= 0 && wParam == (IntPtr)0x0208 && Environment.TickCount - ht > 500)
        {
            ht = Environment.TickCount;
            Trigger();
        }

        return CallNextHookEx(hh, nCode, wParam, lParam);
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