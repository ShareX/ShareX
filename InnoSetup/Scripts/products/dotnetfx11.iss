// requires Windows 2000; Windows Server 2003 Service Pack 1 for Itanium-based Systems; Windows Server 2003 x64 editions; Windows Server 2008 Datacenter; Windows Server 2008 Enterprise; Windows Server 2008 for Itanium-based Systems; Windows Server 2008 Standard; Windows Vista Business; Windows Vista Enterprise; Windows Vista Home Basic; Windows Vista Home Premium; Windows Vista Starter; Windows Vista Ultimate; Windows XP; Windows XP Professional x64 Edition; Windows NT 4.0 Service Pack 6a
// requires internet explorer 5.0.1 or higher
// http://www.microsoft.com/downloads/details.aspx?FamilyID=262d25e3-f589-4842-8157-034d1e7cf3a3

[CustomMessages]
dotnetfx11_title=.NET Framework 1.1

en.dotnetfx11_size=23.1 MB
de.dotnetfx11_size=23,1 MB


[Code]
const
	dotnetfx11_url = 'http://download.microsoft.com/download/a/a/c/aac39226-8825-44ce-90e3-bf8203e74006/dotnetfx.exe';

procedure dotnetfx11();
begin
	if (IsX86() and not netfxinstalled(NetFx11, '')) then
		AddProduct('dotnetfx11.exe',
			'/q:a /c:"install /qb /l"',
			CustomMessage('dotnetfx11_title'),
			CustomMessage('dotnetfx11_size'),
			dotnetfx11_url,
			false, false);
end;
