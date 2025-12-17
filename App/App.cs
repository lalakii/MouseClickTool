using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.9.1.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2025 lalaki.cn")]

// main.
Type t;
using (var f = File.Create(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())))
{
    var r = Assembly.GetEntryAssembly();
    new GZipStream(r.GetManifestResourceStream(r.GetManifestResourceNames()[0]), CompressionMode.Decompress).CopyToAsync(f).Wait();
    f.Position = 0L;
    t = Assembly.Load(new BinaryReader(f).ReadBytes((int)f.Length)).GetExportedTypes()[0];
    f.SetLength(0L);
}

Activator.CreateInstance(t);