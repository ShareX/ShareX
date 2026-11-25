#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ShareX.Tools;

// A piece of a text that holds sensitive data.
public readonly record struct SensitiveDataMatch(SensitiveDataType Type, int Index, int Length)
{
    public int End => Index + Length;
}

// Finds sensitive data in one line of recognized text. The patterns are written for text read from a screenshot: a value
// is only censored when it passes the checks of its kind (check digits, card checksum, address ranges...), so ordinary
// numbers are left alone, and a value after a password label is always censored, since nothing else can tell it apart.
public sealed class SensitiveDataDetector
{
    // Only this group is censored when a pattern has it, such as the value after a password label.
    public const string CensorGroupName = "censor";

    private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(250);
    private const RegexOptions DefaultOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

    // Labels that are clearly about a password, so a few words may come between them and the colon: "Senha do Wi-Fi: ...".
    private static readonly Regex PasswordLabelRegex = Create(
        @"(?<![\p{L}\p{N}])(?:senhas?|passwords?|passwort|contrase[ñn]as?|mot de passe|passphrase|passwd|passcode)[^:=：\r\n]{0,24}?[:=：][ \t]*(?<censor>\S+)",
        RegexOptions.IgnoreCase);

    // Labels that are only a secret when the value comes right after them: "token: ...", "API_KEY=...", "PIN: ...".
    private static readonly Regex SecretLabelRegex = Create(
        @"(?<![\p{L}\p{N}])(?:pwd|pass|pin|token|secret|segredo|api[_ .-]?(?:key|secret|token)|apikey|private[_ .-]?key|client[_ .-]?secret|" +
        @"access[_ .-]?(?:key|token)|refresh[_ .-]?token|auth(?:[_ .-]?token)?|authorization|bearer|credentials?|credenciais|credencial|" +
        @"session(?:[_ .-]?id)?|cookie|chave(?:[_ .-]?(?:de[_ .-]?)?(?:api|acesso|secreta|privada|pix))?)[""']?[ \t]*[:=：][ \t]*" +
        @"(?:(?:bearer|basic|token)[ \t]+)?(?<censor>\S+)",
        RegexOptions.IgnoreCase);

    // The password in a link such as "https://user:password@host".
    private static readonly Regex URLPasswordRegex = Create(@"\b[a-z][a-z0-9+.-]*://[^\s:/@]+:(?<censor>[^\s@/]+)@", RegexOptions.IgnoreCase);

    // Keys and tokens with a known prefix or shape.
    private static readonly Regex KnownKeyRegex = Create(
        @"\b(?:AKIA|ASIA|AGPA|AIDA|AROA|ANPA|ANVA|AIPA)[0-9A-Z]{16}\b|" + // AWS access key ID
        @"\bgh[pousr]_[A-Za-z0-9]{30,}|\bgithub_pat_[A-Za-z0-9_]{30,}|" + // GitHub
        @"\bglpat-[A-Za-z0-9_-]{20,}|" + // GitLab
        @"\bxox[abposr]-[A-Za-z0-9-]{10,}|" + // Slack
        @"\bAIza[0-9A-Za-z_-]{30,}|" + // Google
        @"\b[spr]k_(?:live|test)_[0-9A-Za-z]{10,}|" + // Stripe
        @"\bsk-[A-Za-z0-9_-]{20,}|" + // OpenAI, Anthropic and others
        @"\b(?:hf|npm)_[A-Za-z0-9]{30,}|" + // Hugging Face, npm
        @"\beyJ[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}"); // JSON Web Token

    // Long runs of letters, digits and base64 symbols. Only the ones that look random are kept, see IsRandomLooking.
    // An equals sign is only taken as base64 padding at the end, so "NAME=value" keeps its label.
    private static readonly Regex RandomStringRegex = Create(@"(?<![A-Za-z0-9_+/=-])[A-Za-z0-9_+/-]{24,}={0,2}(?![A-Za-z0-9_+/=-])");

    private static readonly Regex IPv4Regex = Create(
        @"(?<![\d.])(?:(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(?!\d|\.\d)");

    // OCR often drops the space before an address ("IPv62001:db8::1"), so a letter or digit may come right before it.
    private static readonly Regex IPv6Regex = Create(@"(?<!:)(?:[0-9A-Fa-f]{0,4}:){2,7}[0-9A-Fa-f]{0,4}(?![0-9A-Fa-f:])");

