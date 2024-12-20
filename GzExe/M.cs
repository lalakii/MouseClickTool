using System.IO.Compression;
using System.Reflection;

[assembly: AssemblyVersion("2.3.0.0")]
[assembly: AssemblyFileVersion("2.3.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

public static class M
{
    [System.STAThread]
    public static void Main()
    {
        var A = typeof(M).Assembly;
        using GZipStream AA = new(A.GetManifestResourceStream(A.GetManifestResourceNames()[0]), CompressionMode.Decompress);
        System.IO.MemoryStream AAA = new();
        AA.CopyTo(AAA);
        Assembly.Load(AAA.ToArray()).GetExportedTypes()[0].GetMethods(BindingFlags.Public | BindingFlags.Static)[0].Invoke(null, null);
    }
}