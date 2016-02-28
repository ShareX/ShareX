[Code]
var
	WindowsVersion: TWindowsVersion;

procedure initwinversion();
begin
	GetWindowsVersionEx(WindowsVersion);
end;

function exactwinversion(MajorVersion, MinorVersion: integer): boolean;
begin
	Result := (WindowsVersion.Major = MajorVersion) and (WindowsVersion.Minor = MinorVersion);
end;

function minwinversion(MajorVersion, MinorVersion: integer): boolean;
begin
	Result := (WindowsVersion.Major > MajorVersion) or ((WindowsVersion.Major = MajorVersion) and (WindowsVersion.Minor >= MinorVersion));
end;

function maxwinversion(MajorVersion, MinorVersion: integer): boolean;
begin
	Result := (WindowsVersion.Major < MajorVersion) or ((WindowsVersion.Major = MajorVersion) and (WindowsVersion.Minor <= MinorVersion));
end;

function exactwinspversion(MajorVersion, MinorVersion, SpVersion: integer): boolean;
begin
	if exactwinversion(MajorVersion, MinorVersion) then
		Result := WindowsVersion.ServicePackMajor = SpVersion
	else
		Result := true;
end;

function minwinspversion(MajorVersion, MinorVersion, SpVersion: integer): boolean;
begin
	if exactwinversion(MajorVersion, MinorVersion) then
		Result := WindowsVersion.ServicePackMajor >= SpVersion
	else
		Result := true;
end;

function maxwinspversion(MajorVersion, MinorVersion, SpVersion: integer): boolean;
begin
	if exactwinversion(MajorVersion, MinorVersion) then
		Result := WindowsVersion.ServicePackMajor <= SpVersion
	else
		Result := true;
end;