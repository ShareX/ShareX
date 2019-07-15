; requires Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Server 2012 R2, Windows Vista Service Pack 2, Windows XP
; http://www.microsoft.com/en-us/download/details.aspx?id=40784

[CustomMessages]
vcredist2013_title=Visual C++ 2013 Redistributable
vcredist2013_title_x64=Visual C++ 2013 64-Bit Redistributable

vcredist2013_size=6.2 MB
vcredist2013_size_x64=6.9 MB

[Code]
const
	vcredist2013_url = 'http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe';
	vcredist2013_url_x64 = 'http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x64.exe';

	vcredist2013_upgradecode = '{B59F5BF1-67C8-3802-8E59-2CE551A39FC5}';
	vcredist2013_upgradecode_x64 = '{20400CF0-DE7C-327E-9AE4-F0F38D9085F8}';

procedure vcredist2013(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2013_upgradecode, vcredist2013_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2013' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2013_title' + GetArchitectureString()),
				CustomMessage('vcredist2013_size' + GetArchitectureString()),
				GetString(vcredist2013_url, vcredist2013_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
