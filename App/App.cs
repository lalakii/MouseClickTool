using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.7.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2025 lalaki.cn")]

public static class App
{
    [System.STAThread]
    public static void Main()
    {
        var r = Assembly.GetEntryAssembly();
        using System.IO.MemoryStream m = new();
        new GZipStream(r.GetManifestResourceStream(r.GetManifestResourceNames()[0]), CompressionMode.Decompress).CopyTo(m);
        Assembly.Load(m.GetBuffer()).CreateInstance("MouseClickTool");
    }
}