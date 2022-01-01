using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MicaForEveryone.Config
{
    internal class Lexer
    {
        private readonly TextReader _source;
        private readonly List<LexicalToken> _result = new();

        private string _lineData;
        private int _line;
        private int _column;
        private int _start;
        private LexicalTokenType _type;

        public Lexer(TextReader data)
        {
            _source = data;
        }

        private string Line => _lineData;

        public async Task<LexicalToken[]> ParseAsync()
        {
            if (_result.Count > 0)
                return _result.ToArray();

            _lineData = await _source.ReadLineAsync();

            while (_lineData != null)
            {
                while (_column < Line.Length)
                {
                    switch (Line[_column])
                    {
                        case '#':
                        case ';':
                            if (_start != _column)
                            {
                                AddToken();
                            }
                            AddComment();
                            break;

                        case '"':
                            if (_start != _column)
                            {
                                AddToken();
                            }
                            AddStringLiteral();
                            break;

                        case ' ':
                        case '\t':
                            if (_start != _column && _type != LexicalTokenType.Space)
                            {
                                AddToken();
                            }
                            if (_start == _column)
                            {
                                _type = LexicalTokenType.Space;
                            }
                            _column++;
                            break;

                        case '{':
                        case '}':
                        case '=':
                        case ':':
                            if (_start != _column && _type != LexicalTokenType.Operator)
                            {
                                AddToken();
                            }
                            if (_start == _column)
                            {
                                _type = LexicalTokenType.Operator;
                            }
                            _column++;
                            break;

                        default:
                            if (_start != _column && _type != LexicalTokenType.Identifier)
                            {
                                AddToken();
                            }
                            if (_start == _column)
                            {
                                _type = LexicalTokenType.Identifier;
                            }
                            _column++;
                            break;
                    }
                }

                if (_start != _column)
                {
                    AddToken();
                }

                AddLine();
            }

            return _result.ToArray();
        }

        private void AddLine()
        {
            _result.Add(new LexicalToken(LexicalTokenType.NewLine, "", _line, _column));
            _line++;
            _lineData = _source.ReadLine();
            _column = 0;
            _start = 0;
        }

        private void AddComment()
        {
            _result.Add(new LexicalToken(LexicalTokenType.Comment, Line.Substring(_column), _line, _start));
            _column = Line.Length;
            _start = _column;
        }

        private void AddStringLiteral()
        {
            _column++;
            while (_column < Line.Length)
            {
                if (Line[_column] == '"')
                    break;
                _column++;
            }
            var data = Line.Substring(_start + 1, _column - _start - 1);
            var token = new LexicalToken(LexicalTokenType.StringLiteral, data, _line, _column);
            if (Line[_column] != '"')
                throw new ParserError(token, "String Literal not closed");
            _result.Add(token);
            _column++;
            _start = _column;
        }

        private void AddToken()
        {
            var data = Line.Substring(_start, _column - _start);
            _result.Add(new LexicalToken(_type, data, _line, _start));
            _start = _column;
        }
    }
}
