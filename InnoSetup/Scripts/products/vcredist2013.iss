// requires Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2003, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Server 2012 R2, Windows Vista Service Pack 2, Windows XP
// http://www.microsoft.com/en-US/download/details.aspx?id=40784

[CustomMessages]
vcredist2013_title=Visual C++ 2013 Redistributable
vcredist2013_title_x64=Visual C++ 2013 64-Bit Redistributable

en.vcredist2013_size=6.2 MB
de.vcredist2013_size=6,2 MB

en.vcredist2013_size_x64=6.9 MB
de.vcredist2013_size_x64=6,9 MB


[Code]
const
	vcredist2013_url = 'http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe';
	vcredist2013_url_x64 = 'http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x64.exe';

	vcredist2013_productcode = '{13A4EE12-23EA-3371-91EE-EFB36DDFFF3E}';
	vcredist2013_productcode_x64 = '{A749D8E6-B613-3BE3-8F5F-045C84EBA29B}';

procedure vcredist2013();
begin
	if (not IsIA64()) then begin
		if (not msiproduct(GetString(vcredist2013_productcode, vcredist2013_productcode_x64, ''))) then
			AddProduct('vcredist2013' + GetArchitectureString() + '.exe',
				'/passive /norestart',
				CustomMessage('vcredist2013_title' + GetArchitectureString()),
				CustomMessage('vcredist2013_size' + GetArchitectureString()),
				GetString(vcredist2013_url, vcredist2013_url_x64, ''),
				false, false);
	end;
end;
