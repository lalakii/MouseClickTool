using System;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.5.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

public static class App
{
    [STAThread]
    public static void Main()
    {
        var r = Assembly.GetEntryAssembly();
        using System.IO.MemoryStream m = new();
        new GZipStream(r.GetManifestResourceStream(r.GetManifestResourceNames()[0]), CompressionMode.Decompress).CopyTo(m);
        Activator.CreateInstance(Assembly.Load(m.GetBuffer()).GetExportedTypes()[0]);
    }
}