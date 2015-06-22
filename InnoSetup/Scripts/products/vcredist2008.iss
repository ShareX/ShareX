// requires Windows 2000 Service Pack 4, Windows Server 2003, Windows Vista, Windows XP
// requires Windows Installer 3.0
// http://www.microsoft.com/en-us/download/details.aspx?id=29

[CustomMessages]
vcredist2008_title=Visual C++ 2008 Redistributable
vcredist2008_title_x64=Visual C++ 2008 64-Bit Redistributable
vcredist2008_title_ia64=Visual C++ 2008 Itanium Redistributable

en.vcredist2008_size=1.7 MB
de.vcredist2008_size=1,7 MB

en.vcredist2008_size_x64=2.3 MB
de.vcredist2008_size_x64=2,3 MB

en.vcredist2008_size_ia64=4.0 MB
de.vcredist2008_size_ia64=4,0 MB


[Code]
const
	vcredist2008_url = 'http://download.microsoft.com/download/1/1/1/1116b75a-9ec3-481a-a3c8-1777b5381140/vcredist_x86.exe';
	vcredist2008_url_x64 = 'http://download.microsoft.com/download/d/2/4/d242c3fb-da5a-4542-ad66-f9661d0a8d19/vcredist_x64.exe';
	vcredist2008_url_ia64 = 'http://download.microsoft.com/download/a/1/a/a1a4996b-ed78-4c2b-9589-8edd81b8df39/vcredist_IA64.exe';

	vcredist2008_productcode = '{FF66E9F6-83E7-3A3E-AF14-8DE9A809A6A4}';
	vcredist2008_productcode_x64 = '{350AA351-21FA-3270-8B7A-835434E766AD}';
	vcredist2008_productcode_ia64 = '{2B547B43-DB50-3139-9EBE-37D419E0F5FA}';

procedure vcredist2008();
begin
	if (not msiproduct(GetString(vcredist2008_productcode, vcredist2008_productcode_x64, vcredist2008_productcode_ia64))) then
		AddProduct('vcredist2008' + GetArchitectureString() + '.exe',
			'/q',
			CustomMessage('vcredist2008_title' + GetArchitectureString()),
			CustomMessage('vcredist2008_size' + GetArchitectureString()),
			GetString(vcredist2008_url, vcredist2008_url_x64, vcredist2008_url_ia64),
			false, false);
end;
