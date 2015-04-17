// requires Windows 2000 Service Pack 3, Windows 98, Windows 98 Second Edition, Windows ME, Windows Server 2003, Windows XP Service Pack 2
// requires internet explorer 5.0.1 or higher
// requires windows installer 2.0 on windows 98, ME
// requires Windows Installer 3.1 on windows 2000 or higher
// http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5

[CustomMessages]
dotnetfx20_title=.NET Framework 2.0

dotnetfx20_size=23 MB


[Code]
const
	dotnetfx20_url = 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe';
	dotnetfx20_url_x64 = 'http://download.microsoft.com/download/a/3/f/a3f1bf98-18f3-4036-9b68-8e6de530ce0a/NetFx64.exe';
	dotnetfx20_url_ia64 = 'http://download.microsoft.com/download/f/8/6/f86148a4-e8f7-4d08-a484-b4107f238728/NetFx64.exe';

procedure dotnetfx20();
begin
	if (not netfxinstalled(NetFx20, '')) then
		AddProduct('dotnetfx20' + GetArchitectureString() + '.exe',
			'/passive /norestart /lang:ENU',
			CustomMessage('dotnetfx20_title'),
			CustomMessage('dotnetfx20_size'),
			GetString(dotnetfx20_url, dotnetfx20_url_x64, dotnetfx20_url_ia64),
			false, false);
end;