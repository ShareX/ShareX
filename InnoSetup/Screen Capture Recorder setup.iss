#define AppName "Screen Capture Recorder"
#define AppVersion "0.12.8"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
Compression=lzma2/ultra64
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
DirExistsWarning=no
OutputBaseFilename={#AppName} setup
OutputDir=Output\
SolidCompression=true

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "..\Lib\screen-capture-recorder.dll"; DestDir: {app}; Flags: regserver 32bit; Check: IsAdminLoggedOn and not IsWin64
Source: "..\Lib\screen-capture-recorder-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsAdminLoggedOn and IsWin64
Source: "..\Lib\virtual-audio-capturer.dll"; DestDir: {app}; Flags: regserver 32bit; Check: IsAdminLoggedOn and not IsWin64
Source: "..\Lib\virtual-audio-capturer-x64.dll"; DestDir: {app}; Flags: regserver 64bit; Check: IsAdminLoggedOn and IsWin64

#include "Scripts\products.iss"
#include "Scripts\products\stringversion.iss"
#include "Scripts\products\winversion.iss"
#include "Scripts\products\fileversion.iss"
#include "Scripts\products\dotnetfxversion.iss"
#include "Scripts\products\msi31.iss"
#include "Scripts\products\vcredist2010.iss"

[Code]
function InitializeSetup(): Boolean;
begin
  initwinversion();
  msi31('3.1');
  vcredist2010();
  Result := true;
end;