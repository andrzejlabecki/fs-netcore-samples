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
			virtual String^ GetPath();
			virtual String^ GetName();
		};
	}
}
