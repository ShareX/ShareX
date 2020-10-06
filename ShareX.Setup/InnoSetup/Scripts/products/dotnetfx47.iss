; requires Windows Server 2016 (version 1709), Windows 10 Anniversary Update (version 1607) (x86 and x64), Windows 10 Creators Update (version 1703) (x86 and x64), Windows 10 Fall Creators Update (version 1709) (x86 and x64), Windows Server 2012 R2 (x64), Windows 8.1 (x86 and x64), Windows Server 2012 (x64), Windows Server 2008 R2 Service Pack 1 (x64), Windows 7 Service Pack 1 (x86 and x64)
; express setup (downloads and installs the components depending on your OS) if you want to deploy it locally download the full installer on website below
; https://support.microsoft.com/en-us/help/4054531

[CustomMessages]
dotnetfx47_title=.NET Framework 4.7.2

dotnetfx47_size=1 MB - 59 MB

[Code]
const
	dotnetfx47_url = 'http://download.microsoft.com/download/0/5/C/05C1EC0E-D5EE-463B-BFE3-9311376A6809/NDP472-KB4054531-Web.exe';

procedure dotnetfx47(minVersion: integer);
begin
	if (netfxspversion(NetFx4x, '') < minVersion) then
		AddProduct('dotnetfx47.exe',
			'/lcid ' + CustomMessage('lcid') + ' /passive /norestart',
			CustomMessage('dotnetfx47_title'),
			CustomMessage('dotnetfx47_size'),
			dotnetfx47_url,
			false, false, false);
end;

[Setup]
