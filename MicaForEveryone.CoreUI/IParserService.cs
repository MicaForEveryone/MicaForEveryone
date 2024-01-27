using MicaForEveryone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.CoreUI;

public interface IParserService
{
    Task<IEnumerable<Rule>> GetRulesAsync();

    Task ReadAsync(Stream stream);

    Task WriteAsync(Stream stream);

    IEnumerable<Rule> AddRule(Rule rule);

    IEnumerable<Rule> ModifyRule(Rule rule);

    IEnumerable<Rule> RemoveRule(Rule rule);
}