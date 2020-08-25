using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fs.Core.Constants;
using Fs.Business.Pages;

namespace Fs.Blazor.Client.Pages
{
    public class _HostAuthModel : HostAuthModel
    {
    }
}
