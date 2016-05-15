// requires TabletPC, Windows 2000, Windows 2000 Advanced Server, Windows 2000 Professional Edition , Windows 2000 Server, Windows 2000 Service Pack 2, Windows 2000 Service Pack 3, Windows 2000 Service Pack 4, Windows Server 2003 Service Pack 1 for Itanium-based Systems, Windows Server 2003 x64 editions, Windows Server 2003, Datacenter Edition for 64-Bit Itanium-Based Systems, Windows Server 2003, Datacenter x64 Edition, Windows Server 2003, Enterprise Edition for Itanium-based Systems, Windows Server 2003, Enterprise x64 Edition, Windows Server 2003, Standard x64 Edition, Windows Server 2008 Datacenter, Windows Server 2008 Enterprise, Windows Server 2008 for Itanium-based Systems, Windows Server 2008 Standard, Windows Vista Business, Windows Vista Business 64-bit edition, Windows Vista Enterprise, Windows Vista Enterprise 64-bit edition, Windows Vista Home Basic, Windows Vista Home Basic 64-bit edition, Windows Vista Home Premium, Windows Vista Home Premium 64-bit edition, Windows Vista Starter, Windows Vista Ultimate, Windows Vista Ultimate 64-bit edition, Windows XP, Windows XP Home Edition , Windows XP Media Center Edition, Windows XP Professional Edition , Windows XP Professional x64 Edition , Windows XP Service Pack 1, Windows XP Service Pack 2
// requires internet explorer 5.0.1 or higher
// http://www.microsoft.com/downloads/details.aspx?familyid=A8F5654F-088E-40B2-BBDB-A83353618B38

[CustomMessages]
dotnetfx11sp1_title=.NET Framework 1.1 Service Pack 1

en.dotnetfx11sp1_size=10.5 MB
de.dotnetfx11sp1_size=10,5 MB


[Code]
const
	dotnetfx11sp1_url = 'http://download.microsoft.com/download/8/b/4/8b4addd8-e957-4dea-bdb8-c4e00af5b94b/NDP1.1sp1-KB867460-X86.exe';

procedure dotnetfx11sp1();
begin
	if (IsX86() and (netfxspversion(NetFx11, '') < 1)) then
		AddProduct('dotnetfx11sp1.exe',
			'/q',
			CustomMessage('dotnetfx11sp1_title'),
			CustomMessage('dotnetfx11sp1_size'),
			dotnetfx11sp1_url,
			false, false);
end;