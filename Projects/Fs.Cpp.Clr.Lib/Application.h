#pragma once
using namespace System;

namespace Fs 
{
	namespace Cpp
	{
		public ref class Application
		{
			public:
				Application();

		public:
			String^ GetPath();
			String^ GetName();
		};
	}
}
