
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

Fs.Api:
Works with IIS at https://fs-api.netpoc.com/
.Net Core app exposing web API, it's using Fs.Angular.Is4/Fs.Angular.Is4.Client as OAuth providers.

Mappings for hosts file:
127.0.0.1 fs-angular-is4.netpoc.com
127.0.0.1 fs-angular-is4-client.netpoc.com
127.0.0.1 fs-angular-client.netpoc.com
127.0.0.1 fs-blazor-wasm-client.netpoc.com
127.0.0.1 fs-blazor-is4-wasm-client.netpoc.com
127.0.0.1 fs-api.netpoc.com
