using System.IO.Compression;
using System.Reflection;

// main.
var a = Environment.GetCommandLineArgs();
if (a != null && a.Length > 1)
{
    var f = a[1];
    if (File.Exists(f))
    {
        var o = $"{f}.gz";
        if (a.Length > 2)
        {
            o = a[2];
        }

        using GZipStream g = new(File.Create(o), CompressionLevel.Optimal);
        var h = File.OpenRead(f);
        h.CopyTo(g);
    }
}
else
{
    Console.WriteLine("EasyGzip.exe inputFullName compressFileName");
}