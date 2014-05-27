[CustomMessages]
iis_title=Internet Information Services (IIS)


[Code]
function iis(): boolean;
begin
	if (not RegKeyExists(HKLM, 'SYSTEM\CurrentControlSet\Services\W3SVC\Security')) then
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('iis_title')]), mbError, MB_OK)
	else
		Result := true;
end;