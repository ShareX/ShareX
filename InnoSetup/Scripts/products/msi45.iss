[CustomMessages]
msi45_title=Windows Installer 4.5

en.msi45win60_size=1.7 MB
de.msi45win60_size=1,7 MB

en.msi45win52_size=3.0 MB
de.msi45win52_size=3,0 MB

en.msi45win51_size=3.2 MB
de.msi45win51_size=3,2 MB


[Code]
const
	msi45win60_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/Windows6.0-KB942288-v2-x86.msu';
	msi45win52_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/WindowsServer2003-KB942288-v4-x86.exe';
	msi45win51_url = 'http://download.microsoft.com/download/2/6/1/261fca42-22c0-4f91-9451-0e0f2e08356d/WindowsXP-KB942288-v3-x86.exe';

procedure msi45(MinVersion: string);
begin
	if (IsX86() and (compareversion(fileversion(ExpandConstant('{sys}{\}msi.dll')), MinVersion) < 0)) then begin
		if minwinversion(6, 0) then
			AddProduct('msi45_60.msu',
				'/quiet /norestart',
				CustomMessage('msi45_title'),
				CustomMessage('msi45win60_size'),
				msi45win60_url,
				false, false)
		else if minwinversion(5, 2) then
			AddProduct('msi45_52.exe',
				'/quiet /norestart',
				CustomMessage('msi45_title'),
				CustomMessage('msi45win52_size'),
				msi45win52_url,
				false, false)
		else if minwinversion(5, 1) then
			AddProduct('msi45_51.exe',
				'/quiet /norestart',
				CustomMessage('msi45_title'),
				CustomMessage('msi45win51_size'),
				msi45win51_url,
				false, false);
	end;
end;