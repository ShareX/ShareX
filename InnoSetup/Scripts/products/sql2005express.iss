// SQL Server Express is supported on x64 and EMT64 systems in Windows On Windows (WOW). SQL Server Express is not supported on IA64 systems
// requires Microsoft .NET Framework 2.0 or later
// SQLEXPR32.EXE is a smaller package that can be used to install SQL Server Express on 32-bit operating systems only. The larger SQLEXPR.EXE package supports installing onto both 32-bit and 64-bit (WOW install) operating systems. There is no other difference between these packages.
// http://www.microsoft.com/download/en/details.aspx?id=15291

[CustomMessages]
sql2005express_title=SQL Server 2005 Express SP3

en.sql2005express_size=38.1 MB
de.sql2005express_size=38,1 MB

en.sql2005express_size_x64=58.1 MB
de.sql2005express_size_x64=58,1 MB


[Code]
const
	sql2005express_url = 'http://download.microsoft.com/download/4/B/E/4BED5810-C8C0-4697-BDC3-DBC114B8FF6D/SQLEXPR32_NLA.EXE';
	sql2005express_url_x64 = 'http://download.microsoft.com/download/4/B/E/4BED5810-C8C0-4697-BDC3-DBC114B8FF6D/SQLEXPR_NLA.EXE';

procedure sql2005express();
var
	version: string;
begin
	//CHECK NOT FINISHED YET
	//RTM: 9.00.1399.06
	//Service Pack 1: 9.1.2047.00
	//Service Pack 2: 9.2.3042.00
	// Newer detection method required for SP3 and x64
	//Service Pack 3: 9.00.4035.00
	//RegQueryDWordValue(HKLM, 'Software\Microsoft\Microsoft SQL Server\90\DTS\Setup', 'Install', version);
	RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\SQLEXPRESS\MSSQLServer\CurrentVersion', 'CurrentVersion', version);
	if (version < '9.00.4035') then begin
		if (not isIA64()) then
			AddProduct('sql2005express' + GetArchitectureString() + '.exe',
				'/qb ADDLOCAL=ALL INSTANCENAME=SQLEXPRESS',
				CustomMessage('sql2005express_title'),
				CustomMessage('sql2005express_size' + GetArchitectureString()),
				GetString(sql2005express_url, sql2005express_url_x64, ''),
				false, false);
	end;
end;
