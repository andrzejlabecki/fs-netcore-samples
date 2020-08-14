// dllmain.h : Declaration of module class.

class CFsCppComModule : public ATL::CAtlDllModuleT< CFsCppComModule >
{
public :
	DECLARE_LIBID(LIBID_FsCppComLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_FSCPPCOM, "{9086deb6-7e20-410d-be46-9efcd27eab53}")
};

extern class CFsCppComModule _AtlModule;
