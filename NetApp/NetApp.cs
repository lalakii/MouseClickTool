using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.7.0.0")]
[assembly: AssemblyTitle("MouseClickTool minimal")]
[assembly: AssemblyProduct("MouseClickTool minimal")]
[assembly: AssemblyCopyright("Copyright (C) 2025 lalaki.cn")]

public static class NetApp
{
    [STAThread]
    public static void Main()
    {
        var a = Environment.Is64BitProcess ? "x64" : "x86";
        const string n = "MouseClickTool";
        using var f = new FileStream(Path.Combine(Path.GetTempPath(), $"{n}_{a}.dll"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        try
        {
            if (f.Length == 0)
            {
                new GZipStream(new System.Net.WebClient().OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyTo(f);
                f.Position = 0;
            }
            Assembly.Load(new BinaryReader(f).ReadBytes((int)f.Length)).CreateInstance(n);
        }
        catch
        {
            f.SetLength(0);
        }
    }
}