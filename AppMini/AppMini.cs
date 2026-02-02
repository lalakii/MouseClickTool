using System.IO.Compression;

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

    Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
    Activator.CreateInstanceFrom(p, "MouseClickTool");
}
catch
{
    File.Delete(p);
}