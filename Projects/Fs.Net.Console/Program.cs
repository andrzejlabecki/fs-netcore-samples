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

                string appName = app.GetName();
                string appPath = app.GetPath();

                Console.WriteLine("Application name: " + appName);
                Console.WriteLine("Application path: " + appPath);
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine(message);
            }
        }
    }
}
