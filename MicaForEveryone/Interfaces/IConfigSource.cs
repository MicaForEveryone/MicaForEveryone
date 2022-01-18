using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicaForEveryone.Interfaces
{
    public interface IConfigSource
    {
        IEnumerable<IRule> GetRules();
        void SetRule(IRule rule);
        void OpenInEditor();
        Task LoadAsync();
        Task SaveAsync();
    }
}
