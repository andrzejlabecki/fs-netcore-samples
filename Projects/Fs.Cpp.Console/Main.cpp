#include "pch.h"
#include "..\Fs.Cpp.Lib\Application.h"
#include "..\Fs.Cpp.Com\FsCppCom_i.h"
#include "..\Fs.Cpp.Com\FsCppCom_i.c"
#include "FsSharp_h.h"
#include "FsSharp_i.c"

int main()
{
    HRESULT hr = S_OK;
    IApplication* pApp = NULL;
    IApplicationSharp* pAppSharp = NULL;
    CComBSTR bsName;
    CComBSTR bsPath;
    CComBSTR bsNameSharp;
    CComBSTR bsPathSharp;

    if (SUCCEEDED(hr = CoInitializeEx(NULL, COINIT_MULTITHREADED)))
    {
        if (SUCCEEDED(hr = CoCreateInstance(CLSID_Application,
                                            NULL,
                                            CLSCTX_INPROC_SERVER,
                                            IID_IApplication,
                                            (void**)&pApp)))
        {
            hr = pApp->GetName(&bsName);
            hr = pApp->GetPath(&bsPath);
        }

        if (SUCCEEDED(hr = CoCreateInstance(CLSID_ApplicationSharp,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IApplicationSharp,
            (void**)&pAppSharp)))
        {
            hr = pAppSharp->GetName(&bsNameSharp);
            hr = pAppSharp->GetPath(&bsPathSharp);
        }
    }

    wchar_t* wchName = GetAppName();
    wchar_t* wchPath = GetAppPath();

    std::cout << "Application name (C class): " << CW2A(UnmanagedLib::Application().GetName()) << "\r\n";
    std::cout << "Application path (C class): " << CW2A(UnmanagedLib::Application().GetPath()) << "\r\n";
    std::wcout << _T("Application name (global): ") << wchName << _T("\r\n");
    std::wcout << _T("Application path (global): ") << wchPath << _T("\r\n");
    std::cout << "Application name (COM): " << CW2A(bsName) << "\r\n";
    std::cout << "Application path (COM): " << CW2A(bsPath) << "\r\n";
    std::cout << "Application name (COM/Sharp): " << CW2A(bsNameSharp) << "\r\n";
    std::cout << "Application path (COM/Sharp): " << CW2A(bsPathSharp) << "\r\n";

    delete wchName;
    delete wchPath;

    if (pApp != NULL)
        pApp->Release();

    if (pAppSharp != NULL)
        pAppSharp->Release();

    CoUninitialize();

    _CrtDumpMemoryLeaks();
}
