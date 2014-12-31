#define MyAppName "ShareX"
#define MyAppFile "ShareX.exe"
#define MyAppParentDir "..\ShareX\bin\Release"
#define MyAppPath MyAppParentDir + "\ShareX.exe"
#dim Version[4]
#expr ParseVersion(MyAppPath, Version[0], Version[1], Version[2], Version[3])
#define MyAppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2])
#define MyAppPublisher "ShareX Developers"
#define MyAppId "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC"

[Setup]
AllowNoIcons=true
AppCopyright=Copyright © 2007-2015 {#MyAppPublisher}
AppId={#MyAppId}
AppMutex={#MyAppId}
AppName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL=http://getsharex.com
AppSupportURL=https://github.com/ShareX/ShareX/issues
AppUpdatesURL=https://github.com/ShareX/ShareX/releases
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
Compression=lzma/ultra64
CreateAppDir=true
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DirExistsWarning=no
InternalCompressLevel=ultra64
LanguageDetectionMethod=uilanguage
LicenseFile=..\LICENSE.txt
MinVersion=0,5.01.2600
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-setup
OutputDir=Output\
PrivilegesRequired=none
ShowLanguageDialog=auto
ShowUndisplayableLanguages=false
SignedUninstaller=false
SolidCompression=true
Uninstallable=true
UninstallDisplayIcon={app}\{#MyAppFile}
UsePreviousAppDir=yes
UsePreviousGroup=yes
VersionInfoCompany={#MyAppPublisher}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
WizardImageFile=WizardImageFile.bmp
WizardSmallImageFile=WizardSmallImageFile.bmp
WizardImageBackColor=clWhite
WizardImageStretch=False

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateQuickLaunchIcon"; Description: "Create a quick launch shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateSendToIcon"; Description: "Create a send to shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateStartupIcon"; Description: "Launch {#MyAppName} automatically at Windows startup"; GroupDescription: "Other tasks:"

[Files]
Source: "{#MyAppParentDir}\ShareX.exe"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppParentDir}\ShareX.exe.config"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppParentDir}\*.dll"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppParentDir}\*.css"; DestDir: {app}; Flags: ignoreversion
Source: "{#MyAppParentDir}\*.txt"; DestDir: {app}; Flags: ignoreversion

; Language resources
Source: "{#MyAppParentDir}\tr\*.resources.dll"; DestDir: {app}\Languages\tr; Flags: ignoreversion
;Source: "{#MyAppParentDir}\de\*.resources.dll"; DestDir: {app}\Languages\de; Flags: ignoreversion

; Required for screen/audio recording
Source: "..\Lib\screen-capture-recorder.dll"; DestDir: {app}; Flags: regserver 32bit; Check: IsAdminLoggedOn and not IsWin64
Source: "..\Lib\screen-capture-recorder-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsAdminLoggedOn and IsWin64
Source: "..\Lib\audio_sniffer.dll"; DestDir: {app}; Flags: regserver 32bit; Check: IsAdminLoggedOn and not IsWin64
Source: "..\Lib\audio_sniffer-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsAdminLoggedOn and IsWin64

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"; WorkingDir: "{app}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon; Check: not DesktopIconExists
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Tasks: CreateQuickLaunchIcon
Name: "{sendto}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Tasks: CreateSendToIcon
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Parameters: "-silent"; Tasks: CreateStartupIcon

[Run]
Filename: "{app}\{#MyAppFile}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall

[Registry]
;Root: "HKCU"; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueName: "{#MyAppName}"; Flags: uninsdeletevalue
Root: "HKCU"; Subkey: "Software\Classes\*\shell\{#MyAppName}"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\{#MyAppName}"; Flags: dontcreatekey uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Folder\shell\{#MyAppName}"; Flags: dontcreatekey uninsdeletekey

#include "Scripts\products.iss"
#include "Scripts\products\stringversion.iss"
#include "Scripts\products\winversion.iss"
#include "Scripts\products\fileversion.iss"
#include "Scripts\products\dotnetfxversion.iss"
#include "Scripts\products\msi31.iss"
#include "Scripts\products\dotnetfx40full.iss"
#include "Scripts\products\vcredist2010.iss"

[Code]
procedure InitializeWizard;
begin
  WizardForm.LicenseAcceptedRadio.Checked := true;
end;

function InitializeSetup(): Boolean;
begin
  initwinversion();

  msi31('3.1');
  dotnetfx40full();
  vcredist2010();

  Result := true;
end;

function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{userdesktop}\{#MyAppName}.lnk'));
end;