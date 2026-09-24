// main.
using System.Reflection;

var entryAssembly = Assembly.GetEntryAssembly();
using Stream assemblyStream = entryAssembly?.GetManifestResourceStream("MouseClickTool.dll") ?? throw new InvalidOperationException("Embedded MouseClickTool assembly was not found.");
using MemoryStream assemblyBytes = new();
assemblyStream.CopyTo(assemblyBytes);
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
Assembly.Load(assemblyBytes.ToArray()).CreateInstance("MouseClickTool");
