// requires Windows 7, Windows Server 2003, Windows Server 2008, Windows Server 2008 R2, Windows Vista, Windows XP
// requires Microsoft .NET Framework 3.5 SP 1 or later
// requires Windows Installer 4.5 or later
// SQL Server Express is supported on x64 and EMT64 systems in Windows On Windows (WOW). SQL Server Express is not supported on IA64 systems
// SQLEXPR32.EXE is a smaller package that can be used to install SQL Server Express on 32-bit operating systems only. The larger SQLEXPR.EXE package supports installing onto both 32-bit and 64-bit (WOW install) operating systems. There is no other difference between these packages.
// http://www.microsoft.com/download/en/details.aspx?id=3743

[CustomMessages]
sql2008expressr2_title=SQL Server 2008 Express R2

en.sql2008expressr2_size=58.2 MB
de.sql2008expressr2_size=58,2 MB

en.sql2008expressr2_size_x64=74.1 MB
de.sql2008expressr2_size_x64=74,1 MB


[Code]
const
	sql2008expressr2_url = 'http://download.microsoft.com/download/5/1/A/51A153F6-6B08-4F94-A7B2-BA1AD482BC75/SQLEXPR32_x86_ENU.exe';
	sql2008expressr2_url_x64 = 'http://download.microsoft.com/download/5/1/A/51A153F6-6B08-4F94-A7B2-BA1AD482BC75/SQLEXPR_x64_ENU.exe';

procedure sql2008express();
var
	version: string;
begin
	// This check does not take into account that a full version of SQL Server could be installed,
	// making Express unnecessary.
	RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\SQLEXPRESS\MSSQLServer\CurrentVersion', 'CurrentVersion', version);
	if (compareversion(version, '10.5') < 0) then begin
		if (not IsIA64()) then
			AddProduct('sql2008expressr2' + GetArchitectureString() + '.exe',
				'/QS  /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=All /INSTANCENAME=SQLEXPRESS /SQLSVCACCOUNT="NT AUTHORITY\Network Service" /SQLSYSADMINACCOUNTS="builtin\administrators"',
				CustomMessage('sql2008expressr2_title'),
				CustomMessage('sql2008expressr2_size' + GetArchitectureString()),
				GetString(sql2008expressr2_url, sql2008expressr2_url_x64, ''),
				false, false);
	end;
end;
