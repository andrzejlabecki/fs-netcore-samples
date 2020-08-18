#include "pch.h"
#include "Application.h"

Fs::Cpp::Application::Application()
{

}

String^ Fs::Cpp::Application::GetPath()
{
	UnmanagedLib::Application app = UnmanagedLib::Application();

	CComBSTR bsPath = app.GetPath();

	String^ strPath = gcnew String(bsPath.m_str);

	return strPath;
}

String^ Fs::Cpp::Application::GetName()
{
	UnmanagedLib::Application app = UnmanagedLib::Application();

	CComBSTR bsName = app.GetName();

	String^ strName = gcnew String(bsName.m_str);

	return strName;
}
