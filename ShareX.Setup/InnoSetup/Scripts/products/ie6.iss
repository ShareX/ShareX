// requires Windows 2000; Windows 98; Windows ME; Windows NT; Windows XP Service Pack 1
// WARNING: express setup (downloads and installs the components depending on your OS)
// http://www.microsoft.com/downloads/details.aspx?familyid=1E1550CB-5E5D-48F5-B02B-20B602228DE6

[CustomMessages]
ie6_title=Internet Explorer 6

en.ie6_size=1 MB - 77.5 MB
de.ie6_size=1 MB - 77,5 MB


[Code]
const
	ie6_url = 'http://download.microsoft.com/download/ie6sp1/finrel/6_sp1/W98NT42KMeXP/EN-US/ie6setup.exe';

procedure ie6(MinVersion: string);
var
	version: string;
begin
	RegQueryStringValue(HKLM, 'Software\Microsoft\Internet Explorer', 'Version', version);
	if (compareversion(version, MinVersion) < 0) then
		AddProduct('ie6.exe',
			'/q:a /C:"setup /QNT"',
			CustomMessage('ie6_title'),
			CustomMessage('ie6_size'),
			ie6_url,
			false, false);
end;