#include "pch.h"

using namespace UnmanagedLib;

int main()
{
    Application app = Application();

    wchar_t* wchName = GetAppName();
    wchar_t* wchPath = GetAppPath();

    std::cout << "Application name: " << CW2A(app.GetName()) << "\r\n";
    std::cout << "Application path: " << CW2A(app.GetPath()) << "\r\n";
    std::wcout << _T("Application name: ") << wchName << _T("\r\n");
    std::wcout << _T("Application path: ") << wchPath << _T("\r\n");

    delete wchName;
    delete wchPath;

    _CrtDumpMemoryLeaks();
}
