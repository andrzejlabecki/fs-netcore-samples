#include "pch.h"

int main()
{
    Application app = Application();

    std::cout << "Application name: " << CW2A(app.GetName()) << "\r\n";
    std::cout << "Application path: " << CW2A(app.GetPath()) << "\r\n";
}
