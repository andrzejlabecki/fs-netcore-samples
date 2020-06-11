﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Fs.Core.Contracts;

namespace Fs.Core.Interfaces.Services
{
    public interface ISharedConfiguration : IConfiguration
    {
        string GetConnectionString(string name);
        string GetValue(string name);
    }
}

