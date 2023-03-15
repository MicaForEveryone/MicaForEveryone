namespace MicaForEveryone.Core.Interfaces
{
    public interface IConfigParser
    {
        IRule[] Rules { get; }

        Task LoadAsync(StreamReader path);
        Task SaveAsync(StreamWriter path);

        void AddRule(IRule rule);
        void SetRule(IRule rule);
        void RemoveRule(IRule rule);
    }
}
