// Application.cpp : Implementation of CApplication

#include "pch.h"
#include "Application.h"
#include "..\Fs.Cpp.Lib\Application.h"

// CApplication
CApplication::CApplication()
{
}

CApplication::~CApplication()
{
}

STDMETHODIMP CApplication::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] =
	{
		&IID_IApplication
	};
	for (int i = 0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (InlineIsEqualGUID(*arr[i], riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP CApplication::GetPath(BSTR* pbsPath)
{
	HRESULT hr = S_OK;

	try
	{
		*pbsPath = UnmanagedLib::Application().GetPath().Detach();
	}
	catch (...)
	{
		hr = E_FAIL;
	}

	return hr;
}

STDMETHODIMP CApplication::GetName(BSTR* pbsName)
{
	HRESULT hr = S_OK;

	try
	{
		*pbsName = UnmanagedLib::Application().GetName().Detach();
	}
	catch (...)
	{
		hr = E_FAIL;
	}

	return hr;
}