    private static readonly Regex EmailRegex = Create(@"(?<![\w.%+-])[\w.%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}(?![\w-])");

    private static readonly Regex CreditCardRegex = Create(@"(?<![\d-])(?:\d[ -]?){12,18}\d(?![\d-])");

    private static readonly Regex CPFRegex = Create(@"(?<!\d|\d[.\-/])\d{3}[. ]?\d{3}[. ]?\d{3}[-–. ]?\d{2}(?!\d|[.\-/]\d)");

    // Since July 2026 the first twelve characters of a CNPJ can be letters. Those are only matched with their slash,
    // since fourteen letters and digits in a row are usually something else.
    private static readonly Regex CNPJRegex = Create(
        @"(?<![\p{L}\p{N}]|[\p{L}\p{N}][.\-/])(?:[0-9A-Z]{2}\.?[0-9A-Z]{3}\.?[0-9A-Z]{3}/[0-9A-Z]{4}[-–]?\d{2}|\d{14})(?![\p{L}\p{N}]|[.\-/][\p{L}\p{N}])");

    private static readonly Regex PhoneRegex = Create(
        @"(?<![\w+])(?:\+\d{1,3}[ .-]?)?(?:\(\d{2,3}\)|\d{2,3})[ .-]?\d{4,5}[ .-]?\d{4}(?!\d)");

    private readonly List<Rule> _rules = [];

    // Custom patterns that are not valid regular expressions, with the reason, so they can be reported.
    public IReadOnlyList<(string Pattern, string Error)> InvalidPatterns { get; }

    public SensitiveDataDetector(SensitiveDataOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.CensorPasswords)
        {
            _rules.Add(new Rule(SensitiveDataType.Password, PasswordLabelRegex));
            _rules.Add(new Rule(SensitiveDataType.Password, SecretLabelRegex));
            _rules.Add(new Rule(SensitiveDataType.Password, URLPasswordRegex));
        }

        if (options.CensorAPIKeys)
        {
            _rules.Add(new Rule(SensitiveDataType.APIKey, KnownKeyRegex));
            _rules.Add(new Rule(SensitiveDataType.APIKey, RandomStringRegex, IsRandomLooking));
        }

        if (options.CensorIPAddresses)
        {
            _rules.Add(new Rule(SensitiveDataType.IPAddress, IPv4Regex, IsPublicOrPrivateIPv4, UseDigitText: true));
            _rules.Add(new Rule(SensitiveDataType.IPAddress, IPv6Regex, IsIPv6));
        }

        if (options.CensorEmailAddresses)
        {
            _rules.Add(new Rule(SensitiveDataType.EmailAddress, EmailRegex));
        }

        if (options.CensorCreditCardNumbers)
        {
            _rules.Add(new Rule(SensitiveDataType.CreditCardNumber, CreditCardRegex, IsCreditCardNumber, UseDigitText: true));
        }

        if (options.CensorCPF)
        {
            _rules.Add(new Rule(SensitiveDataType.CPF, CPFRegex, IsCPF, UseDigitText: true));
        }

        if (options.CensorCNPJ)
        {
            _rules.Add(new Rule(SensitiveDataType.CNPJ, CNPJRegex, IsCNPJ));
            _rules.Add(new Rule(SensitiveDataType.CNPJ, CNPJRegex, IsCNPJ, UseDigitText: true));
        }

        if (options.CensorPhoneNumbers)
        {
            _rules.Add(new Rule(SensitiveDataType.PhoneNumber, PhoneRegex, IsPhoneNumber, UseDigitText: true));
        }

        List<(string, string)> invalidPatterns = [];

