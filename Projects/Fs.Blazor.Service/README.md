# ASP.NET Core Windows Service Sample

After establishing and starting the service, access the app in a browser at `http://localhost:5000/`.

netsh http add urlacl url=https://fs-blazor-service.netpoc.com:5001/ user=Users
netsh http delete urlacl url=https://fs-blazor-service.netpoc.com:5001/

netsh http add sslcert 
    ipport=127.0.0.1:5001 
    certhash=b28f3911ccc87aa969ddffb82f49dbe6be028ddf 
    appid="{946076E2-30AF-4411-BF2C-A4AD8FC5F073}"

netsh http delete sslcert ipport=127.0.0.1:5001