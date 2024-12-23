using System;
using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.4.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

public static class A
{
    [STAThread]
    public static void Main()
    {
        var A = typeof(A).Assembly;
        using System.IO.MemoryStream AA = new();
        new GZipStream(A.GetManifestResourceStream(A.GetManifestResourceNames()[0]), CompressionMode.Decompress).CopyTo(AA);
        Activator.CreateInstance(Assembly.Load(AA.GetBuffer()).GetExportedTypes()[0]);
    }
}