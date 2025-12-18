using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.9.2.0")]
[assembly: AssemblyTitle("MouseClickTool minimal")]
[assembly: AssemblyProduct("MouseClickTool minimal")]
[assembly: AssemblyCopyright("Copyright (C) 2026 lalaki.cn")]

// main.
var a = Environment.Is64BitProcess ? "x64" : "x86";
var p = Path.Combine(Path.GetTempPath(), $"MouseClickTool_{Assembly.GetExecutingAssembly().GetName().Version}_{a}.dll");
using (var f = File.Open(p, FileMode.OpenOrCreate, FileAccess.ReadWrite))
{
    try
    {
        if (f.Length == 0L)
        {
            new GZipStream(new System.Net.WebClient().OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyTo(f);
        }
    }
    catch
    {
        f.SetLength(0L);
    }
}

Activator.CreateInstanceFrom(p, "MouseClickTool");