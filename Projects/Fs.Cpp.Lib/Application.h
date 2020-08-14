#pragma once

#ifdef FSCPPLIB_EXPORTS
#define CBIAPPLICATION_API __declspec(dllexport)
#else
#define CBIAPPLICATION_API __declspec(dllimport)
#endif

CBIAPPLICATION_API wchar_t* GetAppPath();
CBIAPPLICATION_API wchar_t* GetAppName();

namespace UnmanagedLib
{
	class CBIAPPLICATION_API Application
	{
	public:
		Application();

	public:
		CComBSTR GetPath();
		CComBSTR GetName();

	public:
		wchar_t* BSTRToWchar(CComBSTR& bsString);

	protected:
		CComBSTR& BSTRRight(CComBSTR& bsString, UINT nCount);
	};
}

