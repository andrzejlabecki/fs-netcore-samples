#pragma once

#ifdef FSCPPLIB_EXPORTS
#define CBIAPPLICATION_API __declspec(dllexport)
#else
#define CBIAPPLICATION_API __declspec(dllimport)
#endif


class CBIAPPLICATION_API Application
{
public:
	Application();

public:
	CComBSTR GetPath();
	CComBSTR GetName();
	
protected:
	CComBSTR& BSTRRight(CComBSTR& bsString, UINT nCount);
};

