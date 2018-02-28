; requires Windows 7, Windows Server 2003 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Vista, Windows XP Service Pack 2, Windows XP Service Pack 3
; http://www.microsoft.com/en-US/download/details.aspx?id=35

[CustomMessages]
en.directxruntime_title=DirectX End-User Runtime
de.directxruntime_title=DirectX Endbenutzer Runtime

directxruntime_size=1 MB - 95.6 MB

[Files]
;includes dxwebsetup.exe in setup executable so that we don't need to download it
Source: "src\dxwebsetup.exe"; Flags: dontcopy

[Code]
const
	directxruntime_url = 'http://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe';

procedure directxruntime();
begin
	ExtractTemporaryFile('dxwebsetup.exe');

	AddProduct('dxwebsetup.exe',
		'/Q',
		CustomMessage('directxruntime_title'),
		CustomMessage('directxruntime_size'),
		directxruntime_url,
		true, false, false);
end;

[Setup]
