// requires Windows 2000 Service Pack 3, Windows 98, Windows 98 Second Edition, Windows ME, Windows Server 2003, Windows XP Service Pack 2
// requires Windows Installer 3.0
// http://www.microsoft.com/en-us/download/details.aspx?id=3387

[CustomMessages]
vcredist2005_title=Visual C++ 2005 Redistributable
vcredist2005_title_x64=Visual C++ 2005 64-Bit Redistributable
vcredist2005_title_ia64=Visual C++ 2005 Itanium Redistributable

en.vcredist2005_size=2.6 MB
de.vcredist2005_size=2,6 MB

en.vcredist2005_size_x64=4.1 MB
de.vcredist2005_size_x64=4,1 MB

en.vcredist2005_size_ia64=8.8 MB
de.vcredist2005_size_ia64=8,8 MB


[Code]
const
	vcredist2005_url = 'http://download.microsoft.com/download/d/3/4/d342efa6-3266-4157-a2ec-5174867be706/vcredist_x86.exe';
	vcredist2005_url_x64 = 'http://download.microsoft.com/download/9/1/4/914851c6-9141-443b-bdb4-8bad3a57bea9/vcredist_x64.exe';
	vcredist2005_url_ia64 = 'http://download.microsoft.com/download/8/1/6/816129e4-7f2f-4ba6-b065-684223e2fe1e/vcredist_IA64.exe';

	vcredist2005_productcode = '{A49F249F-0C91-497F-86DF-B2585E8E76B7}';
	vcredist2005_productcode_x64 = '{6E8E85E8-CE4B-4FF5-91F7-04999C9FAE6A}';
	vcredist2005_productcode_ia64 = '{03ED71EA-F531-4927-AABD-1C31BCE8E187}';

procedure vcredist2005();
begin
	if (not msiproduct(GetString(vcredist2005_productcode, vcredist2005_productcode_x64, vcredist2005_productcode_ia64))) then
		AddProduct('vcredist2005' + GetArchitectureString() + '.exe',
			'/q:a /c:"install /qb /l',
			CustomMessage('vcredist2005_title' + GetArchitectureString()),
			CustomMessage('vcredist2005_size' + GetArchitectureString()),
			GetString(vcredist2005_url, vcredist2005_url_x64, vcredist2005_url_ia64),
			false, false);
end;
