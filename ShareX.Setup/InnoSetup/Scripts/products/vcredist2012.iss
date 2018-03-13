; requires Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Vista Service Pack 2, Windows XP
; http://www.microsoft.com/en-us/download/details.aspx?id=30679

[CustomMessages]
vcredist2012_title=Visual C++ 2012 Redistributable
vcredist2012_title_x64=Visual C++ 2012 64-Bit Redistributable

vcredist2012_size=6.3 MB
vcredist2012_size_x64=6.9 MB

[Code]
const
	vcredist2012_url = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU_4/vcredist_x86.exe';
	vcredist2012_url_x64 = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU_4/vcredist_x64.exe';

	vcredist2012_upgradecode = '{4121ED58-4BD9-3E7B-A8B5-9F8BAAE045B7}';
	vcredist2012_upgradecode_x64 = '{EFA6AFA1-738E-3E00-8101-FD03B86B29D1}';

procedure vcredist2012(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2012_upgradecode, vcredist2012_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2012' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2012_title' + GetArchitectureString()),
				CustomMessage('vcredist2012_size' + GetArchitectureString()),
				GetString(vcredist2012_url, vcredist2012_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
