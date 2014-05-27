[Code]
function GetFullVersion(VersionMS, VersionLS: cardinal): string;
var
	version: string;
begin
	version := IntToStr(word(VersionMS shr 16));
	version := version + '.' + IntToStr(word(VersionMS and not $ffff0000));

	version := version + '.' + IntToStr(word(VersionLS shr 16));
	version := version + '.' + IntToStr(word(VersionLS and not $ffff0000));

	Result := version;
end;

function fileversion(file: string): string;
var
	versionMS, versionLS: cardinal;
begin
	if GetVersionNumbers(file, versionMS, versionLS) then
		Result := GetFullVersion(versionMS, versionLS)
	else
		Result := '0';
end;
