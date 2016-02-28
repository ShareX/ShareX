[CustomMessages]
de.dotnetfx11lp_title=.NET Framework 1.1 Sprachpaket: Deutsch

de.dotnetfx11lp_size=1,4 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
de.dotnetfx11lp_lcid=1031

de.dotnetfx11lp_url=http://download.microsoft.com/download/6/8/2/6821e687-526a-4ef8-9a67-9a402ec5ac9e/langpack.exe


[Code]
procedure dotnetfx11lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (IsX86() and not netfxinstalled(NetFx11, CustomMessage('dotnetfx11lp_lcid'))) then
			AddProduct('dotnetfx11' + ActiveLanguage() + '.exe',
				'/q:a /c:"inst.exe /qb /l"',
				CustomMessage('dotnetfx11lp_title'),
				CustomMessage('dotnetfx11lp_size'),
				CustomMessage('dotnetfx11lp_url'),
				false, false);
	end;
end;
