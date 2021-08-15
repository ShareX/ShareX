#define MyAppName "Recorder Devices for ShareX"
#define MyAppVersion "0.12.10"
#define MyAppRootDirectory "..\.."
#define MyAppOutputDirectory MyAppRootDirectory + "\Output"
#define MyAppLibDirectory MyAppRootDirectory + "\Lib"

[Setup]
AppName={#MyAppName}
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DirExistsWarning=no
DisableReadyPage=no
DisableReadyMemo=no
OutputBaseFilename=Recorder-devices-setup
OutputDir={#MyAppOutputDirectory}
ShowLanguageDialog=no

[Files]
Source: "{#MyAppLibDirectory}\screen-capture-recorder.dll"; DestDir: {app}; Flags: regserver 32bit; Check: not IsWin64
Source: "{#MyAppLibDirectory}\screen-capture-recorder-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsWin64
Source: "{#MyAppLibDirectory}\virtual-audio-capturer.dll"; DestDir: {app}; Flags: regserver 32bit; Check: not IsWin64
Source: "{#MyAppLibDirectory}\virtual-audio-capturer-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsWin64