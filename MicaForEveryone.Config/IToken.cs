namespace MicaForEveryone.Config
{
    /// <summary>
    /// Token is a string with a position from config file
    /// </summary>
    public interface IToken
    {
        int Line { get; }
        int Column { get; }
        string Data { get; }
    }
}
