using System.IO.Compression;

// built-in Gzip
var a = Environment.GetCommandLineArgs();
if (a != null && a.Length > 1)
{
    var f = a[1];
    if (File.Exists(f))
    {
        var o = $"{f}.gz";
        if (a.Length > 2)
        {
            o = a[2];
        }

        using GZipStream g = new(File.Create(o), CompressionLevel.Optimal);
        var h = File.OpenRead(f);
        h.CopyTo(g);
    }
}
else
{
    var n = "EasyGzip.exe";
    Console.WriteLine($@"
Usage:
  {n} <inputFile> <outputFile>

Description:
  Compresses a file using Gzip compression.

Arguments:
  <inputFile>   The path or full name of the file to compress.
  <outputFile>  The path or name of the resulting Gzip file.

Example:
  {n} document.txt document.txt.gz
");
}