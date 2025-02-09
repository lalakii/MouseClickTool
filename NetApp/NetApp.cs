using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.8.0.0")]
[assembly: AssemblyTitle("MouseClickTool minimal")]
[assembly: AssemblyProduct("MouseClickTool minimal")]
[assembly: AssemblyCopyright("Copyright (C) 2025 lalaki.cn")]

// main.
var a = Environment.Is64BitProcess ? "x64" : "x86";
Type? t = null;
using (var f = File.Open(Path.Combine(Path.GetTempPath(), $"MouseClickTool_{a}.dll"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
{
    try
    {
        if (f.Length == 0L)
        {
            new GZipStream(new System.Net.WebClient().OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyToAsync(f).Wait();
            f.Position = 0L;
        }

        t = Assembly.Load(new BinaryReader(f).ReadBytes((int)f.Length)).GetExportedTypes()[0];
    }
    catch
    {
        f.SetLength(0L);
    }
}

Activator.CreateInstance(t);