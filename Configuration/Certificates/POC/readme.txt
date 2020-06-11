
PowerShell script for certificate
https://gallery.technet.microsoft.com/scriptcenter/Self-signed-certificate-5920a7c6
New-SelfSignedCertificateEx.ps1


- Start PowerShell as administrator

- change directory
cd C:\FerrataSoft\Code\FSCollaboration\NETCore\Configuration\Certificates\POC

- execute

Import-module .\New-SelfsignedCertificateEx.ps1 -Force

- execute

New-SelfsignedCertificateEx -Subject "CN=NETPOC" -KeyUsage "KeyEncipherment, DigitalSignature" -SAN "localhost","*.netpoc.com" -StoreLocation "LocalMachine" -FriendlyName "NETPOC" -NotAfter $([datetime]::now.AddYears(20)) -Exportable 


export certificate from MMC LocalMachine to C:\FerrataSoft\Code\FSCollaboration\NETCore\Configuration\Certificates\POC\netpoc.server.pfx
Password 1234

Import the C:\FerrataSoft\Code\FSCollaboration\NETCore\Configuration\Certificates\POC\netpoc.server.pfx to Trusted Root
