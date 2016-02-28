[CustomMessages]
mdac28_title=Microsoft Data Access Components 2.8

en.mdac28_size=5.4 MB
de.mdac28_size=5,4 MB


[Code]
const
	mdac28_url = 'http://download.microsoft.com/download/c/d/f/cdfd58f1-3973-4c51-8851-49ae3777586f/MDAC_TYP.EXE';

procedure mdac28(MinVersion: string);
var
	version: string;
begin
	//check for MDAC installation
	RegQueryStringValue(HKLM, 'Software\Microsoft\DataAccess', 'FullInstallVer', version);
	if (compareversion(version, MinVersion) < 0) then
		AddProduct('mdac28.exe',
			'/q:a /c:"install /qb /l"',
			CustomMessage('mdac28_title'),
			CustomMessage('mdac28_size'),
			mdac28_url,
			false, false);
end;