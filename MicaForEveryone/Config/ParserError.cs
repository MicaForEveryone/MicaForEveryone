using System;

namespace MicaForEveryone.Config
{
    public class ParserError : Exception
    {
        public ParserError(IToken token, string message) : 
            base($"Error in token `{token.Data}` at line {token.Line + 1}, column {token.Column + 1}:\n{message}")
        {
            Token = token;
        }

        public IToken Token { get; }
    }

    public class InvalidSymbolError : ParserError
    {
        internal InvalidSymbolError(Symbol symbol, string message) : base(symbol.Token, message)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get; }
    }

    public class UnexpectedTokenError : ParserError
    {
        public UnexpectedTokenError(IToken token, string message) : base(token, message)
        {
        }
    }

    public class UnexpectedEndOfFile : ParserError
    {
        internal UnexpectedEndOfFile(IToken token) : base(token, "Unexpected End of File")
        {
        }
    }
}
