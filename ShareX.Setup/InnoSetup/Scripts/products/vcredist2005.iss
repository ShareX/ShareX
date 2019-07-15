; requires Windows 2000 Service Pack 3, Windows 98, Windows 98 Second Edition, Windows ME, Windows Server 2003, Windows XP Service Pack 2
; requires Windows Installer 3.0
; http://www.microsoft.com/en-us/download/details.aspx?id=3387

[CustomMessages]
vcredist2005_title=Visual C++ 2005 Redistributable
vcredist2005_title_x64=Visual C++ 2005 64-Bit Redistributable

vcredist2005_size=2.6 MB
vcredist2005_size_x64=4.1 MB

[Code]
const
	vcredist2005_url = 'http://download.microsoft.com/download/8/B/4/8B42259F-5D70-43F4-AC2E-4B208FD8D66A/vcredist_x86.EXE';
	vcredist2005_url_x64 = 'http://download.microsoft.com/download/8/B/4/8B42259F-5D70-43F4-AC2E-4B208FD8D66A/vcredist_x64.EXE';

	vcredist2005_upgradecode = '{86C9D5AA-F00C-4921-B3F2-C60AF92E2844}';
	vcredist2005_upgradecode_x64 = '{A8D19029-8E5C-4E22-8011-48070F9E796E}';

procedure vcredist2005(minVersion: string);
begin
	if (not IsIA64()) then begin
		if (not msiproductupgrade(GetString(vcredist2005_upgradecode, vcredist2005_upgradecode_x64, ''), minVersion)) then
			AddProduct('vcredist2005' + GetArchitectureString() + '.exe',
				'/q:a /c:"install /qb /l',
				CustomMessage('vcredist2005_title' + GetArchitectureString()),
				CustomMessage('vcredist2005_size' + GetArchitectureString()),
				GetString(vcredist2005_url, vcredist2005_url_x64, ''),
				false, false, false);
	end;
end;

[Setup]
