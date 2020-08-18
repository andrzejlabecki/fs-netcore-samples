using System;
using System.Runtime.InteropServices;

namespace Fs.CSharp
{
    [Guid("3D2C2913-7E0C-4A8D-B8AD-C7DD9EE14480")]
    [ComVisible(true)]
    public interface IApplication
    {
        string GetPath();
        string GetName();
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("ACE32CC7-A66C-4E52-9FFD-C60F9C872975")]
    [ComVisible(true)]
    [ProgId("Fs.CSharp.Lib.Application")]
    public class Application : Fs.Cpp.Application, IApplication
    {
    }
}
