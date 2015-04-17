[CustomMessages]
sqlcompact35sp2_title=SQL Server Compact 3.5 Service Pack 2

en.sqlcompact35sp2_size=5.3 MB
de.sqlcompact35sp2_size=5,3 MB


[Code]
const
	sqlcompact35sp2_url = 'http://download.microsoft.com/download/E/C/1/EC1B2340-67A0-4B87-85F0-74D987A27160/SSCERuntime-ENU.exe';

procedure sqlcompact35sp2();
begin
	if (isX86() and not RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v3.5')) then
		AddProduct('sqlcompact35sp2.msi',
			'/qb',
			CustomMessage('sqlcompact35sp2_title'),
			CustomMessage('sqlcompact35sp2_size'),
			sqlcompact35sp2_url,
			false, false);
end;
