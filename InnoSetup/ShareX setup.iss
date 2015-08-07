#define AppName "ShareX"
#define AppFilename "ShareX.exe"
#define AppParentDirectory "..\ShareX\bin\Release"
#define AppPath AppParentDirectory + "\" + AppFilename
#dim Version[4]
#expr ParseVersion(AppPath, Version[0], Version[1], Version[2], Version[3])
#define AppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2])
#define AppPublisher "ShareX Developers"
#define AppId "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC"

[Setup]
AppCopyright=Copyright (C) 2007-2015 {#AppPublisher}
AppId={#AppId}
AppMutex={#AppId}
AppName={#AppName}
AppPublisher={#AppPublisher}
AppPublisherURL=https://getsharex.com
AppSupportURL=https://github.com/ShareX/ShareX/issues
AppUpdatesURL=https://github.com/ShareX/ShareX/releases
AppVerName={#AppName} {#AppVersion}
AppVersion={#AppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
CreateUninstallRegKey=not IsTaskSelected('PortableMode')
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
DirExistsWarning=no
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.txt
MinVersion=0,5.01.2600
OutputBaseFilename={#AppName}-{#AppVersion}-setup
OutputDir=Output\
PrivilegesRequired=none
ShowLanguageDialog=no
Uninstallable=not IsTaskSelected('PortableMode')
UninstallDisplayIcon={app}\{#AppFilename}
UninstallDisplayName={#AppName}
VersionInfoCompany={#AppPublisher}
VersionInfoTextVersion={#AppVersion}
VersionInfoVersion={#AppVersion}
WizardImageBackColor=clWhite
WizardImageFile=WizardImageFile.bmp
WizardImageStretch=no
WizardSmallImageFile=WizardSmallImageFile.bmp

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "PortableMode"; Description: "Portable mode"; Flags: unchecked
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateSendToIcon"; Description: "Create a send to shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateQuickLaunchIcon"; Description: "Create a quick launch shortcut"; GroupDescription: "Additional shortcuts:"; OnlyBelowVersion: 0,6.1
Name: "CreateStartupIcon"; Description: "Launch {#AppName} automatically at Windows startup"; GroupDescription: "Other tasks:"

[Files]
Source: "PersonalPath.cfg"; DestDir: {app}; Tasks: PortableMode

Source: "{#AppParentDirectory}\ShareX.exe"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\ShareX.exe.config"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\*.dll"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\*.css"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\*.txt"; DestDir: {app}; Flags: ignoreversion
Source: "Output\Recorder-devices-setup.exe"; DestDir: {app}; Flags: ignoreversion

Source: "{#AppParentDirectory}\tr\*.resources.dll"; DestDir: {app}\Languages\tr; Flags: ignoreversion
Source: "{#AppParentDirectory}\de\*.resources.dll"; DestDir: {app}\Languages\de; Flags: ignoreversion
Source: "{#AppParentDirectory}\fr\*.resources.dll"; DestDir: {app}\Languages\fr; Flags: ignoreversion
Source: "{#AppParentDirectory}\zh-CN\*.resources.dll"; DestDir: {app}\Languages\zh-CN; Flags: ignoreversion
Source: "{#AppParentDirectory}\hu\*.resources.dll"; DestDir: {app}\Languages\hu; Flags: ignoreversion
Source: "{#AppParentDirectory}\ko-KR\*.resources.dll"; DestDir: {app}\Languages\ko-KR; Flags: ignoreversion
Source: "{#AppParentDirectory}\es\*.resources.dll"; DestDir: {app}\Languages\es; Flags: ignoreversion
Source: "{#AppParentDirectory}\nl-NL\*.resources.dll"; DestDir: {app}\Languages\nl-NL; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: not PortableMode
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"; WorkingDir: "{app}"; Tasks: not PortableMode
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon and not PortableMode; Check: not DesktopIconExists
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: CreateQuickLaunchIcon and not PortableMode
Name: "{sendto}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: CreateSendToIcon and not PortableMode
Name: "{userstartup}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Parameters: "-silent"; Tasks: CreateStartupIcon and not PortableMode

[Run]
Filename: "{app}\{#AppFilename}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall

[UninstallRun]
Filename: regsvr32; WorkingDir: {app}; Parameters: "/s /u screen-capture-recorder.dll"; Check: not IsWin64
Filename: regsvr32; WorkingDir: {app}; Parameters: "/s /u screen-capture-recorder-x64.dll"; Check: IsWin64
Filename: regsvr32; WorkingDir: {app}; Parameters: "/s /u audio_sniffer.dll"; Check: not IsWin64
Filename: regsvr32; WorkingDir: {app}; Parameters: "/s /u audio_sniffer-x64.dll"; Check: IsWin64

[Registry]
;Root: "HKCU"; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueName: "{#AppName}"; Flags: uninsdeletevalue; Tasks: not PortableMode
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#AppName}"; Flags: dontcreatekey uninsdeletekey; Tasks: not PortableMode
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#AppName}"; Flags: dontcreatekey uninsdeletekey; Tasks: not PortableMode
Root: "HKCU"; Subkey: "Software\Classes\Folder\shell\{#AppName}"; Flags: dontcreatekey uninsdeletekey; Tasks: not PortableMode

[Code]
#include "Scripts\products.iss"
#include "Scripts\products\stringversion.iss"
#include "Scripts\products\winversion.iss"
#include "Scripts\products\fileversion.iss"
#include "Scripts\products\dotnetfxversion.iss"
#include "Scripts\products\msi31.iss"
#include "Scripts\products\dotnetfx40full.iss"

procedure InitializeWizard;
begin
  WizardForm.LicenseAcceptedRadio.Checked := true;
end;

function InitializeSetup(): Boolean;
begin
  initwinversion();
  msi31('3.1');
  dotnetfx40full();
  Result := true;
end;

function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{userdesktop}\{#AppName}.lnk'));
end;