; requires Windows 2000 Service Pack 4, Windows Server 2003, Windows Vista, Windows XP
; requires Windows Installer 3.0
; http://www.microsoft.com/en-us/download/details.aspx?id=29

[CustomMessages]
vcredist2008_title=Visual C++ 2008 Redistributable
vcredist2008_title_x64=Visual C++ 2008 64-Bit Redistributable

vcredist2008_size=4.3 MB
vcredist2008_size_x64=5.0 MB

[Code]
const
	vcredist2008_url = 'http://download.microsoft.com/download/5/D/8/5D8C65CB-C849-4025-8E95-C3966CAFD8AE/vcredist_x86.exe';
	vcredist2008_url_x64 = 'http://download.microsoft.com/download/5/D/8/5D8C65CB-C849-4025-8E95-C3966CAFD8AE/vcredist_x64.exe';

	vcredist2008_upgradecode = '{AA783A14-A7A3-3D33-95F0-9A351D530011}';
	vcredist2008_upgradecode_sp1mfc = '{DE2C306F-A067-38EF-B86C-03DE4B0312F9}';
	vcredist2008_upgradecode_sp1mfc_x64 = '{FDA45DDF-8E17-336F-A3ED-356B7B7C688A}';

procedure vcredist2008(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2008_upgradecode_sp1mfc, vcredist2008_upgradecode_sp1mfc_x64, ''), minVersion) and not msiproductupgrade(vcredist2008_upgradecode, minVersion)) then
			AddProduct('vcredist2008' + GetArchitectureString() + '.exe',
				'/q',
				CustomMessage('vcredist2008_title' + GetArchitectureString()),
				CustomMessage('vcredist2008_size' + GetArchitectureString()),
				GetString(vcredist2008_url, vcredist2008_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
