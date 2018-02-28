{
	--- TYPES AND VARIABLES ---
}
type
	TProduct = record
		File: String;
		Title: String;
		Parameters: String;
		ForceSuccess : boolean;
		InstallClean : boolean;
		MustRebootAfter : boolean;
	end;

	InstallResult = (InstallSuccessful, InstallRebootRequired, InstallError);

var
	installMemo, downloadMessage: string;
	products: array of TProduct;
	delayedReboot, isForcedX86: boolean;
	DependencyPage: TOutputProgressWizardPage;

procedure AddProduct(filename, parameters, title, size, url: string; forceSuccess, installClean, mustRebootAfter : boolean);
{
	Adds a product to the list of products to download.
	Parameters:
		filename: the file name under which to save the file
		parameters: the parameters with which to run the file
		title: the product title
		size: the file size
		url: the URL to download from
		forceSuccess: whether to continue in case of setup failure
		installClean: whether the product needs a reboot before installing
		mustRebootAfter: whether the product needs a reboot after installing
}
var
	path: string;
	i: Integer;
begin
	installMemo := installMemo + '%1' + title + #13;

	path := ExpandConstant('{src}{\}') + CustomMessage('DependenciesDir') + '\' + filename;
	if not FileExists(path) then begin
		path := ExpandConstant('{tmp}{\}') + filename;

		if not FileExists(path) then begin
			isxdl_AddFile(url, path);

			downloadMessage := downloadMessage + '%1' + title + ' (' + size + ')' + #13;
		end;
	end;

	i := GetArrayLength(products);
	SetArrayLength(products, i + 1);
	products[i].File := path;
	products[i].Title := title;
	products[i].Parameters := parameters;
	products[i].ForceSuccess := forceSuccess;
	products[i].InstallClean := installClean;
	products[i].MustRebootAfter := mustRebootAfter;
end;

function SmartExec(product : TProduct; var resultcode : Integer): boolean;
{
	Executes a product and returns the exit code.
	Parameters:
		product: the product to install
		resultcode: the exit code
}
begin
	if (LowerCase(Copy(product.File, Length(product.File) - 2, 3)) = 'exe') then begin
		Result := Exec(product.File, product.Parameters, '', SW_SHOWNORMAL, ewWaitUntilTerminated, resultcode);
	end else begin
		Result := ShellExec('', product.File, product.Parameters, '', SW_SHOWNORMAL, ewWaitUntilTerminated, resultcode);
	end;
end;

