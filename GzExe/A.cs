using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyVersion("2.4.0.0")]
[assembly: AssemblyFileVersion("2.4.0.0")]
[assembly: AssemblyTitle("MouseClickTool")]
[assembly: AssemblyProduct("MouseClickTool")]
[assembly: AssemblyCopyright("Copyright (C) 2024 lalaki.cn")]

public static class A
{
    [System.STAThread]
    public static void Main()
    {
        try { SetProcessDPIAware(); } catch { }
        Application.EnableVisualStyles();
        var A = typeof(A).Assembly;
        using System.IO.MemoryStream AA = new();
        new GZipStream(A.GetManifestResourceStream(A.GetManifestResourceNames()[0]), CompressionMode.Decompress).CopyTo(AA);
        Application.Run((Form)Assembly.Load(AA.ToArray()).GetExportedTypes()[0].GetConstructors()[0].Invoke(null));
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
}