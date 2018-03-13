; requires Windows 10, Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003 Service Pack 2, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Vista Service Pack 2, Windows XP Service Pack 3
; http://www.visualstudio.com/en-us/downloads/

[CustomMessages]
vcredist2017_title=Visual C++ 2017 Redistributable
vcredist2017_title_x64=Visual C++ 2017 64-Bit Redistributable

vcredist2017_size=13.7 MB
vcredist2017_size_x64=14.5 MB

[Code]
const
	vcredist2017_url = 'http://download.microsoft.com/download/1/f/e/1febbdb2-aded-4e14-9063-39fb17e88444/vc_redist.x86.exe';
	vcredist2017_url_x64 = 'http://download.microsoft.com/download/3/b/f/3bf6e759-c555-4595-8973-86b7b4312927/vc_redist.x64.exe';

	vcredist2017_upgradecode = '{65E5BD06-6392-3027-8C26-853107D3CF1A}';
	vcredist2017_upgradecode_x64 = '{36F68A90-239C-34DF-B58C-64B30153CE35}';

procedure vcredist2017(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2017_upgradecode, vcredist2017_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2017' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2017_title' + GetArchitectureString()),
				CustomMessage('vcredist2017_size' + GetArchitectureString()),
				GetString(vcredist2017_url, vcredist2017_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
