; requires Windows 10, Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003 Service Pack 2, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Vista Service Pack 2, Windows XP Service Pack 3
; http://www.microsoft.com/en-us/download/details.aspx?id=48145

[CustomMessages]
vcredist2015_title=Visual C++ 2015 Redistributable
vcredist2015_title_x64=Visual C++ 2015 64-Bit Redistributable

vcredist2015_size=13.8 MB
vcredist2015_size_x64=14.6 MB

[Code]
const
	vcredist2015_url = 'http://download.microsoft.com/download/d/e/c/dec58546-c2f5-40a7-b38e-4df8d60b9764/vc_redist.x86.exe';
	vcredist2015_url_x64 = 'http://download.microsoft.com/download/2/c/6/2c675af0-2155-4961-b32e-289d7addfcec/vc_redist.x64.exe';

	vcredist2015_upgradecode = '{65E5BD06-6392-3027-8C26-853107D3CF1A}';
	vcredist2015_upgradecode_x64 = '{36F68A90-239C-34DF-B58C-64B30153CE35}';

procedure vcredist2015(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2015_upgradecode, vcredist2015_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2015' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2015_title' + GetArchitectureString()),
				CustomMessage('vcredist2015_size' + GetArchitectureString()),
				GetString(vcredist2015_url, vcredist2015_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
