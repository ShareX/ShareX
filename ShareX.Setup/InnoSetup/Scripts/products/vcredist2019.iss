; requires Windows 10, Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003 Service Pack 2, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Vista Service Pack 2, Windows XP Service Pack 3
; https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads

[CustomMessages]
vcredist2019_title=Visual C++ 2015-2019 Redistributable (x86)
vcredist2019_title_x64=Visual C++ 2015-2019 Redistributable (x64)

vcredist2019_size=13.7 MB
vcredist2019_size_x64=14.3 MB

[Code]
const
	vcredist2019_url = 'http://aka.ms/vs/16/release/vc_redist.x86.exe';
	vcredist2019_url_x64 = 'http://aka.ms/vs/16/release/vc_redist.x64.exe';

	vcredist2019_upgradecode = '{65E5BD06-6392-3027-8C26-853107D3CF1A}';
	vcredist2019_upgradecode_x64 = '{36F68A90-239C-34DF-B58C-64B30153CE35}';

procedure vcredist2019(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2019_upgradecode, vcredist2019_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2019' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2019_title' + GetArchitectureString()),
				CustomMessage('vcredist2019_size' + GetArchitectureString()),
				GetString(vcredist2019_url, vcredist2019_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
