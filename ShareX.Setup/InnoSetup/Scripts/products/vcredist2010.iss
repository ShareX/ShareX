; requires Windows 7, Windows Server 2003 R2 (32-Bit x86), Windows Server 2003 Service Pack 2, Windows Server 2008 R2, Windows Server 2008 Service Pack 2, Windows Vista Service Pack 2, Windows XP Service Pack 3
; http://www.microsoft.com/en-us/download/details.aspx?id=5555

[CustomMessages]
vcredist2010_title=Visual C++ 2010 Redistributable
vcredist2010_title_x64=Visual C++ 2010 64-Bit Redistributable

vcredist2010_size=8.6 MB
vcredist2010_size_x64=9.8 MB

[Code]
const
	vcredist2010_url = 'http://download.microsoft.com/download/1/6/5/165255E7-1014-4D0A-B094-B6A430A6BFFC/vcredist_x86.exe';
	vcredist2010_url_x64 = 'http://download.microsoft.com/download/1/6/5/165255E7-1014-4D0A-B094-B6A430A6BFFC/vcredist_x64.exe';

	vcredist2010_upgradecode = '{1F4F1D2A-D9DA-32CF-9909-48485DA06DD5}';
	vcredist2010_upgradecode_x64 = '{5B75F761-BAC8-33BC-A381-464DDDD813A3}';

procedure vcredist2010(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2010_upgradecode, vcredist2010_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2010' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2010_title' + GetArchitectureString()),
				CustomMessage('vcredist2010_size' + GetArchitectureString()),
				GetString(vcredist2010_url, vcredist2010_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
