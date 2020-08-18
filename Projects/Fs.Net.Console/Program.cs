using System;
using System.Runtime.InteropServices;
using Fs.Cpp;

namespace Fs.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Application app = new Application();
                FsCppComLib.IApplication appInt = new FsCppComLib.ApplicationClass();

                string appName = app.GetName();
                string appPath = app.GetPath();

                Console.WriteLine("Application name (C++): " + appName);
                Console.WriteLine("Application path (C++): " + appPath);

                appName = appInt.GetName();
                appPath = appInt.GetPath();

                Console.WriteLine("Application name (COM): " + appName);
                Console.WriteLine("Application path (COM): " + appPath);
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine(message);
            }
        }
    }
}
