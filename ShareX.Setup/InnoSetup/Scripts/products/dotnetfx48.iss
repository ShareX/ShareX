; requires Windows 10, Windows 7 Service Pack 1, Windows 8.1, Windows Server 2008 R2 SP1, Windows Server 2012, Windows Server 2012 R2, Windows Server 2016
; express setup (downloads and installs the components depending on your OS) if you want to deploy it locally download the full installer on website below
; https://dotnet.microsoft.com/download/dotnet-framework/net48

[CustomMessages]
dotnetfx48_title=.NET Framework 4.8

dotnetfx48_size=1 MB - 59 MB

[Code]
const
	dotnetfx48_url = 'http://download.visualstudio.microsoft.com/download/pr/7afca223-55d2-470a-8edc-6a1739ae3252/c9b8749dd99fc0d4453b2a3e4c37ba16/ndp48-web.exe';

procedure dotnetfx48(minVersion: integer);
begin
	if (netfxspversion(NetFx4x, '') < minVersion) then
		AddProduct('dotnetfx48.exe',
			'/lcid ' + CustomMessage('lcid') + ' /passive /norestart',
			CustomMessage('dotnetfx48_title'),
			CustomMessage('dotnetfx48_size'),
			dotnetfx48_url,
			false, false, false);
end;

[Setup]
