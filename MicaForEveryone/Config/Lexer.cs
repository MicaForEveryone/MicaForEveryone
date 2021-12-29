using System;
using System.Collections.Generic;

namespace MicaForEveryone.Config
{
    internal class Lexer
    {
        private readonly string[] _data;
        private readonly List<LexicalToken> _result = new();

        private int _line;
        private int _column;
        private int _start;
        private LexicalTokenType _type;

        public Lexer(string[] data)
        {
            _data = data;
        }

        private string Line => _data[_line];

        public LexicalToken[] Parse()
        {
            if (_result.Count > 0)
                return _result.ToArray();

            while (_line < _data.Length)
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
            _result.Add(new LexicalToken(LexicalTokenType.Space, "\n", _line, _column));
            _line++;
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
            if (Line[_column] != '"')
                throw new FormatException($"string literal not closed at ({_line}:{_column})");
            var data = Line.Substring(_start + 1, _column - _start - 1);
            _result.Add(new LexicalToken(LexicalTokenType.StringLiteral, data, _line, _column));
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
