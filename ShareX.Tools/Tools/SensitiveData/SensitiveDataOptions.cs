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

using System.ComponentModel;

namespace ShareX.Tools;

public enum SensitiveDataCensorMethod // Localized
{
    // A solid box hides the text for good. Pixelated or blurred text can sometimes be read back, so they are not the default.
    [Description("Solid box")]
    Fill,
    [Description("Pixelate")]
    Pixelate,
    [Description("Blur")]
    Blur
}

public enum SensitiveDataType
{
    Password,
    APIKey,
    IPAddress,
    EmailAddress,
    CreditCardNumber,
    CPF,
    CNPJ,
    PhoneNumber,
    Custom
}

// What the "Censor sensitive data" after capture task looks for in the text of a screenshot, and how it hides it.
public sealed class SensitiveDataOptions
{
    // A value written after a label such as "password:", "senha =" or "token:".
    public bool CensorPasswords { get; set; } = true;
    // Keys and tokens in well-known formats (AWS, GitHub, Slack, Google, Stripe, OpenAI, JWT...) and long random strings.
    public bool CensorAPIKeys { get; set; } = true;
    public bool CensorIPAddresses { get; set; } = true;
    public bool CensorEmailAddresses { get; set; } = true;
    public bool CensorCreditCardNumbers { get; set; } = true;
    // Brazilian individual taxpayer number, checked with its check digits.
    public bool CensorCPF { get; set; } = true;
    // Brazilian company number, numeric or alphanumeric, checked with its check digits.
    public bool CensorCNPJ { get; set; } = true;
    // Off by default: phone numbers look like many other numbers.
    public bool CensorPhoneNumbers { get; set; }
    // Regular expressions of the user's own. Only the group named "censor" is hidden when the pattern has one.
    public List<string> CustomPatterns { get; set; } = [];
    public SensitiveDataCensorMethod CensorMethod { get; set; } = SensitiveDataCensorMethod.Fill;

    // A method rather than a property, so it is not written to the settings file.
    public bool IsAnyEnabled() => CensorPasswords || CensorAPIKeys || CensorIPAddresses || CensorEmailAddresses || CensorCreditCardNumbers ||
        CensorCPF || CensorCNPJ || CensorPhoneNumbers || CustomPatterns.Any(pattern => !string.IsNullOrWhiteSpace(pattern));
}
