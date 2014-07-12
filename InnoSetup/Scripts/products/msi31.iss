[CustomMessages]
msi31_title=Windows Installer 3.1

en.msi31_size=2.5 MB
de.msi31_size=2,5 MB


[Code]
const
	msi31_url = 'http://download.microsoft.com/download/1/4/7/147ded26-931c-4daf-9095-ec7baf996f46/WindowsInstaller-KB893803-v2-x86.exe';

procedure msi31(MinVersion: string);
begin
	// Check for required Windows Installer 3.0 on Windows 2000 or higher
	if (IsX86() and minwinversion(5, 0) and (compareversion(fileversion(ExpandConstant('{sys}{\}msi.dll')), MinVersion) < 0)) then
		AddProduct('msi31.exe',
			'/passive /norestart',
			CustomMessage('msi31_title'),
			CustomMessage('msi31_size'),
			msi31_url,
			false, false);
end;