        foreach (string pattern in options.CustomPatterns ?? [])
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                continue;
            }

            try
            {
                _rules.Add(new Rule(SensitiveDataType.Custom, Create(pattern, RegexOptions.IgnoreCase, compiled: false)));
            }
            catch (ArgumentException e)
            {
                invalidPatterns.Add((pattern, e.Message));
            }
        }

        InvalidPatterns = invalidPatterns;
    }

    public bool HasRules => _rules.Count > 0;

    // The sensitive parts of the text, in order and without overlaps.
    public IReadOnlyList<SensitiveDataMatch> Find(string text)
    {
        if (string.IsNullOrEmpty(text) || _rules.Count == 0)
        {
            return [];
        }

        string digitText = ToDigitText(text);
        List<SensitiveDataMatch> matches = [];

        foreach (Rule rule in _rules)
        {
            string input = rule.UseDigitText ? digitText : text;

            try
            {
                // When a match fails its checks, the search goes on from the next character rather than after the
                // match, since the real value can start inside it: "pedido 1 4111 1111 1111 1111".
                for (Match match = rule.Regex.Match(input); match.Success;)
                {
                    Group censor = match.Groups[CensorGroupName];
                    Capture capture = censor.Success ? censor : match;

                    if (capture.Length > 0 && (rule.IsValid == null || rule.IsValid(capture.Value)))
                    {
                        matches.Add(new SensitiveDataMatch(rule.Type, capture.Index, capture.Length));
                        match = rule.Regex.Match(input, match.Index + Math.Max(match.Length, 1));
                    }
                    else
                    {
                        match = match.Index + 1 < input.Length ? rule.Regex.Match(input, match.Index + 1) : Match.Empty;
                    }
                }
            }
            catch (RegexMatchTimeoutException)
            {
                // A custom pattern that takes too long on this line is skipped rather than holding up the capture.
            }
        }

        return Merge(matches);
    }

    private static List<SensitiveDataMatch> Merge(List<SensitiveDataMatch> matches)
    {
        List<SensitiveDataMatch> merged = [];

        foreach (SensitiveDataMatch match in matches.OrderBy(match => match.Index).ThenByDescending(match => match.Length))
        {
            if (merged.Count > 0 && match.Index <= merged[^1].End)
            {
                SensitiveDataMatch last = merged[^1];
                merged[^1] = last with { Length = Math.Max(last.End, match.End) - last.Index };
            }
            else
            {
                merged.Add(match);
            }
        }

        return merged;
    }

    // OCR often reads a 0 as the letter O and a 1 as I or l. Numbers are checked in a copy of the text with those letters
    // read as digits when they touch a digit, so "l23.456.789-O9" is read as a CPF but the "o" of "Cartão 4111" is left
    // alone. The copy has the same length, so what is found in it is at the same place in the text.
    private static string ToDigitText(string text)
    {
        return string.Create(text.Length, text, (span, source) =>
        {
            for (int i = 0; i < source.Length; i++)
            {
                char digit = source[i] switch
                {
                    'O' or 'o' => '0',
                    'I' or 'l' or '|' => '1',
                    char c => c
                };

                bool touchesDigit = (i > 0 && char.IsAsciiDigit(span[i - 1])) || (i + 1 < source.Length && char.IsAsciiDigit(source[i + 1]));
                span[i] = touchesDigit ? digit : source[i];
            }
        });
    }

    private static Regex Create(string pattern, RegexOptions options = RegexOptions.None, bool compiled = true)
    {
        return new Regex(pattern, DefaultOptions | options | (compiled ? RegexOptions.Compiled : RegexOptions.None), MatchTimeout);
    }

    private static string GetDigits(string value) => new(value.Where(char.IsAsciiDigit).ToArray());

    private static bool IsRepeatedDigit(string digits) => digits.Length > 0 && digits.All(c => c == digits[0]);

    // Random keys mix upper case, lower case and digits and switch between them all the time: about six times in ten
    // characters. Words and identifiers such as "MyComponentName2024" switch about three times, and hexadecimal hashes
    // have no upper case.
    internal static bool IsRandomLooking(string value)
    {
        int upper = value.Count(char.IsAsciiLetterUpper);
        int lower = value.Count(char.IsAsciiLetterLower);
        int digits = value.Count(char.IsAsciiDigit);

        if (upper < 2 || lower < 2 || digits < 2)
        {
            return false;
        }

        int switches = 0;

        for (int i = 1; i < value.Length; i++)
        {
            if (GetCharacterClass(value[i]) != GetCharacterClass(value[i - 1]))
            {
                switches++;
            }
        }

        return (float)switches / value.Length >= 0.4f && GetEntropy(value) >= 3.5;
    }

    private static int GetCharacterClass(char c) => char.IsAsciiLetterUpper(c) ? 0 : char.IsAsciiLetterLower(c) ? 1 : char.IsAsciiDigit(c) ? 2 : 3;

    // Shannon entropy in bits per character.
    private static double GetEntropy(string value)
    {
        return value.GroupBy(c => c).Sum(group =>
        {
            double p = (double)group.Count() / value.Length;
            return -p * Math.Log2(p);
        });
    }

    // Loopback, unspecified and mask addresses say nothing about anyone, so they are left alone.
    internal static bool IsPublicOrPrivateIPv4(string value)
    {
        return !value.StartsWith("127.", StringComparison.Ordinal) && !value.StartsWith("0.", StringComparison.Ordinal) &&
            !value.StartsWith("255.", StringComparison.Ordinal);
    }

    // Needs three groups at least, so "std::" or "a::b" in code is not taken for an address.
    internal static bool IsIPv6(string value)
    {
        return value.Split(':').Count(group => group.Length > 0) >= 3 && IPAddress.TryParse(value, out IPAddress? address) &&
            address.AddressFamily == AddressFamily.InterNetworkV6 && !address.Equals(IPAddress.IPv6Loopback) && !address.Equals(IPAddress.IPv6Any);
    }

    // The whole number is checked, and so is the number before each space or dash, so a card number followed by a
    // security code or another number ("4111 1111 1111 1111 123") is still found and censored with what follows it.
    internal static bool IsCreditCardNumber(string value)
    {
        string digits = GetDigits(value);
        int count = 0;

        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsAsciiDigit(value[i]))
            {
                count++;

                if ((i + 1 == value.Length || !char.IsAsciiDigit(value[i + 1])) && IsCardDigits(digits[..count]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Card numbers start with 3 (Amex, Diners, JCB), 4 (Visa, Elo), 5 (Mastercard, Elo) or 6 (Discover, Elo, Hipercard)
    // and pass the Luhn check.
    private static bool IsCardDigits(string digits)
    {
        if (digits.Length < 13 || digits.Length > 19 || digits[0] < '3' || digits[0] > '6' || IsRepeatedDigit(digits))
        {
            return false;
        }

        int sum = 0;

        for (int i = 0; i < digits.Length; i++)
        {
            int digit = digits[digits.Length - 1 - i] - '0';

            if (i % 2 == 1)
            {
                digit *= 2;

                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
        }

        return sum % 10 == 0;
    }

    internal static bool IsCPF(string value)
    {
        string digits = GetDigits(value);

        if (digits.Length != 11 || IsRepeatedDigit(digits))
        {
            return false;
        }

        return GetCheckDigit(digits.AsSpan(0, 9), 10) == digits[9] - '0' && GetCheckDigit(digits.AsSpan(0, 10), 11) == digits[10] - '0';

        static int GetCheckDigit(ReadOnlySpan<char> digits, int firstWeight)
        {
            int sum = 0;

            for (int i = 0; i < digits.Length; i++)
            {
                sum += (digits[i] - '0') * (firstWeight - i);
            }

            int remainder = sum * 10 % 11;
            return remainder == 10 ? 0 : remainder;
        }
    }

    // Each character counts as its code minus the code of '0', so digits keep their value and 'A' counts 17. This is the
    // rule for the alphanumeric CNPJ, and it gives the same result as before for numeric ones.
    internal static bool IsCNPJ(string value)
    {
        string characters = new(value.ToUpperInvariant().Where(char.IsAsciiLetterOrDigit).ToArray());

        if (characters.Length != 14 || !char.IsAsciiDigit(characters[12]) || !char.IsAsciiDigit(characters[13]) || IsRepeatedDigit(characters))
        {
            return false;
        }

        return GetCheckDigit(characters.AsSpan(0, 12)) == characters[12] - '0' && GetCheckDigit(characters.AsSpan(0, 13)) == characters[13] - '0';

        static int GetCheckDigit(ReadOnlySpan<char> characters)
        {
            int sum = 0;
            int weight = characters.Length - 7;

            for (int i = 0; i < characters.Length; i++)
            {
                sum += (characters[i] - '0') * weight;
                weight = weight == 2 ? 9 : weight - 1;
            }

            int remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }

    internal static bool IsPhoneNumber(string value)
    {
        int digits = GetDigits(value).Length;
        return digits >= 10 && digits <= 13;
    }

    private sealed record Rule(SensitiveDataType Type, Regex Regex, Func<string, bool>? IsValid = null, bool UseDigitText = false);
}
