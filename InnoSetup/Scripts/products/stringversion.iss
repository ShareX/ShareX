function stringtoversion(var temp: String): Integer;
var
	part: String;
	pos1: Integer;

begin
	if (Length(temp) = 0) then begin
		Result := -1;
		Exit;
	end;

	pos1 := Pos('.', temp);
	if (pos1 = 0) then begin
		Result := StrToInt(temp);
		temp := '';
	end else begin
		part := Copy(temp, 1, pos1 - 1);
		temp := Copy(temp, pos1 + 1, Length(temp));
		Result := StrToInt(part);
	end;
end;

function compareinnerversion(var x, y: String): Integer;
var
	num1, num2: Integer;

begin
	num1 := stringtoversion(x);
	num2 := stringtoversion(y);
	if (num1 = -1) or (num2 = -1) then begin
		Result := 0;
		Exit;
	end;

	if (num1 < num2) then begin
		Result := -1;
	end else if (num1 > num2) then begin
		Result := 1;
	end else begin
		Result := compareinnerversion(x, y);
	end;
end;

function compareversion(versionA, versionB: String): Integer;
var
  temp1, temp2: String;

begin
    temp1 := versionA;
    temp2 := versionB;
    Result := compareinnerversion(temp1, temp2);
end;