function PendingReboot: boolean;
{
	Checks whether the machine has a pending reboot.
}
var	names: String;
begin
	if (RegQueryMultiStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager', 'PendingFileRenameOperations', names)) then begin
		Result := true;
	end else if ((RegQueryMultiStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager', 'SetupExecute', names)) and (names <> ''))  then begin
		Result := true;
	end else begin
		Result := false;
	end;
end;

function InstallProducts: InstallResult;
{
	Installs the downloaded products
}
var
	resultCode, i, productCount, finishCount: Integer;
begin
	Result := InstallSuccessful;
	productCount := GetArrayLength(products);

	if productCount > 0 then begin
		DependencyPage := CreateOutputProgressPage(CustomMessage('depinstall_title'), CustomMessage('depinstall_description'));
		DependencyPage.Show;

		for i := 0 to productCount - 1 do begin
			if (products[i].InstallClean and (delayedReboot or PendingReboot())) then begin
				Result := InstallRebootRequired;
				break;
			end;

			DependencyPage.SetText(FmtMessage(CustomMessage('depinstall_status'), [products[i].Title]), '');
			DependencyPage.SetProgress(i, productCount);

			while true do begin
				// set 0 as used code for shown error if SmartExec fails
				resultCode := 0;
				if SmartExec(products[i], resultCode) then begin
					// setup executed; resultCode contains the exit code
					if (products[i].MustRebootAfter) then begin
						// delay reboot after install if we installed the last dependency anyways
						if (i = productCount - 1) then begin
							delayedReboot := true;
						end else begin
							Result := InstallRebootRequired;
						end;
						break;
					end else if (resultCode = 0) or (products[i].ForceSuccess) then begin
						finishCount := finishCount + 1;
						break;
					end else if (resultCode = 3010) then begin
						// Windows Installer resultCode 3010: ERROR_SUCCESS_REBOOT_REQUIRED
						delayedReboot := true;
						finishCount := finishCount + 1;
						break;
					end;
				end;

				case MsgBox(FmtMessage(SetupMessage(msgErrorFunctionFailed), [products[i].Title, IntToStr(resultCode)]), mbError, MB_ABORTRETRYIGNORE) of
					IDABORT: begin
						Result := InstallError;
						break;
					end;
					IDIGNORE: begin
						break;
					end;
				end;
			end;

			if Result <> InstallSuccessful then begin
				break;
			end;
		end;

		// only leave not installed products for error message
		for i := 0 to productCount - finishCount - 1 do begin
			products[i] := products[i+finishCount];
		end;
		SetArrayLength(products, productCount - finishCount);

		DependencyPage.Hide;
	end;
end;

{
	--------------------
	INNO EVENT FUNCTIONS
	--------------------
}

function PrepareToInstall(var NeedsRestart: boolean): String;
{
	Before the "preparing to install" page.
	See: http://www.jrsoftware.org/ishelp/index.php?topic=scriptevents
}
var
	i: Integer;
	s: string;
begin
	delayedReboot := false;

	case InstallProducts() of
		InstallError: begin
			s := CustomMessage('depinstall_error');

			for i := 0 to GetArrayLength(products) - 1 do begin
				s := s + #13 + '	' + products[i].Title;
			end;

			Result := s;
			end;
		InstallRebootRequired: begin
			Result := products[0].Title;
			NeedsRestart := true;

			// write into the registry that the installer needs to be executed again after restart
			RegWriteStringValue(HKEY_CURRENT_USER, 'SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce', 'InstallBootstrap', ExpandConstant('{srcexe}'));
			end;
	end;
end;

function NeedRestart : boolean;
{
	Checks whether a restart is needed at the end of install
	See: http://www.jrsoftware.org/ishelp/index.php?topic=scriptevents
}
begin
	Result := delayedReboot;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
{
	Just before the "ready" page.
	See: http://www.jrsoftware.org/ishelp/index.php?topic=scriptevents
}
var
	s: string;
begin
	if downloadMessage <> '' then
		s := s + CustomMessage('depdownload_memo_title') + ':' + NewLine + FmtMessage(downloadMessage, [Space]) + NewLine;
	if installMemo <> '' then
		s := s + CustomMessage('depinstall_memo_title') + ':' + NewLine + FmtMessage(installMemo, [Space]) + NewLine;

	if MemoDirInfo <> '' then
		s := s + MemoDirInfo + NewLine + NewLine;
	if MemoGroupInfo <> '' then
		s := s + MemoGroupInfo + NewLine + NewLine;
	if MemoTasksInfo <> '' then
		s := s + MemoTasksInfo;

	Result := s
end;

function NextButtonClick(CurPageID: Integer): boolean;
{
	At each "next" button click
	See: http://www.jrsoftware.org/ishelp/index.php?topic=scriptevents
}
begin
	Result := true;

	if CurPageID = wpReady then begin
		if downloadMessage <> '' then begin
			// change isxdl language only if it is not english because isxdl default language is already english
			if (ActiveLanguage() <> 'en') then begin
				ExtractTemporaryFile(CustomMessage('isxdl_langfile'));
				isxdl_SetOption('language', ExpandConstant('{tmp}{\}') + CustomMessage('isxdl_langfile'));
			end;
			//isxdl_SetOption('title', FmtMessage(SetupMessage(msgSetupWindowTitle), [CustomMessage('appname')]));

			//if SuppressibleMsgBox(FmtMessage(CustomMessage('depdownload_msg'), [FmtMessage(downloadMessage, [''])]), mbConfirmation, MB_YESNO, IDYES) = IDNO then
			//	Result := false
			//else if
			if isxdl_DownloadFiles(StrToInt(ExpandConstant('{wizardhwnd}'))) = 0 then
				Result := false;
		end;
	end;
end;

{
	-----------------------------
	ARCHITECTURE HELPER FUNCTIONS
	-----------------------------
}

function IsX86: boolean;
{
	Gets whether the computer is x86 (32 bits).
}
begin
	Result := isForcedX86 or (ProcessorArchitecture = paX86) or (ProcessorArchitecture = paUnknown);
end;

function IsX64: boolean;
{
	Gets whether the computer is x64 (64 bits).
}
begin
	Result := (not isForcedX86) and Is64BitInstallMode and (ProcessorArchitecture = paX64);
end;

function IsIA64: boolean;
{
	Gets whether the computer is IA64 (Itanium 64 bits).
}
begin
	Result := (not isForcedX86) and Is64BitInstallMode and (ProcessorArchitecture = paIA64);
end;

function GetString(x86, x64, ia64: String): String;
{
	Gets a string depending on the computer architecture.
	Parameters:
		x86: the string if the computer is x86
		x64: the string if the computer is x64
		ia64: the string if the computer is IA64
}
begin
	if IsX64() and (x64 <> '') then begin
		Result := x64;
	end else if IsIA64() and (ia64 <> '') then begin
		Result := ia64;
	end else begin
		Result := x86;
	end;
end;

function GetArchitectureString(): String;
{
	Gets the "standard" architecture suffix string.
	Returns either _x64, _ia64 or nothing.
}
begin
	if IsX64() then begin
		Result := '_x64';
	end else if IsIA64() then begin
		Result := '_ia64';
	end else begin
		Result := '';
	end;
end;

procedure SetForceX86(value: boolean);
{
	Forces the setup to use X86 products
}
begin
	isForcedX86 := value;
end;
