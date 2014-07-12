[CustomMessages]
msi20_title=Windows Installer 2.0

en.msi20_size=1.7 MB
de.msi20_size=1,7 MB


[Code]
const
	msi20_url = 'http://download.microsoft.com/download/WindowsInstaller/Install/2.0/W9XMe/EN-US/InstMsiA.exe';

procedure msi20(MinVersion: string);
begin
	// Check for required Windows Installer 2.0 on Windows 98 and ME
	if (IsX86() and maxwinversion(4, 9) and (compareversion(fileversion(ExpandConstant('{sys}{\}msi.dll')), MinVersion) < 0)) then
		AddProduct('msi20.exe',
			'/q:a /c:"msiinst /delayrebootq"',
			CustomMessage('msi20_title'),
			CustomMessage('msi20_size'),
			msi20_url,
			false, false);
end;