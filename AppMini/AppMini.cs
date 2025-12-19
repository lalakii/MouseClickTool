using System.IO.Compression;
using System.Reflection;

// main.
var a = Environment.Is64BitProcess ? "x64" : "x86";
var p = Path.Combine(Path.GetTempPath(), $"MouseClickTool_{DateTime.Now:yyyy-MM}_{a}.dll");
try
{
    using (var f = File.Open(p, FileMode.OpenOrCreate, FileAccess.Write))
    {
        if (f.Length == 0L)
        {
            new GZipStream(new System.Net.WebClient().OpenRead($"https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/{a}.GZ"), CompressionMode.Decompress).CopyTo(f);
        }
    }

    Activator.CreateInstanceFrom(p, "MouseClickTool");
}
catch
{
    File.Delete(p);
}