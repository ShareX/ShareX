// requires Windows 7, Windows Server 2003 R2 (32-Bit x86), Windows Server 2003 Service Pack 2, Windows Server 2008 R2, Windows Server 2008 Service Pack 2, Windows Vista Service Pack 2, Windows XP Service Pack 3
// http://www.microsoft.com/en-us/download/details.aspx?id=5555

[CustomMessages]
vcredist2010_title=Visual C++ 2010 Redistributable
vcredist2010_title_x64=Visual C++ 2010 64-Bit Redistributable
vcredist2010_title_ia64=Visual C++ 2010 Itanium Redistributable

en.vcredist2010_size=4.8 MB
de.vcredist2010_size=4,8 MB

en.vcredist2010_size_x64=5.5 MB
de.vcredist2010_size_x64=5,5 MB

en.vcredist2010_size_ia64=2.2 MB
de.vcredist2010_size_ia64=2,2 MB


[Code]
const
	vcredist2010_url = 'http://download.microsoft.com/download/5/B/C/5BC5DBB3-652D-4DCE-B14A-475AB85EEF6E/vcredist_x86.exe';
	vcredist2010_url_x64 = 'http://download.microsoft.com/download/3/2/2/3224B87F-CFA0-4E70-BDA3-3DE650EFEBA5/vcredist_x64.exe';
	vcredist2010_url_ia64 = 'http://download.microsoft.com/download/3/3/A/33A75193-2CBC-424E-A886-287551FF1EB5/vcredist_IA64.exe';

	vcredist2010_productcode = '{196BB40D-1578-3D01-B289-BEFC77A11A1E}';
	vcredist2010_productcode_x64 = '{DA5E371C-6333-3D8A-93A4-6FD5B20BCC6E}';
	vcredist2010_productcode_ia64 = '{C1A35166-4301-38E9-BA67-02823AD72A1B}';

procedure vcredist2010();
begin
	if (not msiproduct(GetString(vcredist2010_productcode, vcredist2010_productcode_x64, vcredist2010_productcode_ia64))) then
		AddProduct('vcredist2010' + GetArchitectureString() + '.exe',
			'/q /norestart',
			CustomMessage('vcredist2010_title' + GetArchitectureString()),
			CustomMessage('vcredist2010_size' + GetArchitectureString()),
			GetString(vcredist2010_url, vcredist2010_url_x64, vcredist2010_url_ia64),
			false, false);
end;
