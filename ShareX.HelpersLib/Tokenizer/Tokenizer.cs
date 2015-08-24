#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.HelpersLib
{
    public class Tokenizer
    {
        public char[] WhitespaceChars { get; set; }
        public char[] SymbolChars { get; set; }
        public char[] LiteralDelimiters { get; set; }
        public string[] Keywords { get; set; }
        public char LiteralEscapeChar { get; set; }
        public bool KeepWhitespace { get; set; }
        public bool AutoParseLiteral { get; set; }

        public Tokenizer()
        {
            WhitespaceChars = new char[] { ' ', '\t', '\r', '\n' };
            SymbolChars = new char[] { ',', ':', ';', '+', '-', '*', '/', '^', '=', '<', '>', '(', ')', '[', ']', '{', '}', '?', '!', '&', '|' };
            LiteralDelimiters = new char[] { '"', '\'' };
            LiteralEscapeChar = '\\';
            Keywords = new string[] { "if", "else", "return" };
            KeepWhitespace = false;
        }

        public List<Token> TokenizeFile(string filePath)
        {
            string text = File.ReadAllText(filePath, Encoding.UTF8);
            return Tokenize(text);
        }

        public List<Token> Tokenize(string text)
        {
            List<Token> tokens = new List<Token>();
            Token currentToken = null;
            char currentChar;

            for (int i = 0; i < text.Length; i++)
            {
                currentChar = text[i];

                if (WhitespaceChars.Contains(currentChar)) // Whitespace
                {
                    CheckIdentifier(currentToken);

                    if (KeepWhitespace)
                    {
                        currentToken = new Token(TokenType.Whitespace, currentChar.ToString(), i);
                        tokens.Add(currentToken);
                    }
                    else
                    {
                        currentToken = new Token();
                    }
                }
                else if (SymbolChars.Contains(currentChar)) // Symbol
                {
                    CheckIdentifier(currentToken);

                    currentToken = new Token(TokenType.Symbol, currentChar.ToString(), i);
                    tokens.Add(currentToken);
                }
                else if (LiteralDelimiters.Contains(currentChar)) // Literal
                {
                    CheckIdentifier(currentToken);

                    currentToken = new Token(TokenType.Literal, currentChar.ToString(), i);
                    tokens.Add(currentToken);

                    char delimeter = currentChar;

                    for (i++; i < text.Length; i++)
                    {
                        currentChar = text[i];
                        currentToken.Text += currentChar;

                        if (currentChar == delimeter && !IsEscaped(currentToken.Text))
                        {
                            if (AutoParseLiteral)
                            {
                                currentToken.Text = ParseString(currentToken.Text);
                            }

                            break;
                        }
                    }
                }
                else // Identifier, Numeric, Keyword
                {
                    if (currentToken != null && currentToken.Type == TokenType.Identifier)
                    {
                        currentToken.Text += currentChar;
                    }
                    else
                    {
                        currentToken = new Token(TokenType.Identifier, currentChar.ToString(), i);
                        tokens.Add(currentToken);
                    }

                    if (i + 1 >= text.Length) // EOF
                    {
                        CheckIdentifier(currentToken);
                    }
                }
            }

            return tokens;
        }

        public string ParseString(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > 1)
            {
                StringBuilder sb = new StringBuilder();
                char delimeter = text[0];
                int length;

                if (text[text.Length - 1] == delimeter && !IsEscaped(text))
                {
                    length = text.Length - 1;
                }
                else
                {
                    length = text.Length;
                }

                for (int i = 1; i < length; i++)
                {
                    if (text[i] != LiteralEscapeChar || text[i - 1] == LiteralEscapeChar)
                    {
                        sb.Append(text[i]);
                    }
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        private void CheckIdentifier(Token token)
        {
            if (token != null && token.Type == TokenType.Identifier)
            {
                double result;

                if (double.TryParse(token.Text, out result))
                {
                    token.Type = TokenType.Numeric;
                }
                else if (Keywords.Contains(token.Text))
                {
                    token.Type = TokenType.Keyword;
                }
            }
        }

        private bool IsEscaped(string text, int position = -1)
        {
            if (position == -1)
            {
                position = text.Length - 1;
            }

            if (position > 0 && text[position - 1] == LiteralEscapeChar)
            {
                if (position > 1 && text[position - 2] == LiteralEscapeChar)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static BetweenTagsResult GetBetweenTags(List<Token> tokens, string startTag, string endTag, int startIndex = 0)
        {
            BetweenTagsResult result = new BetweenTagsResult();
            result.StartIndex = startIndex;
            result.TokenList = new List<Token>();

            int depth = 0;
            bool isStartFound = false;
            int i;

            for (i = startIndex; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (token.Type == TokenType.Symbol)
                {
                    if (isStartFound)
                    {
                        if (token.Text == startTag)
                        {
                            depth++;
                        }
                        else if (token.Text == endTag)
                        {
                            if (depth == 0)
                            {
                                i++;
                                result.Status = true;
                                break;
                            }

                            depth--;
                        }
                    }
                    else
                    {
                        if (token.Text == startTag)
                        {
                            isStartFound = true;
                            continue;
                        }
                    }
                }

                if (isStartFound)
                {
                    result.TokenList.Add(token);
                }
            }

            result.EndIndex = i;
            return result;
        }
    }
}