using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Globalization;

namespace DataViz
{
    public sealed class JsonReader
    {
        internal static readonly long MinDateTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0)).Ticks;
        internal static readonly DateTime MinDate = new DateTime(100, 1, 1, 0, 0, 0);

        TextReader _reader;

        public JsonReader(string jsonText)
            : this(new StringReader(jsonText)) {}

        public JsonReader(TextReader reader)
        {
            _reader = reader;
        }

        char GetNextCharacter()
        {
            return (char)_reader.Read();
        }

        char GetNextSignificantCharacter()
        {
            char ch = (char)_reader.Read();
            while ((ch != '\0') && Char.IsWhiteSpace(ch))
            {
                ch = (char)_reader.Read();
            }
            return ch;
        }

        string GetCharacters(int count)
        {
            string s = String.Empty;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)_reader.Read();
                if (ch == '\0')
                {
                    return null;
                }
                s += ch;
            }
            return s;
        }

        char PeekNextSignificantCharacter()
        {
            char ch = (char)_reader.Peek();
            while ((ch != '\0') && Char.IsWhiteSpace(ch))
            {
                _reader.Read();
                ch = (char)_reader.Peek();
            }
            return ch;
        }

        List<object> ReadArray()
        {
            var array = new List<object>();
            
            // Consume the '['
            _reader.Read();
            while (true)
            {
                char ch = PeekNextSignificantCharacter();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated array literal.");
                }

                if (ch == ']')
                {
                    _reader.Read();
                    return array;
                }

                if (array.Count != 0)
                {
                    if (ch != ',')
                    {
                        throw new FormatException("Invalid array literal.");
                    }
                    else
                    {
                        _reader.Read();
                    }
                }

                object item = ReadValue();
                array.Add(item);
            }
        }

        bool ReadBoolean()
        {
            string s = ReadName(/* allowQuotes */ false);
            if (s != null)
            {
                if (s.Equals("true", StringComparison.Ordinal))
                {
                    return true;
                }
                else if (s.Equals("false", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            throw new FormatException("Invalid boolean literal.");
        }

        string ReadName(bool allowQuotes)
        {
            char ch = PeekNextSignificantCharacter();

            if ((ch == '"') || (ch == '\''))
            {
                if (allowQuotes)
                {
                    return ReadString();
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    ch = (char)_reader.Peek();
                    if ((ch == '_') || Char.IsLetterOrDigit(ch))
                    {
                        _reader.Read();
                        sb.Append(ch);
                    }
                    else
                    {
                        return sb.ToString();
                    }
                }
            }

            return null;
        }

        void ReadNull()
        {
            string s = ReadName(/* allowQuotes */ false);
            if ((s == null) || !s.Equals("null", StringComparison.Ordinal))
            {
                throw new FormatException("Invalid null literal.");
            }
        }

        object ReadNumber()
        {
            char ch = (char)_reader.Read();

            StringBuilder sb = new StringBuilder();
            bool hasDecimal = (ch == '.');

            sb.Append(ch);
            while (true)
            {
                ch = PeekNextSignificantCharacter();

                if (Char.IsDigit(ch) || (ch == '.'))
                {
                    _reader.Read();
                    sb.Append(ch);
                }
                else
                {
                    break;
                }
            }

            string s = sb.ToString();
            if (hasDecimal)
            {
                float value;
                if (Single.TryParse(s, out value))
                {
                    return value;
                }
            }
            else
            {
                int value;
                if (Int32.TryParse(s, out value))
                {
                    return value;
                }
                else
                {
                    long lvalue;
                    if (Int64.TryParse(s, out lvalue))
                    {
                        return lvalue;
                    }
                }
            }

            throw new FormatException("Invalid numeric literal.");
        }

        JsObject ReadObject()
        {
            JsObject record = new JsObject();
            
            // Consume the '{'
            _reader.Read();
            while (true)
            {
                char ch = PeekNextSignificantCharacter();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated object literal.");
                }

                if (ch == '}')
                {
                    _reader.Read();
                    return record;
                }

                if (record.Count != 0)
                {
                    if (ch != ',')
                    {
                        throw new FormatException("Invalid object literal.");
                    }
                    else
                    {
                        _reader.Read();
                    }
                }

                string name = ReadName(/* allowQuotes */ true);
                ch = PeekNextSignificantCharacter();

                if (ch != ':')
                {
                    throw new FormatException("Unexpected name/value pair syntax in object literal.");
                }
                else
                {
                    _reader.Read();
                }

                object item = ReadValue();
                record[name] = item;
            }
        }

        string ReadString()
        {
            bool dummy;
            return ReadString(out dummy);
        }

        string ReadString(out bool hasLeadingSlash)
        {
            StringBuilder sb = new StringBuilder();

            char endQuoteCharacter = (char)_reader.Read();
            bool inEscape = false;
            bool firstCharacter = true;

            hasLeadingSlash = false;

            while (true)
            {
                char ch = GetNextCharacter();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated string literal.");
                }
                if (firstCharacter)
                {
                    if (ch == '\\')
                    {
                        hasLeadingSlash = true;
                    }
                    firstCharacter = false;
                }

                if (inEscape)
                {
                    if (ch == 'u')
                    {
                        string unicodeSequence = GetCharacters(4);
                        if (unicodeSequence == null)
                        {
                            throw new FormatException("Unterminated string literal.");
                        }
                        ch = (char)Int32.Parse(unicodeSequence, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }

                    sb.Append(ch);
                    inEscape = false;
                    continue;
                }

                if (ch == '\\')
                {
                    inEscape = true;
                    continue;
                }

                if (ch == endQuoteCharacter)
                {
                    return sb.ToString();
                }

                sb.Append(ch);
            }
        }

        public object ReadValue()
        {
            object value = null;
            bool allowNull = false;

            char ch = PeekNextSignificantCharacter();
            if (ch == '[')
            {
                value = ReadArray();
            }
            else if (ch == '{')
            {
                value = ReadObject();
            }
            else if ((ch == '\'') || (ch == '"'))
            {
                bool hasLeadingSlash;
                string s = ReadString(out hasLeadingSlash);
                if (hasLeadingSlash && s.StartsWith("@") && s.EndsWith("@"))
                {
                    long ticks;

                    if (Int64.TryParse(s.Substring(1, s.Length - 2), out ticks))
                    {
                        value = new DateTime(ticks * 10000 + JsonReader.MinDateTimeTicks, DateTimeKind.Utc);
                    }
                }

                if (value == null)
                {
                    value = s;
                }
            }
            else if (Char.IsDigit(ch) || (ch == '-') || (ch == '.'))
            {
                value = ReadNumber();
            }
            else if ((ch == 't') || (ch == 'f'))
            {
                value = ReadBoolean();
            }
            else if (ch == 'n')
            {
                ReadNull();
                allowNull = true;
            }

            if ((value == null) && (allowNull == false))
            {
                throw new FormatException("Invalid JSON text.");
            }
            return value;
        }
    }
}