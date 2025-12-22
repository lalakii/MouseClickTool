using System.IO.Compression;
using System.Reflection;

// main.
using MemoryStream m = new();
new GZipStream(Assembly.GetEntryAssembly().GetManifestResourceStream("App.x86.GZ"), CompressionMode.Decompress).CopyTo(m);
Assembly.Load(m.ToArray()).CreateInstance("MouseClickTool");
