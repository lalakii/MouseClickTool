using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.9.2.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2026 lalaki.cn")]

// main.
var p = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
using (var f = File.Create(p))
{
    new GZipStream(Assembly.GetEntryAssembly().GetManifestResourceStream("App.x86.GZ"), CompressionMode.Decompress).CopyTo(f);
}

Activator.CreateInstanceFrom(p, "MouseClickTool");