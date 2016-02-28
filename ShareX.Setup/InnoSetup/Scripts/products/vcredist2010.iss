// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1 or later
// requires Internet Explorer 5.01 or later
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
vcredist2010_title=Visual C++ 2010 Redistributable

en.vcredist2010_size=4.8 MB
de.vcredist2010_size=4,8 MB

en.vcredist2010_size_x64=5.5 MB
de.vcredist2010_size_x64=5,5 MB

en.vcredist2010_size_ia64=2.2 MB
de.vcredist2010_size_ia64=2,2 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.vcredist2010_lcid=
de.vcredist2010_lcid='/lcid 1031 '


[Code]
const
	vcredist2010_url = 'http://download.microsoft.com/download/5/B/C/5BC5DBB3-652D-4DCE-B14A-475AB85EEF6E/vcredist_x86.exe';
	vcredist2010_url_x64 = 'http://download.microsoft.com/download/3/2/2/3224B87F-CFA0-4E70-BDA3-3DE650EFEBA5/vcredist_x64.exe';
	vcredist2010_url_ia64 = 'http://download.microsoft.com/download/3/3/A/33A75193-2CBC-424E-A886-287551FF1EB5/vcredist_IA64.exe';

procedure vcredist2010();
var
	version: cardinal;
begin
  RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\' + GetString('x86', 'x64', 'ia64'), 'Installed', version);

  if ((version <> 1)) then
    RegQueryDWordValue(HKLM, 'SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\VCRedist\' + GetString('x86', 'x64', 'ia64'), 'Installed', version);

	if ((version <> 1)) then
		AddProduct('vcredist2010' + GetArchitectureString() + '.exe',
			CustomMessage('vcredist2010_lcid') + '/passive /norestart',
			CustomMessage('vcredist2010_title'),
			CustomMessage('vcredist2010_size' + GetArchitectureString()),
			GetString(vcredist2010_url, vcredist2010_url_x64, vcredist2010_url_ia64),
			false, false);
end;