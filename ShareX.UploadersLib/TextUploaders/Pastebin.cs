#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ShareX.UploadersLib.TextUploaders
{
    public class PastebinTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Pastebin;

        public override TextUploader CreateUploader(UploadersConfig uploadersConfig)
        {
            PastebinSettings settings = uploadersConfig.PastebinSettings;

            if (string.IsNullOrEmpty(settings.TextFormat))
            {
                settings.TextFormat = uploadersConfig.TextFormat;
            }

            return new Pastebin(APIKeys.PastebinKey, settings);
        }

        public override bool CheckConfig(UploadersConfig uploadersConfig) => true;
    }

    public sealed class Pastebin : TextUploader
    {
        private string APIKey;

        public PastebinSettings Settings { get; private set; }

        public Pastebin(string apiKey)
        {
            APIKey = apiKey;
            Settings = new PastebinSettings();
        }

        public Pastebin(string apiKey, PastebinSettings settings)
        {
            APIKey = apiKey;
            Settings = settings;
        }

        public bool Login()
        {
            if (!string.IsNullOrEmpty(Settings.Username) && !string.IsNullOrEmpty(Settings.Password))
            {
                Dictionary<string, string> loginArgs = new Dictionary<string, string>();

                loginArgs.Add("api_dev_key", APIKey);
                loginArgs.Add("api_user_name", Settings.Username);
                loginArgs.Add("api_user_password", Settings.Password);

                string loginResponse = SendRequest(HttpMethod.POST, "http://pastebin.com/api/api_login.php", loginArgs);

                if (!string.IsNullOrEmpty(loginResponse) && !loginResponse.StartsWith("Bad API request"))
                {
                    Settings.UserKey = loginResponse;
                    return true;
                }
            }

            Settings.UserKey = null;
            Errors.Add("Pastebin login failed.");
            return false;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text) && Settings != null)
            {
                Dictionary<string, string> args = new Dictionary<string, string>();

                args.Add("api_dev_key", APIKey); // which is your unique API Developers Key
                args.Add("api_option", "paste"); // set as 'paste', this will indicate you want to create a new paste
                args.Add("api_paste_code", text); // this is the text that will be written inside your paste

                // Optional args
                args.Add("api_paste_name", Settings.Title); // this will be the name / title of your paste
                args.Add("api_paste_format", Settings.TextFormat); // this will be the syntax highlighting value
                args.Add("api_paste_private", GetPrivacy(Settings.Exposure)); // this makes a paste public or private, public = 0, private = 1
                args.Add("api_paste_expire_date", GetExpiration(Settings.Expiration)); // this sets the expiration date of your paste

                if (!string.IsNullOrEmpty(Settings.UserKey))
                {
                    args.Add("api_user_key", Settings.UserKey); // this paramater is part of the login system
                }

                ur.Response = SendRequest(HttpMethod.POST, "http://pastebin.com/api/api_post.php", args);

                if (!string.IsNullOrEmpty(ur.Response) && !ur.Response.StartsWith("Bad API request") && ur.Response.IsValidUrl())
                {
                    if (Settings.RawURL)
                    {
                        string paste_key = URLHelpers.GetFileName(ur.Response);
                        ur.URL = "http://pastebin.com/raw/" + paste_key;
                    }
                    else
                    {
                        ur.URL = ur.Response;
                    }
                }
                else
                {
                    Errors.Add(ur.Response);
                }
            }

            return ur;
        }

        private string GetPrivacy(PastebinPrivacy privacy)
        {
            switch (privacy)
            {
                case PastebinPrivacy.Public:
                    return "0";
                default:
                case PastebinPrivacy.Unlisted:
                    return "1";
                case PastebinPrivacy.Private:
                    return "2";
            }
        }

        private string GetExpiration(PastebinExpiration expiration)
        {
            switch (expiration)
            {
                default:
                case PastebinExpiration.N:
                    return "N";
                case PastebinExpiration.M10:
                    return "10M";
                case PastebinExpiration.H1:
                    return "1H";
                case PastebinExpiration.D1:
                    return "1D";
                case PastebinExpiration.W1:
                    return "1W";
                case PastebinExpiration.W2:
                    return "2W";
                case PastebinExpiration.M1:
                    return "1M";
            }
        }

        public static List<PastebinSyntaxInfo> GetSyntaxList()
        {
            string syntaxList = @"4cs = 4CS
6502acme = 6502 ACME Cross Assembler
6502kickass = 6502 Kick Assembler
6502tasm = 6502 TASM/64TASS
abap = ABAP
actionscript = ActionScript
actionscript3 = ActionScript 3
ada = Ada
aimms = AIMMS
algol68 = ALGOL 68
apache = Apache Log
applescript = AppleScript
apt_sources = APT Sources
arm = ARM
asm = ASM (NASM)
asp = ASP
asymptote = Asymptote
autoconf = autoconf
autohotkey = Autohotkey
autoit = AutoIt
avisynth = Avisynth
awk = Awk
bascomavr = BASCOM AVR
bash = Bash
basic4gl = Basic4GL
dos = Batch
bibtex = BibTeX
blitzbasic = Blitz Basic
b3d = Blitz3D
bmx = BlitzMax
bnf = BNF
boo = BOO
bf = BrainFuck
c = C
c_winapi = C (WinAPI)
c_mac = C for Macs
cil = C Intermediate Language
csharp = C#
cpp = C++
cpp-winapi = C++ (WinAPI)
cpp-qt = C++ (with Qt extensions)
c_loadrunner = C: Loadrunner
caddcl = CAD DCL
cadlisp = CAD Lisp
cfdg = CFDG
chaiscript = ChaiScript
chapel = Chapel
clojure = Clojure
klonec = Clone C
klonecpp = Clone C++
cmake = CMake
cobol = COBOL
coffeescript = CoffeeScript
cfm = ColdFusion
css = CSS
cuesheet = Cuesheet
d = D
dart = Dart
dcl = DCL
dcpu16 = DCPU-16
dcs = DCS
delphi = Delphi
oxygene = Delphi Prism (Oxygene)
diff = Diff
div = DIV
dot = DOT
e = E
ezt = Easytrieve
ecmascript = ECMAScript
eiffel = Eiffel
email = Email
epc = EPC
erlang = Erlang
fsharp = F#
falcon = Falcon
fo = FO Language
f1 = Formula One
fortran = Fortran
freebasic = FreeBasic
freeswitch = FreeSWITCH
gambas = GAMBAS
gml = Game Maker
gdb = GDB
genero = Genero
genie = Genie
gettext = GetText
go = Go
groovy = Groovy
gwbasic = GwBasic
haskell = Haskell
haxe = Haxe
hicest = HicEst
hq9plus = HQ9 Plus
html4strict = HTML
html5 = HTML 5
icon = Icon
idl = IDL
ini = INI file
inno = Inno Script
intercal = INTERCAL
io = IO
ispfpanel = ISPF Panel Definition
j = J
java = Java
java5 = Java 5
javascript = JavaScript
jcl = JCL
jquery = jQuery
json = JSON
julia = Julia
kixtart = KiXtart
latex = Latex
ldif = LDIF
lb = Liberty BASIC
lsl2 = Linden Scripting
lisp = Lisp
llvm = LLVM
locobasic = Loco Basic
logtalk = Logtalk
lolcode = LOL Code
lotusformulas = Lotus Formulas
lotusscript = Lotus Script
lscript = LScript
lua = Lua
m68k = M68000 Assembler
magiksf = MagikSF
make = Make
mapbasic = MapBasic
matlab = MatLab
mirc = mIRC
mmix = MIX Assembler
modula2 = Modula 2
modula3 = Modula 3
68000devpac = Motorola 68000 HiSoft Dev
mpasm = MPASM
mxml = MXML
mysql = MySQL
nagios = Nagios
netrexx = NetRexx
newlisp = newLISP
nginx = Nginx
nimrod = Nimrod
nsis = NullSoft Installer
oberon2 = Oberon 2
objeck = Objeck Programming Langua
objc = Objective C
ocaml-brief = OCalm Brief
ocaml = OCaml
octave = Octave
pf = OpenBSD PACKET FILTER
glsl = OpenGL Shading
oobas = Openoffice BASIC
oracle11 = Oracle 11
oracle8 = Oracle 8
oz = Oz
parasail = ParaSail
parigp = PARI/GP
pascal = Pascal
pawn = Pawn
pcre = PCRE
per = Per
perl = Perl
perl6 = Perl 6
php = PHP
php-brief = PHP Brief
pic16 = Pic 16
pike = Pike
pixelbender = Pixel Bender
plsql = PL/SQL
postgresql = PostgreSQL
postscript = PostScript
povray = POV-Ray
powershell = Power Shell
powerbuilder = PowerBuilder
proftpd = ProFTPd
progress = Progress
prolog = Prolog
properties = Properties
providex = ProvideX
puppet = Puppet
purebasic = PureBasic
pycon = PyCon
python = Python
pys60 = Python for S60
q = q/kdb+
qbasic = QBasic
qml = QML
rsplus = R
racket = Racket
rails = Rails
rbs = RBScript
rebol = REBOL
reg = REG
rexx = Rexx
robots = Robots
rpmspec = RPM Spec
ruby = Ruby
gnuplot = Ruby Gnuplot
rust = Rust
sas = SAS
scala = Scala
scheme = Scheme
scilab = Scilab
scl = SCL
sdlbasic = SdlBasic
smalltalk = Smalltalk
smarty = Smarty
spark = SPARK
sparql = SPARQL
sqf = SQF
sql = SQL
standardml = StandardML
stonescript = StoneScript
sclang = SuperCollider
swift = Swift
systemverilog = SystemVerilog
tsql = T-SQL
tcl = TCL
teraterm = Tera Term
thinbasic = thinBasic
typoscript = TypoScript
unicon = Unicon
uscript = UnrealScript
ups = UPC
urbi = Urbi
vala = Vala
vbnet = VB.NET
vbscript = VBScript
vedit = Vedit
verilog = VeriLog
vhdl = VHDL
vim = VIM
visualprolog = Visual Pro Log
vb = VisualBasic
visualfoxpro = VisualFoxPro
whitespace = WhiteSpace
whois = WHOIS
winbatch = Winbatch
xbasic = XBasic
xml = XML
xorg_conf = Xorg Config
xpp = XPP
yaml = YAML
z80 = Z80 Assembler
zxbasic = ZXBasic";

            List<PastebinSyntaxInfo> result = new List<PastebinSyntaxInfo>();
            result.Add(new PastebinSyntaxInfo("None", "text"));

            foreach (string line in syntaxList.Lines().Select(x => x.Trim()))
            {
                int index = line.IndexOf('=');

                if (index > 0)
                {
                    PastebinSyntaxInfo syntaxInfo = new PastebinSyntaxInfo();
                    syntaxInfo.Value = line.Remove(index).Trim();
                    syntaxInfo.Name = line.Substring(index + 1).Trim();
                    result.Add(syntaxInfo);
                }
            }

            return result;
        }
    }

    public enum PastebinPrivacy
    {
        [Description("Public")]
        Public,
        [Description("Unlisted")]
        Unlisted,
        [Description("Private (members only)")]
        Private
    }

    public enum PastebinExpiration
    {
        [Description("Never")]
        N,
        [Description("10 Minutes")]
        M10,
        [Description("1 Hour")]
        H1,
        [Description("1 Day")]
        D1,
        [Description("1 Week")]
        W1,
        [Description("2 Weeks")]
        W2,
        [Description("1 Month")]
        M1
    }

    public class PastebinSyntaxInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public PastebinSyntaxInfo()
        {
        }

        public PastebinSyntaxInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class PastebinSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public PastebinPrivacy Exposure { get; set; } = PastebinPrivacy.Unlisted;
        public PastebinExpiration Expiration { get; set; } = PastebinExpiration.N;
        public string Title { get; set; }
        public string TextFormat { get; set; } = "text";
        public string UserKey { get; set; }
        public bool RawURL { get; set; }
    }
}