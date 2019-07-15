[Code]
#ifdef UNICODE
	#define AW "W"
#else
	#define AW "A"
#endif

type
	INSTALLSTATE = Longint;
const
	INSTALLSTATE_INVALIDARG = -2;	// An invalid parameter was passed to the function.
	INSTALLSTATE_UNKNOWN = -1;		// The product is neither advertised or installed.
	INSTALLSTATE_ADVERTISED = 1;	// The product is advertised but not installed.
	INSTALLSTATE_ABSENT = 2;		// The product is installed for a different user.
	INSTALLSTATE_DEFAULT = 5;		// The product is installed for the current user.

function MsiQueryProductState(szProduct: string): INSTALLSTATE;
external 'MsiQueryProductState{#AW}@msi.dll stdcall';

function MsiEnumRelatedProducts(szUpgradeCode: string; nReserved: dword; nIndex: dword; szProductCode: string): integer;
external 'MsiEnumRelatedProducts{#AW}@msi.dll stdcall';

function MsiGetProductInfo(szProductCode: string; szProperty: string; szValue: string; var nvalueSize: dword): integer;
external 'MsiGetProductInfo{#AW}@msi.dll stdcall';

function msiproduct(productID: string): boolean;
begin
	Result := MsiQueryProductState(productID) = INSTALLSTATE_DEFAULT;
end;

function msiproductupgrade(upgradeCode: string; minVersion: string): boolean;
var
	productCode, version: string;
	valueSize: dword;
begin
	SetLength(productCode, 39);
	Result := false;

	if (MsiEnumRelatedProducts(upgradeCode, 0, 0, productCode) = 0) then begin
		SetLength(version, 39);
		valueSize := Length(version);

		if (MsiGetProductInfo(productCode, 'VersionString', version, valueSize) = 0) then begin
			Result := compareversion(version, minVersion) >= 0;
		end;
	end;
end;

[Setup]
