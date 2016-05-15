// http://support.microsoft.com/kb/239114

[CustomMessages]
jet4sp8_title=Jet 4

en.jet4sp8_size=3.7 MB
de.jet4sp8_size=3,7 MB


[Code]
const
	jet4sp8_url = 'http://download.microsoft.com/download/4/3/9/4393c9ac-e69e-458d-9f6d-2fe191c51469/Jet40SP8_9xNT.exe';

procedure jet4sp8(MinVersion: string);
begin
	//check for Jet4 Service Pack 8 installation
	if (compareversion(fileversion(ExpandConstant('{sys}{\}msjet40.dll')), MinVersion) < 0) then
		AddProduct('jet4sp8.exe',
			'/q:a /c:"install /qb /l"',
			CustomMessage('jet4sp8_title'),
			CustomMessage('jet4sp8_size'),
			jet4sp8_url,
			false, false);
end;