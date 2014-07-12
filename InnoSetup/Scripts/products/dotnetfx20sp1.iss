// requires Windows 2000 Service Pack 4, Windows Server 2003, Windows XP Service Pack 2
// requires KB 835732 on Windows 2000 Service Pack 4
// http://www.microsoft.com/downloads/details.aspx?FamilyID=79bc3b77-e02c-4ad3-aacf-a7633f706ba5

[CustomMessages]
dotnetfx20sp1_title=.NET Framework 2.0 Service Pack 1

en.dotnetfx20sp1_size=23.6 MB
de.dotnetfx20sp1_size=23,6 MB


[Code]
const
	dotnetfx20sp1_url = 'http://download.microsoft.com/download/0/8/c/08c19fa4-4c4f-4ffb-9d6c-150906578c9e/NetFx20SP1_x86.exe';
	dotnetfx20sp1_url_x64 = 'http://download.microsoft.com/download/9/8/6/98610406-c2b7-45a4-bdc3-9db1b1c5f7e2/NetFx20SP1_x64.exe';
	dotnetfx20sp1_url_ia64 = 'http://download.microsoft.com/download/c/9/7/c97d534b-8a55-495d-ab06-ad56f4b7f155/NetFx20SP1_ia64.exe';

procedure dotnetfx20sp1();
begin
	if (netfxspversion(NetFx20, '') < 1) then
		AddProduct('dotnetfx20sp1' + GetArchitectureString() + '.exe',
			'/passive /norestart /lang:ENU',
			CustomMessage('dotnetfx20sp1_title'),
			CustomMessage('dotnetfx20sp1_size'),
			GetString(dotnetfx20sp1_url, dotnetfx20sp1_url_x64, dotnetfx20sp1_url_ia64),
			false, false);
end;