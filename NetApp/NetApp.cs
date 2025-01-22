using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.6.0.0")]
[assembly: AssemblyTitle("MouseClickTool minimal")]
[assembly: AssemblyProduct("MouseClickTool minimal")]
[assembly: AssemblyCopyright("Copyright (C) 2025 lalaki.cn")]

public static class NetApp
{
    [STAThread]
    public static void Main()
    {
        var a = Environment.Is64BitProcess ? "x64" : "x86";
        using FileStream f = new(Path.Combine(Path.GetTempPath(), $"lalaki_mouse_click_tool_{a}.dll"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        MemoryStream m = new();
        try
        {
            if (f.Length == 0)
            {
                new GZipStream(new System.Net.WebClient().OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyTo(m);
                m.CopyTo(f);
            }
            else
            {
                f.CopyTo(m);
            }
            Assembly.Load(m.GetBuffer()).CreateInstance("MouseClickTool");
        }
        catch
        {
            f.SetLength(0);
        }
    }
}