[Code]
type
	NetFXType = (NetFx10, NetFx11, NetFx20, NetFx30, NetFx35, NetFx40Client, NetFx40Full, NetFx4x);

const
	netfx11plus_reg = 'Software\Microsoft\NET Framework Setup\NDP\';

function netfxinstalled(version: NetFXType; lcid: string): boolean;
var
	regVersion: cardinal;
	regVersionString: string;
begin
	if (lcid <> '') then
		lcid := '\' + lcid;

	if (version = NetFx10) then begin
		RegQueryStringValue(HKLM, 'Software\Microsoft\.NETFramework\Policy\v1.0\3705', 'Install', regVersionString);
		Result := regVersionString <> '';
	end else begin
		case version of
			NetFx11:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v1.1.4322' + lcid, 'Install', regVersion);
			NetFx20:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v2.0.50727' + lcid, 'Install', regVersion);
			NetFx30:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v3.0\Setup' + lcid, 'InstallSuccess', regVersion);
			NetFx35:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v3.5' + lcid, 'Install', regVersion);
			NetFx40Client:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Client' + lcid, 'Install', regVersion);
			NetFx40Full:
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Full' + lcid, 'Install', regVersion);
			NetFx4x:
			begin
				RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Full' + lcid, 'Release', regVersion);
				Result := (regVersion >= 378389); // 4.5.0+
				Exit;
			end;
		end;
		Result := (regVersion <> 0);
	end;
end;

function netfxspversion(version: NetFXType; lcid: string): integer;
var
	regVersion: cardinal;
begin
	if (lcid <> '') then
		lcid := '\' + lcid;

	case version of
		NetFx10:
			// not supported
			regVersion := -1;
		NetFx11:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v1.1.4322' + lcid, 'SP', regVersion)) then
				regVersion := -1;
		NetFx20:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v2.0.50727' + lcid, 'SP', regVersion)) then
				regVersion := -1;
		NetFx30:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v3.0' + lcid, 'SP', regVersion)) then
				regVersion := -1;
		NetFx35:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v3.5' + lcid, 'SP', regVersion)) then
				regVersion := -1;
		NetFx40Client:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Client' + lcid, 'Servicing', regVersion)) then
				regVersion := -1;
		NetFx40Full:
			if (not RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Full' + lcid, 'Servicing', regVersion)) then
				regVersion := -1;
		NetFx4x:
			if (RegQueryDWordValue(HKLM, netfx11plus_reg + 'v4\Full' + lcid, 'Release', regVersion)) then begin
				if (regVersion >= 461308) then
					regVersion := 71 // 4.7.1+
				else if (regVersion >= 460798) then
					regVersion := 70 // 4.7+
				else if (regVersion >= 394802) then
					regVersion := 62 // 4.6.2+
				else if (regVersion >= 394254) then
					regVersion := 61 // 4.6.1+
				else if (regVersion >= 393295) then
					regVersion := 60 // 4.6+
				else if (regVersion >= 379893) then
					regVersion := 52 // 4.5.2+
				else if (regVersion >= 378675) then
					regVersion := 51 // 4.5.1+
				else if (regVersion >= 378389) then
					regVersion := 50 // 4.5+
				else
					regVersion := -1;
			end;
	end;
	Result := regVersion;
end;

[Setup]
