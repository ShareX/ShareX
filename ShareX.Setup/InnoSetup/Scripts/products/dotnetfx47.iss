; requires Windows 10, Windows 7 Service Pack 1, Windows 8.1, Windows Server 2008 R2 SP1, Windows Server 2012, Windows Server 2012 R2, Windows Server 2016
; WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
; https://www.microsoft.com/en-US/download/details.aspx?id=55170

[CustomMessages]
dotnetfx47_title=.NET Framework 4.7

dotnetfx47_size=1 MB - 59 MB

[Code]
const
	dotnetfx47_url = 'http://download.microsoft.com/download/A/E/A/AEAE0F3F-96E9-4711-AADA-5E35EF902306/NDP47-KB3186500-Web.exe';

procedure dotnetfx47(minVersion: integer);
begin
	if (not netfxinstalled(NetFx4x, '') or (netfxspversion(NetFx4x, '') < minVersion)) then
		AddProduct('dotnetfx47.exe',
			'/lcid ' + CustomMessage('lcid') + ' /passive /norestart',
			CustomMessage('dotnetfx47_title'),
			CustomMessage('dotnetfx47_size'),
			dotnetfx47_url,
			false, false, false);
end;

[Setup]
