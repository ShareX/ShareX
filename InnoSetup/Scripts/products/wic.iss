//requires Windows Server 2003, Windows Server 2003 R2 Datacenter Edition (32-Bit x86), Windows Server 2003 R2 Enterprise Edition (32-Bit x86), Windows Server 2003 R2 Standard Edition (32-bit x86), Windows XP Service Pack 2

[CustomMessages]
wic_title=Windows Imaging Component
 
en.wic_size=1.2 MB
de.wic_size=1,2 MB
 

[Code]
const
	wic_url = 'http://download.microsoft.com/download/f/f/1/ff178bb1-da91-48ed-89e5-478a99387d4f/wic_x86_';
	wic_url_x64 = 'http://download.microsoft.com/download/6/4/5/645fed5f-a6e7-44d9-9d10-fe83348796b0/wic_x64';

function GetConvertedLanguageID(): string;
begin
	case ActiveLanguage() of
		'en': //English
			Result := 'enu';
		'zh': //Chinese
			Result := 'chs';
		'de': //German
			Result := 'deu';
		'es': //Spanish
			Result := 'esn';
		'fr': //French
			Result := 'fra';
		'it': //Italian
			Result := 'ita';
		'ja': //Japanese
			Result := 'jpn';
		'nl': //Dutch
			Result := 'nld';
		'pt': //Portuguese
			Result := 'ptb';
		'ru': //Russian
			Result := 'rus';
	end;
end;

procedure wic();
begin
	if (not IsIA64()) then begin
		//only needed on Windows XP SP2 or Windows Server 2003
		if ((exactwinversion(5, 1) and exactwinspversion(5, 1, 2)) or (exactwinversion(5, 2))) then begin
			if (not FileExists(GetEnv('windir') + '\system32\windowscodecs.dll')) then
				AddProduct('wic' + GetArchitectureString() + '_' + GetConvertedLanguageID() + '.exe',
					'/q',
					CustomMessage('wic_title'),
					CustomMessage('wic_size'),
					GetString(wic_url, wic_url_x64, '') + GetConvertedLanguageID() + '.exe',
					false, false);
		end;
	end;
end;