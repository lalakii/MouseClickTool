using System.IO.Compression;
using System.Reflection;

// main.
using MemoryStream m = new();
new GZipStream(Assembly.GetEntryAssembly().GetManifestResourceStream("App.x86.GZ"), CompressionMode.Decompress).CopyTo(m);
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
Assembly.Load(m.ToArray()).CreateInstance("MouseClickTool");