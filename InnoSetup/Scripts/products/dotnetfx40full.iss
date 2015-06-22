// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1
// requires Internet Explorer 5.01
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
dotnetfx40full_title=.NET Framework 4.0 Full

dotnetfx40full_size=3 MB - 197 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.dotnetfx40full_lcid=
de.dotnetfx40full_lcid=/lcid 1031


[Code]
const
	dotnetfx40full_url = 'http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe';

procedure dotnetfx40full();
begin
	if (not netfxinstalled(NetFx40Full, '')) then
		AddProduct('dotNetFx40_Full_setup.exe',
			CustomMessage('dotnetfx40full_lcid') + ' /q /passive /norestart',
			CustomMessage('dotnetfx40full_title'),
			CustomMessage('dotnetfx40full_size'),
			dotnetfx40full_url,
			false, false);
end;