// main.
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"MouseClickTool_AnyCPU.dll");
try
{
    if (!File.Exists(p) || (DateTime.UtcNow - File.GetLastWriteTime(p)).TotalDays > 30)
    {
        using System.Net.WebClient w = new();
        w.DownloadFile("https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/App/MouseClickTool.dll", p);
    }

    System.Reflection.Assembly.Load(File.ReadAllBytes(p)).CreateInstance("MouseClickTool");
}
catch
{
    File.Delete(p);
}