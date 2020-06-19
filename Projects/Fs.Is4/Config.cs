// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource()
                {
                    Name = "user_name",
                    DisplayName = "Username",
                    UserClaims = { JwtClaimTypes.PreferredUserName }
                }
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("Client2.AppAPI", "My API1"),
                new ApiResource("WebAPI", "My API2")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "Fs.Blazor.Client",
                    ClientSecrets = { new IdentityServer4.Models.Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },

                    // where to redirect to after login
                    RedirectUris = { "https://fs-blazor-client.netpoc.com/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://fs-blazor-client.netpoc.com/signout-callback-oidc" },

                    AllowedCorsOrigins = { "https://fs-blazor-client.netpoc.com" },
                },
                new Client
                {
                    ClientId = "AngularPOC.Client2",
                    //ClientSecrets = { new IdentityServer4.Models.Secret("secret".Sha256()) },
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Client2.AppAPI",
                        "WebAPI"
                    },

                    // where to redirect to after login
                    RedirectUris = { "https://angular4.netpoc.com/authentication/login-callback",
                                     "https://angular4.netpoc.com/signin-oidc"},

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://angular4.netpoc.com/authentication/logout-callback" },

                    AllowedCorsOrigins = { "https://angular4.netpoc.com" },
                },
                // ilink web forms asp.net 4.5.2 web app
                new Client
                {
                    ClientId = "iLink",
                    ClientName = "Legalilink",
                    ClientSecrets = { new Secret("3B13615E-69D1-458C-9149-908DCEB06F62".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:63902/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:63902/signout-oidc",
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:63902" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "user_name"
                    }
                }
            };
    }
}