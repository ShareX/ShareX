#define MyAppName "ShareX"
#define MyAppRootDirectory "..\.."
#define MyAppOutputDirectory MyAppRootDirectory + "\Output"
#define MyAppDependenciesDirectory MyAppOutputDirectory + "\Dependencies"
#define MyAppReleaseDirectory MyAppRootDirectory + "\" + MyAppName + "\bin\Release\win-arm64\publish"
#define MyAppFileName MyAppName + ".exe"
#define MyAppFilePath MyAppReleaseDirectory + "\" + MyAppFileName
#define MyAppVersion GetStringFileInfo(MyAppFilePath, "ProductVersion")
#define MyAppPublisher "ShareX Team"
#define MyAppURL "https://getsharex.com"
#define MyAppId "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC"

[Setup]
AppCopyright=Copyright (c) 2007-2025 ShareX Team
AppId={#MyAppId}
AppMutex={#MyAppId}
AppName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x64os arm64 x86
ArchitecturesInstallIn64BitMode=x64os
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile={#MyAppRootDirectory}\LICENSE.txt
MinVersion=6.1sp1
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-setup-arm64
OutputDir={#MyAppOutputDirectory}
PrivilegesRequired=none
SolidCompression=yes
UninstallDisplayIcon={app}\{#MyAppFileName}
UninstallDisplayName={#MyAppName}
VersionInfoCompany={#MyAppPublisher}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}

[Tasks]
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Check: not IsUpdating and not DesktopIconExists
Name: "CreateContextMenuButton"; Description: "Show ""Upload with ShareX"" button in Windows Explorer context menu"; GroupDescription: "Additional shortcuts:"; Check: not IsUpdating
Name: "CreateSendToIcon"; Description: "Create a send to shortcut"; GroupDescription: "Additional shortcuts:"; Check: not IsUpdating
Name: "CreateStartupIcon"; Description: "Run ShareX when Windows starts"; GroupDescription: "Other tasks:"; Check: not IsUpdating
Name: "EnableBrowserExtensionSupport"; Description: "Enable browser extension support"; GroupDescription: "Other tasks:"; Check: not IsUpdating
Name: "DisablePrintScreenKeyForSnippingTool"; Description: "Disable Print Screen key for Snipping Tool"; GroupDescription: "Other tasks:"; Check: not IsUpdating

[Files]
Source: "{#MyAppReleaseDirectory}\*.exe"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\*.dll"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\*.json"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppRootDirectory}\Licenses\*.txt"; DestDir: {app}\Licenses; Flags: ignoreversion
Source: "{#MyAppDependenciesDirectory}\*.exe"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppDependenciesDirectory}\exiftool_files\*"; DestDir: {app}\exiftool_files; Flags: ignoreversion recursesubdirs
Source: "{#MyAppReleaseDirectory}\ShareX_File_Icon.ico"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\ar-YE\*.resources.dll"; DestDir: {app}\ar-YE; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\de\*.resources.dll"; DestDir: {app}\de; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\es\*.resources.dll"; DestDir: {app}\es; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\es-MX\*.resources.dll"; DestDir: {app}\es-MX; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\fa-IR\*.resources.dll"; DestDir: {app}\fa-IR; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\fr\*.resources.dll"; DestDir: {app}\fr; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\he-IL\*.resources.dll"; DestDir: {app}\he-IL; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\hu\*.resources.dll"; DestDir: {app}\hu; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\id-ID\*.resources.dll"; DestDir: {app}\id-ID; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\it-IT\*.resources.dll"; DestDir: {app}\it-IT; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\ja-JP\*.resources.dll"; DestDir: {app}\ja-JP; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\ko-KR\*.resources.dll"; DestDir: {app}\ko-KR; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\nl-NL\*.resources.dll"; DestDir: {app}\nl-NL; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\pl\*.resources.dll"; DestDir: {app}\pl; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\pt-BR\*.resources.dll"; DestDir: {app}\pt-BR; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\pt-PT\*.resources.dll"; DestDir: {app}\pt-PT; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\ro\*.resources.dll"; DestDir: {app}\ro; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\ru\*.resources.dll"; DestDir: {app}\ru; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\tr\*.resources.dll"; DestDir: {app}\tr; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\uk\*.resources.dll"; DestDir: {app}\uk; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\vi-VN\*.resources.dll"; DestDir: {app}\vi-VN; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\zh-CN\*.resources.dll"; DestDir: {app}\zh-CN; Flags: ignoreversion
Source: "{#MyAppReleaseDirectory}\zh-TW\*.resources.dll"; DestDir: {app}\zh-TW; Flags: ignoreversion
Source: "{#MyAppRootDirectory}\ShareX.ScreenCaptureLib\Stickers\*"; DestDir: {app}\Stickers; Flags: ignoreversion recursesubdirs
Source: "puush"; DestDir: {app}; Check: IsPuushMode

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppFileName}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"; WorkingDir: "{app}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppFileName}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon
Name: "{usersendto}\{#MyAppName}"; Filename: "{app}\{#MyAppFileName}"; WorkingDir: "{app}"; Tasks: CreateSendToIcon
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppFileName}"; WorkingDir: "{app}"; Parameters: "-silent"; Tasks: CreateStartupIcon

[Run]
Filename: "{app}\{#MyAppFileName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall; Check: not IsNoRun

[Registry]
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#MyAppName}"; ValueType: string; ValueData: "Upload with {#MyAppName}"; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#MyAppName}"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#MyAppFileName}"",0"; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#MyAppName}\command"; ValueType: string; ValueData: """{app}\{#MyAppFileName}"" ""%1"""; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#MyAppName}"; ValueType: string; ValueData: "Upload with {#MyAppName}"; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#MyAppName}"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#MyAppFileName}"",0"; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#MyAppName}\command"; ValueType: string; ValueData: """{app}\{#MyAppFileName}"" ""%1"""; Tasks: CreateContextMenuButton
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#MyAppName}"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#MyAppName}"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\.sxcu"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\ShareX.sxcu"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\.sxie"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\ShareX.sxie"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\SystemFileAssociations\image\shell\ShareXImageEditor"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "SOFTWARE\Google\Chrome\NativeMessagingHosts\com.getsharex.sharex"; ValueType: string; ValueData: "{app}\host-manifest-chrome.json"; Flags: uninsdeletekey; Tasks: EnableBrowserExtensionSupport
Root: "HKCU"; Subkey: "SOFTWARE\Mozilla\NativeMessagingHosts\ShareX"; ValueType: string; ValueData: "{app}\host-manifest-firefox.json"; Flags: uninsdeletekey; Tasks: EnableBrowserExtensionSupport
Root: "HKCU"; Subkey: "Control Panel\Keyboard"; ValueType: dword; ValueName: "PrintScreenKeyForSnippingEnabled"; ValueData: "0"; Flags: uninsdeletevalue; Tasks: DisablePrintScreenKeyForSnippingTool

[Code]
procedure InitializeWizard;
begin
  if not IsAdmin then
  begin
    WizardForm.DirEdit.Text := ExpandConstant('{userpf}\{#MyAppName}');
  end;
end;

function InitializeUninstall(): Boolean;
var
  ErrorCode: Integer;
begin
  if CheckForMutexes('{#MyAppId}') then
  begin
    if MsgBox('Uninstall has detected that {#MyAppName} is currently running.' + #13#10#13#10 + 'Would you like to close it?', mbError, MB_YESNO) = IDYES then
    begin
      Exec('taskkill.exe', '/f /im {#MyAppFileName}', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
    end
    else
    begin
      Result := False;
      Exit;
    end;
  end;

  Result := True;
end;

function CmdLineParamExists(const value: string): Boolean;
var
  i: Integer;
begin
  Result := False;
  for i := 1 to ParamCount do
    if CompareText(ParamStr(i), value) = 0 then
    begin
      Result := True;
      Exit;
    end;
end;

function IsUpdating(): Boolean;
begin
  Result := CmdLineParamExists('/UPDATE');
end;

function IsNoRun(): Boolean;
begin
  Result := CmdLineParamExists('/NORUN');
end;

function IsPuushMode(): Boolean;
begin
  Result := CmdLineParamExists('-puush');
end;

function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{userdesktop}\{#MyAppName}.lnk'));
end;