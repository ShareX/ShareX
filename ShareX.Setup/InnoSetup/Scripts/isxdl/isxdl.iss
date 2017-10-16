[Files]
Source: "scripts\isxdl\isxdl.dll"; Flags: dontcopy

[Code]
//replace PAnsiChar with PChar on non-unicode Inno Setup
procedure isxdl_AddFile(URL, Filename: PAnsiChar);
external 'isxdl_AddFile@files:isxdl.dll stdcall';

function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';

//replace PAnsiChar with PChar on non-unicode Inno Setup
function isxdl_SetOption(Option, Value: PAnsiChar): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';
