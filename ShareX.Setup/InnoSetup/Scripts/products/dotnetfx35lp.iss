[CustomMessages]
de.dotnetfx35lp_title=.NET Framework 3.5 Sprachpaket: Deutsch

dotnetfx35lp_size=13 MB - 51 MB

dotnetfx35lp_url=
de.dotnetfx35lp_url=http://download.microsoft.com/download/d/1/e/d1e617c3-c7f4-467e-a7de-af832450efd3/dotnetfx35langpack_x86de.exe

[Code]
procedure dotnetfx35lp();
begin
	if (CustomMessage('dotnetfx35lp_url') <> '') then begin
		if (not netfxinstalled(NetFx35, CustomMessage('lcid'))) then
			AddProduct('dotnetfx35_' + ActiveLanguage() + '.exe',
				'/lang:enu /passive /norestart',
				CustomMessage('dotnetfx35lp_title'),
				CustomMessage('dotnetfx35lp_size'),
				CustomMessage('dotnetfx35lp_url'),
				false, false, false);
	end;
end;

[Setup]
