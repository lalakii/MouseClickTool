using System.IO.Compression;
using System.Reflection;

// main.
var p = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.dll");
using (var f = File.Create(p))
{
    new GZipStream(Assembly.GetEntryAssembly().GetManifestResourceStream("App.x86.GZ"), CompressionMode.Decompress).CopyTo(f);
}

Activator.CreateInstanceFrom(p, "MouseClickTool");