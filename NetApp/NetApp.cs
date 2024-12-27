using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.5.0.0")]
[assembly: AssemblyTitle("MouseClickTool minimal")]
[assembly: AssemblyProduct("MouseClickTool minimal")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

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
                System.Net.WebClient w = new();
                new GZipStream(w.OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyTo(m);
                m.Position = 0;
                m.CopyTo(f);
            }
            else
            {
                f.CopyTo(m);
            }
            Activator.CreateInstance(Assembly.Load(m.GetBuffer()).GetExportedTypes()[0]);
        }
        catch
        {
            f.SetLength(0);
        }
    }
}