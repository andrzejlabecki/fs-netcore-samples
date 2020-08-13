#include "pch.h"
#include "Application.h"

Application::Application()
{
}

CComBSTR Application::GetPath()
{
	// "D:\WINDOWS\system32\dllhost.exe /Processid:{3D14228D-FBE1-11D0-995D-00C04FD919C1}"
	// ""E:\Program Files\NUnit 2.2\bin\nunit-gui.exe" "
	// "\\?\D:\WINDOWS\Microsoft.NET\Framework\v1.1.4322\aspnet_wp.exe 4092 256 8 2 3 0 20 20 ZNB7CSHxqCwGW47nDb1s2GIoeiwiXE"

	TCHAR szCmd[512];
	::GetModuleFileName(0, szCmd, 511);
	CComBSTR bsPath = szCmd;

	return bsPath;
}

CComBSTR Application::GetName()
{
	TCHAR szCmd[512];
	::GetModuleFileName(0, szCmd, 511);
	CComBSTR bsPath = szCmd;
	CComBSTR bsName(L"");
	wchar_t* pStart = bsPath.m_str;
	wchar_t* pSlash = NULL;

	while (true)
	{
		pSlash = wcsstr(pStart, L"\\");

		if (pSlash == NULL)
		{
			pSlash = wcsstr(pStart, L"/");
			if (pSlash == NULL)
				break;
		}

		pStart = pSlash + 1;
	}

	UINT len = bsPath.Length();
	UINT start = pStart - bsPath.m_str;
	UINT count = len - start;

	bsName = BSTRRight(bsPath, count);

	return bsName;
}

CComBSTR& Application::BSTRRight(CComBSTR& bsString, UINT nCount)
{
	UINT nLen = ::SysStringLen(bsString);

	if (nCount < 0) nCount = 0;
	if (nCount > nLen) nCount = nLen;

	bsString = CComBSTR(nCount, bsString + nLen - nCount);
	return bsString;
}
