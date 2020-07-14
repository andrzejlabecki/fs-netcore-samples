
Fs.Angular.Is4:
Works with IIS at https://fs-angular-is4.netpoc.com/
App with angular client and .Net Core server, it's with integrated IS4.

Fs.Angular.Is4.Client:
Works with IIS at https://fs-angular-is4-client.netpoc.com/
App with angular client and .Net Core server, it's with integrated IS4. 
It is using Fs.Angular.Is4 as OIDC/OAuth provider.

Fs.Angular.Client:
Works with IIS at https://fs-angular-client.netpoc.com/
App with angular client and .Net Core server. It is using Fs.Angular.Is4 as OIDC/OAuth provider.

Fs.Blazor.Is4.Wasm.Client:
Works with IIS at https://fs-blazor-is4-wasm-client.netpoc.com/
App with blazor client and .Net Core server, it's with integrated IS4. 
It is using Fs.Angular.Is4 as OIDC/OAuth provider.

Fs.Blazor.Wasm.Client:
Works with IIS at https://fs-blazor-wasm-client.netpoc.com/
App with blazor client and .Net Core server. It is using Fs.Angular.Is4 as OIDC/OAuth provider.

Fs.Blazor.Is4:
Works with IIS at https://fs-blazor-is4.netpoc.com/
App with blazor client (server-side) and .Net Core server, it's with integrated IS4.

Fs.Blazor.Client:
Works with IIS at https://fs-blazor-client.netpoc.com/
App with blazor client (server-side) and .Net Core server. It is using Fs.Angular.Is4 as OIDC/OAuth provider.

Fs.Api:
Works with IIS at https://fs-api.netpoc.com/
.Net Core app exposing web API, it's using Fs.Angular.Is4/Fs.Angular.Is4.Client as OAuth providers.

Fs.Mvc.Is4:
Works with IIS at https://fs-mvc-is4.netpoc.com/
App with MVC client and .Net Core server, it's with integrated IS4.

Mappings for hosts file:
127.0.0.1 fs-angular-is4.netpoc.com
127.0.0.1 fs-angular-is4-client.netpoc.com
127.0.0.1 fs-angular-client.netpoc.com
127.0.0.1 fs-blazor-wasm-client.netpoc.com
127.0.0.1 fs-blazor-is4-wasm-client.netpoc.com
127.0.0.1 fs-blazor-is4.netpoc.com
127.0.0.1 fs-blazor-client.netpoc.com
127.0.0.1 fs-mvc-is4.netpoc.com
127.0.0.1 fs-api.netpoc.com

Configuration is maintained across 3 json files - SharedSettings.json (global), 
appsettings.json (application specific), appsettings.<environment>.json (application/environment specific).
Higher level config file overrides settings from the lower level ones, if the same setting defined in multiple
json configuration files.

There are 3 identity server implementations - angular, blazor (server), and MVC. Identity server can be \
switched globally in SharedSettings.json - "OidcAuthority:httpLink".

There are 2 apps with identity server and ability to login through external OIDC provider -
Fs.Angular.Is4.Client, Fs.Blazor.Is4.Wasm.Client (with web assembly blazor client).

There are 3 pure clients using external identity server for authentication - 
Fs.Angular.Client, Fs.Blazor.Wasm.Client (with web assembly blazor client), 
Fs.Blazor.Client (with server blazor client)

There is separate API app - Fs.Api using external identity servers for authorization. 

VS 2019 templates were used for applications, but they need to be customized for various workflows and 
combinations of identity server and client app.

