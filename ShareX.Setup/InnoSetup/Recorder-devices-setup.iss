#define MyAppName "Recorder Devices for ShareX"
#define MyAppVersion "0.12.8"
#define MyAppRootDirectory "..\.."
#define MyAppOutputDirectory MyAppRootDirectory + "\Output"
#define MyAppLibDirectory MyAppRootDirectory + "\Lib"

[Setup]
AppName={#MyAppName}
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DirExistsWarning=no
OutputBaseFilename=Recorder-devices-setup
OutputDir={#MyAppOutputDirectory}
ShowLanguageDialog=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "{#MyAppLibDirectory}\screen-capture-recorder.dll"; DestDir: {app}; Flags: regserver 32bit; Check: not IsWin64
Source: "{#MyAppLibDirectory}\screen-capture-recorder-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsWin64
Source: "{#MyAppLibDirectory}\virtual-audio-capturer.dll"; DestDir: {app}; Flags: regserver 32bit; Check: not IsWin64
Source: "{#MyAppLibDirectory}\virtual-audio-capturer-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsWin64

[Code]
#include "Scripts\products.iss"
#include "Scripts\products\stringversion.iss"
#include "Scripts\products\winversion.iss"
#include "Scripts\products\fileversion.iss"
#include "Scripts\products\msi31.iss"
#include "Scripts\products\vcredist2010.iss"

function InitializeSetup(): Boolean;
begin
  initwinversion();
  msi31('3.1');
  vcredist2010();
  Result := true;
end;