using System;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Fs.Core.Interfaces.Services;
using Fs.Core.Constants;
using Fs.Business.Pages;

namespace Fs.Blazor.Service.Pages
{
    public class _HostAuthModel : HostAuthModel
    {
    }
}
