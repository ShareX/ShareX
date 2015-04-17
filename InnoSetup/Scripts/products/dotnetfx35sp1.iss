// requires Windows Server 2003 Service Pack 1, Windows Server 2008, Windows Vista, Windows XP Service Pack 2
// requires Windows Installer 3.1
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7

[CustomMessages]
dotnetfx35sp1_title=.NET Framework 3.5 Service Pack 1

en.dotnetfx35sp1_size=3 MB - 232 MB
de.dotnetfx35sp1_size=3 MB - 232 MB


[Code]
const
	dotnetfx35sp1_url = 'http://download.microsoft.com/download/0/6/1/061f001c-8752-4600-a198-53214c69b51f/dotnetfx35setup.exe';

procedure dotnetfx35sp1();
begin
	if (netfxspversion(NetFx35, '') < 1) then
		AddProduct('dotnetfx35sp1' + GetArchitectureString() + '.exe',
			'/lang:enu /passive /norestart',
			CustomMessage('dotnetfx35sp1_title'),
			CustomMessage('dotnetfx35sp1_size'),
			dotnetfx35sp1_url,
			false, false);
end;