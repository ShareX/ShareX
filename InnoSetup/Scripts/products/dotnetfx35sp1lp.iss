[CustomMessages]
de.dotnetfx35sp1lp_title=.NET Framework 3.5 SP1 Sprachpaket: Deutsch

de.dotnetfx35sp1lp_size=22 MB - 98 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
de.dotnetfx35sp1lp_lcid=1031

de.dotnetfx35sp1lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe


[Code]
procedure dotnetfx35sp1lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (netfxspversion(NetFx35, CustomMessage('dotnetfx35sp1lp_lcid')) < 1) then
			AddProduct('dotnetfx35sp1_' + ActiveLanguage() + '.exe',
				'/lang:enu /passive /norestart',
				CustomMessage('dotnetfx35sp1lp_title'),
				CustomMessage('dotnetfx35sp1lp_size'),
				CustomMessage('dotnetfx35sp1lp_url'),
				false, false);
	end;
end;